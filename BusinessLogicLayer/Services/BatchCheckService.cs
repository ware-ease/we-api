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
            while (!stoppingToken.IsCancellationRequested)
            {
                //figure out the next run time: today at 8am or tomorrow if its already past that
                var now = DateTime.Now;
                var nextRunTime = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);
                if (now > nextRunTime)
                {
                    //if it already past 8am today, so schedule for 8am tomorrow
                    nextRunTime = nextRunTime.AddDays(1);
                }
                var delay = nextRunTime - now;
                _logger.LogInformation($"[BatchCheckService] Đang đợi đến {nextRunTime} để chạy (còn {delay}).");
                //wait until the next run time
                await Task.Delay(delay, stoppingToken);

                try
                {
                    //time to do the batch check
                    await DoBatchCheckAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[BatchCheckService] Lỗi khi kiểm tra batch.");
                }

                //done with the batch check, now delay for 24 hours to run it again at the same time tomorrow
                _logger.LogInformation($"[BatchCheckService] bắt đầu đợi 24h).");
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                //await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task DoBatchCheckAsync(CancellationToken stoppingToken)
        {
            const int batchSize = 100;
            int skipWarehouse = 0;

            //resolve repositories and services from the DI scope
            using var scope = _scopeFactory.CreateScope();
            var _warehouseRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Warehouse>>();
            var _batchRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Batch>>();
            var _productRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Product>>();
            var _inventoryRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Inventory>>();
            var _accountRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Account>>();
            var _firebaseService = scope.ServiceProvider.GetRequiredService<IFirebaseService>();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            //loop through all warehouses in chunks
            while (!stoppingToken.IsCancellationRequested)
            {
                var warehouses = await _warehouseRepository.Search(
                    filter: w => true,
                    orderBy: q => q.OrderBy(w => w.Name),
                    pageIndex: skipWarehouse / batchSize,
                    pageSize: batchSize
                );

                if (!warehouses.Any())
                {
                    _logger.LogInformation("[BatchCheckService] Không còn kho nào để kiểm tra.");
                    break;
                }

                foreach (var warehouse in warehouses)
                {
                    int skipInventory = 0;
                    //loop through all inventories in this warehouse in chunks
                    while (true)
                    {
                        var inventories = await _inventoryRepository.Search(
                            filter: i => i.WarehouseId == warehouse.Id && i.CurrentQuantity > 0,
                            orderBy: q => q.OrderBy(i => i.Batch.ExpDate),
                            includeProperties: "Batch",
                            pageIndex: skipInventory/batchSize,
                            pageSize: batchSize
                        );

                        if (!inventories.Any())
                        {
                            _logger.LogInformation("[BatchCheckService] Không còn inventory nào để kiểm tra.");
                            break;
                        }

                        //find all the warehouse keeper for this warehouse to notify
                        var managers = (await _accountRepository.Search(
                            filter: a => a.AccountWarehouses.Any(aw => aw.WarehouseId == warehouse.Id)
                            && a.AccountGroups.Any(ag => ag.GroupId == "2"),
                            includeProperties: "AccountWarehouses,AccountGroups")).Select(m => m.Id).ToList();

                        foreach (var inventory in inventories)
                        {
                            var batch = inventory.Batch;
                            if (batch == null || batch.ExpDate == null)
                                continue;

                            //calculate how many days left until this batch expires
                            var daysUntilExpiry = (batch.ExpDate.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Now).TotalDays;
                            var product = await _productRepository.GetByCondition(p => p.Id == batch.ProductId);

                            //check if this batch is already expired
                            switch (daysUntilExpiry)
                            {
                                case <= 0:
                                    {
                                        //expired, set AlertLevel to 2 if not already
                                        if (batch.AlertLevel == 2) continue;
                                        batch.AlertLevel = 2;
                                        _batchRepository.Update(batch);
                                        await _unitOfWork.SaveAsync();
                                        await _firebaseService.SendNotificationToUsersAsync(managers, "Lô đã hết hạn.", $"Lô {batch.Code} thuộc sản phẩm {product.Sku} đã hết hạn, hạn của phẩm: {batch.ExpDate.Value:dd/MM/yyyy}",
                                            NotificationType.ALERT_LEVEL_2, null);
                                        _logger.LogInformation($"[BatchCheckService] Gửi cảnh báo bậc 2 cho batch {batch.Id} tại kho {warehouse.Name}, đã hết hạn sử dụng {(int)daysUntilExpiry}.");
                                        _logger.LogInformation($"[BatchCheckService] Lô {batch.Code} thuộc sản phẩm {product.Sku} đã hết hạn sử dụng {batch.ExpDate.Value.ToString("dd/MM/yyyy")}");
                                        break;
                                    }
                                case <= 30:
                                    {
                                        //less than 30 days left, set AlertLevel to 2 if not already
                                        if (batch.AlertLevel == 2) continue;
                                        batch.AlertLevel = 2;
                                        _batchRepository.Update(batch);
                                        await _unitOfWork.SaveAsync();
                                        await _firebaseService.SendNotificationToUsersAsync(managers, "Lô gần hết hạn.", $"Lô {batch.Code} thuộc sản phẩm {product.Sku} sẽ hết hạn vào ngày {batch.ExpDate.Value:dd/MM/yyyy} (còn {(int)daysUntilExpiry} ngày)",
                                            NotificationType.ALERT_LEVEL_2, null);
                                        _logger.LogInformation($"[BatchCheckService] Gửi cảnh báo bậc 2 cho batch {batch.Id} tại kho {warehouse.Name}, đã hết hạn sử dụng {(int)daysUntilExpiry}.");
                                        _logger.LogInformation($"[BatchCheckService] Lô {batch.Code} thuộc sản phẩm {product.Sku} đã hết hạn sử dụng {batch.ExpDate.Value.ToString("dd/MM/yyyy")}");
                                        break;
                                    }
                                case <= 90:
                                    {
                                        //between 30 and 90 days left, set AlertLevel to 1 if not already
                                        if (batch.AlertLevel == 1 || batch.AlertLevel == 2) continue;
                                        batch.AlertLevel = 1;
                                        _batchRepository.Update(batch);
                                        await _unitOfWork.SaveAsync();
                                        await _firebaseService.SendNotificationToUsersAsync(managers, "Lô gần hết hạn.", $"Lô {batch.Code} thuộc sản phẩm {product.Sku} sẽ hết hạn vào ngày {batch.ExpDate.Value:dd/MM/yyyy} (còn {(int)daysUntilExpiry} ngày)",
                                            NotificationType.ALERT_LEVEL_1, null);
                                        _logger.LogInformation($"[BatchCheckService] Gửi cảnh báo bậc 2 cho batch {batch.Id} tại kho {warehouse.Name}, đã hết hạn sử dụng {(int)daysUntilExpiry}.");
                                        _logger.LogInformation($"[BatchCheckService] Lô {batch.Code} thuộc sản phẩm {product.Sku} đã hết hạn sử dụng {batch.ExpDate.Value.ToString("dd/MM/yyyy")}");
                                        break;
                                    }
                                default:
                                    //more than 90 days left, no alert
                                    continue;
                            }
                        }
                        //move to the next inventory page
                        skipInventory += batchSize;
                    }
                }

                //move to the next warehouse page
                skipWarehouse += batchSize;
            }
        }



        /*protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const int batchSize = 100;
            int skip = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                //bool hasExpiringBatch = false;

                using var scope = _scopeFactory.CreateScope();
                var _warehouseRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Warehouse>>();
                var _batchRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Batch>>();
                var _productRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Product>>();
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
                            var product = await _productRepository.GetByCondition(p => p.Id == batch.ProductId);

                            switch (daysUntilExpiry)
                            {
                                case <= 0:
                                    {
                                        if (batch.AlertLevel == 2)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            batch.AlertLevel = 2;
                                            _batchRepository.Update(batch);
                                            await _unitOfWork.SaveAsync();
                                            await _firebaseService.SendNotificationToUsersAsync(managers, "Lô đã hết hạn.", $"Lô {batch.Code} thuộc sản phẩm {product.Sku} đã hết hạn, hạn của của phẩm: {batch.ExpDate.Value.ToString("dd/MM/yyyy")}", NotificationType.ALERT_LEVEL_2, null);
                                            //_logger.LogInformation($"[BatchCheckService] Gửi cảnh báo bậc 2 cho batch {batch.Id} tại kho {warehouse.Name}, đã hết hạn sử dụng {(int)daysUntilExpiry}.");
                                            //_logger.LogInformation($"[BatchCheckService] Lô {batch.Code} thuộc sản phẩm {product.Sku} đã hết hạn sử dụng {batch.ExpDate.Value.ToString("dd/MM/yyyy")}");
                                        }
                                        break;
                                    }
                                case <= 30:
                                    {
                                        if (batch.AlertLevel == 2)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            batch.AlertLevel = 2;
                                            _batchRepository.Update(batch);
                                            await _unitOfWork.SaveAsync();
                                            await _firebaseService.SendNotificationToUsersAsync(managers, "Lô gần hết hạn.", $"Lô {batch.Code} thuộc sản phẩm {product.Sku} sẽ hết hạn vào ngày {batch.ExpDate.Value.ToString("dd/MM/yyyy")}(còn {(int)daysUntilExpiry} ngày trước khi hết hạn)", NotificationType.ALERT_LEVEL_2, null);
                                            //_logger.LogInformation($"[BatchCheckService] Gửi cảnh báo bậc 2 cho batch {batch.Id} tại kho {warehouse.Name}, hết hạn sau {(int)daysUntilExpiry} ngày.");
                                            //_logger.LogInformation($"[BatchCheckService] Lô {batch.Code} thuộc sản phẩm {product.Sku} sẽ hết hạn vào ngày {batch.ExpDate.Value.ToString("dd/MM/yyyy")}");
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
                                            _batchRepository.Update(batch);
                                            await _unitOfWork.SaveAsync();
                                            await _firebaseService.SendNotificationToUsersAsync(managers, "Lô gần hết hạn.", $"Lô {batch.Code} thuộc sản phẩm {product.Sku} sẽ hết hạn vào ngày {batch.ExpDate.Value.ToString("dd/MM/yyyy")}(còn {(int)daysUntilExpiry} ngày trước khi hết hạn)", NotificationType.ALERT_LEVEL_1, null);
                                            //_logger.LogInformation($"[BatchCheckService] Gửi cảnh báo bậc 1 cho batch {batch.Id} tại kho {warehouse.Name}, hết hạn sau {(int)daysUntilExpiry} ngày.");
                                            //_logger.LogInformation($"[BatchCheckService] Lô {batch.Code} thuộc sản phẩm {product.Sku} sẽ hết hạn vào ngày {batch.ExpDate.Value.ToString("dd/MM/yyyy")}");
                                        }
                                        break;
                                    }
                                default:
                                    continue;
                            }
                        }
                        
                    }
                    skip += batchSize;
                    //await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    //await Task.Delay(TimeSpan.FromHours(5), stoppingToken);
                }
            }
        }*/
    }
}
