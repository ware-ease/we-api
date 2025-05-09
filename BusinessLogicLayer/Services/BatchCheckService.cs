using BusinessLogicLayer.IServices;
using Data.Entity;
using DataAccessLayer.Generic;
using DataAccessLayer.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class BatchCheckService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BatchCheckService> _logger;
        private readonly IConfiguration _configuration;

        public BatchCheckService(IServiceScopeFactory scopeFactory, ILogger<BatchCheckService> logger, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const int batchSize = 100;
            int skip = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                //bool hasExpiringBatch = false;

                using var scope = _scopeFactory.CreateScope();
                var _warehouseRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Warehouse>>();
                var _inventoryRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Inventory>>();
                var _accountRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Account>>();
                var _firebaseService = scope.ServiceProvider.GetRequiredService<IFirebaseService>();

                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var warehouses = await _warehouseRepository.Search(
                    filter: w => true,
                    orderBy: q => q.OrderBy(w => w.Name),
                    pageIndex: skip / batchSize,
                    pageSize: batchSize
                );

                if (!warehouses.Any())
                {
                    _logger.LogInformation("[BatchExpirationMonitorService] Không còn kho nào để kiểm tra.");
                    skip = 0;
                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                }
                else
                {
                    foreach (var warehouse in warehouses)
                    {
                        var inventories = await _inventoryRepository.Search(
                            filter: i => i.WarehouseId == warehouse.Id && i.CurrentQuantity > 0,
                            orderBy: q => q.OrderBy(i => i.Batch.ExpDate),
                            includeProperties: "Batch",
                            pageIndex: 0,
                            pageSize: null
                        );

                        var managers = (await _accountRepository.Search(
                            filter: a => a.AccountWarehouses.Any(aw => aw.WarehouseId == warehouse.Id)
                            && a.AccountGroups.Any(ag => ag.GroupId == "2"),
                            includeProperties: "AccountWarehouses,AccountGroups")).Select(m => m.Id).ToList();

                        foreach (var inventory in inventories)
                        {
                            var batch = inventory.Batch;
                            if (batch == null || batch.ExpDate == null)
                                continue;

                            var daysUntilExpiry = (batch.ExpDate.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Now).TotalDays;

                            switch (daysUntilExpiry)
                            {
                                case <= 30:
                                    {
                                        if (batch.AlertLevel == 2)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            batch.AlertLevel = 2;
                                            _inventoryRepository.Update(inventory);
                                            await _unitOfWork.SaveAsync();
                                            await _firebaseService.SendNotificationToUsersAsync(managers, "Lô gần hết hạn.", $"Batch Id: {batch.Id}", NotificationType.ALERT_LEVEL_2, null);
                                            _logger.LogInformation($"[BatchCheckService] Gửi cảnh báo bậc 2 cho batch {batch.Id} tại kho {warehouse.Name}, hết hạn sau {daysUntilExpiry} ngày.");
                                        }
                                        break;
                                    }
                                case <= 90:
                                    {
                                        if (batch.AlertLevel == 1 || batch.AlertLevel == 2)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            batch.AlertLevel = 1;
                                            _inventoryRepository.Update(inventory);
                                            await _unitOfWork.SaveAsync();
                                            await _firebaseService.SendNotificationToUsersAsync(managers, "Lô sắp hết hạn.", $"Batch Id: {batch.Id}", NotificationType.ALERT_LEVEL_1, null);
                                            _logger.LogInformation($"[BatchCheckService] Gửi cảnh báo bậc 1 cho batch {batch.Id} tại kho {warehouse.Name}, hết hạn sau {daysUntilExpiry} ngày.");
                                        }
                                        break;
                                    }
                                default:
                                    continue;
                            }
                        }
                        
                    }
                    skip += batchSize;
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }
    }
}
