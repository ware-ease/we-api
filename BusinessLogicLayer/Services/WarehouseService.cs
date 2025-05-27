using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.Warehouse;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.UnitOfWork;
using Sprache;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services
{
    public class WarehouseService : GenericService<Warehouse>, IWarehouseService
    {
        IWarehouseRepository _warehouseRepository;
        IMapper _mapper;
        IUnitOfWork _unitOfWork;

        public WarehouseService(IGenericRepository<Warehouse> genericRepository, IMapper mapper, IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            _warehouseRepository = unitOfWork.WarehouseRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public override async Task<ServiceResponse> Get<TResult>()
        {
            var results = await _genericRepository.Search();

            IEnumerable<TResult> mappedResults = _mapper.Map<IEnumerable<TResult>>(results);

            foreach (var mappedResult in mappedResults)
            {
                if (mappedResult.CreatedBy != null)
                {
                    var createdBy = await GetCreatedBy(mappedResult.CreatedBy);

                    if (createdBy != null)
                    {
                        mappedResult.CreatedBy = createdBy!.Username;
                    }
                    else
                    {
                        mappedResult.CreatedBy = "Deleted user";
                    }
                }
            }

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Tìm thành công!",
                Data = mappedResults
            };
        }

        public async Task<ServiceResponse> GetFullWarehouseInfo<TResult>(string id)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByCondition(w => w.Id == id);

            if (warehouse == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Không tìm thấy kho!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(warehouse);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Tìm thành công!",
                Data = result
            };
        }

        public override async Task<ServiceResponse> Update<TResult, TRequest>(TRequest request)
        {
            var updateDto = request as UpdateWarehouseDTO;
            if (updateDto == null || string.IsNullOrEmpty(updateDto.Id))
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Error,
                    Message = "Invalid request data!"
                };
            }

            // 🔥 Tìm warehouse hiện có trong DB
            var existingEntity = await _genericRepository.GetByCondition(x => x.Id == updateDto.Id);
            if (existingEntity == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Warehouse not found!",
                    Data = updateDto.Id
                };
            }

            // 🔥 CHỈ CẬP NHẬT CÁC TRƯỜNG CÓ GIÁ TRỊ (Không ghi đè toàn bộ)
            if (!string.IsNullOrWhiteSpace(updateDto.Name))
            {
                existingEntity.Name = updateDto.Name;
            }
            if (!string.IsNullOrWhiteSpace(updateDto.Phone))
            {
                existingEntity.Phone = updateDto.Phone;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Address))
            {
                existingEntity.Address = updateDto.Address;
            }
            if (updateDto.Longitude.HasValue && updateDto.Longitude != 0)
            {
                existingEntity.Longitude = updateDto.Longitude.Value;
            }
            if (updateDto.Latitude.HasValue && updateDto.Latitude != 0)
            {
                existingEntity.Latitude = updateDto.Latitude.Value;
            }
            if (updateDto.Area.HasValue)
            {
                existingEntity.Area = updateDto.Area.Value;
            }

            if (updateDto.OperateFrom.HasValue)
            {
                existingEntity.OperateFrom = updateDto.OperateFrom.Value;
            }

            try
            {
                _genericRepository.Update(existingEntity);
                await _unitOfWork.SaveAsync();

                TResult result = _mapper.Map<TResult>(existingEntity);

                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Cập nhật thành công!",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Error,
                    Message = ex.Message,
                    Data = request
                };
            }
        }

        public async Task<ServiceResponse> SearchWarehouses(int? pageIndex = null, int? pageSize = null,
                                                                     string? keyword = null, float? minArea = null, float? maxArea = null)
        {
            Expression<Func<Warehouse, bool>> filter = w =>
                (string.IsNullOrEmpty(keyword) || w.Name.Contains(keyword)) &&
                (!minArea.HasValue || w.Area >= minArea) &&
                (!maxArea.HasValue || w.Area <= maxArea);

            var totalRecords = await _warehouseRepository.Count(filter);

            var results = await _warehouseRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize);

            var mappedResults = _mapper.Map<IEnumerable<WarehouseDTO>>(results);
            foreach (var mappedResult in mappedResults)
            {
                var createdByAccount = await _unitOfWork.AccountRepository.GetByCondition(a => a.Id == mappedResult.CreatedBy, "Profile,AccountGroups,AccountGroups.Group");
                if (createdByAccount != null)
                {
                    mappedResult.CreatedByAvatarUrl = createdByAccount.Profile!.AvatarUrl;
                    mappedResult.CreatedByFullName = $"{createdByAccount.Profile.FirstName} {createdByAccount.Profile.LastName}";
                    mappedResult.CreatedByGroup = createdByAccount.AccountGroups.FirstOrDefault()?.Group?.Name;
                }
            }
            int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? totalRecords));

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Search successful!",
                Data = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageIndex = pageIndex ?? 1,
                    PageSize = pageSize ?? totalRecords,
                    Records = mappedResults
                }
            };
        }

        public async Task<ServiceResponse> GetInventoryByWarehouseId(string warehouseId)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByCondition(i => i.Id == warehouseId,
                includeProperties: "Inventories,Inventories.Batch," +
                                    "Inventories.Batch.Product," +
                                    "Inventories.Batch.Product.Unit," +
                                    "Inventories.Batch.Product.Brand");

            if (warehouse == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Không tìm thấy kho.",
                    Data = warehouseId
                };
            }
            var result = _mapper.Map<WarehouseInventoryDTO>(warehouse);
            result.Inventories = _mapper.Map<IEnumerable<Data.Model.Request.Inventory.InventoryDTO>>(warehouse.Inventories);

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Tìm thành công.",
                Data = result
            };
        }
        public async Task<ServiceResponse> GetWarehouseStatisticsAsync(string? warehouseId)
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);
            var startOfLastMonth = startOfMonth.AddMonths(-1);
            var endOfLastMonth = startOfMonth.AddTicks(-1);

            var allDetails = await _unitOfWork.GoodNoteDetailRepository.Search(
                d => (string.IsNullOrEmpty(warehouseId) || d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId) &&
                     d.GoodNote.CreatedTime >= startOfLastMonth && d.GoodNote.CreatedTime <= endOfMonth,
                includeProperties: "GoodNote,GoodNote.GoodRequest"
            );

            var currentMonthDetails = allDetails.Where(d => d.GoodNote.CreatedTime >= startOfMonth && d.GoodNote.CreatedTime <= endOfMonth);
            var lastMonthDetails = allDetails.Where(d => d.GoodNote.CreatedTime >= startOfLastMonth && d.GoodNote.CreatedTime <= endOfLastMonth);

            // Hàm tính tổng số lượng theo loại phiếu
            float SumByNoteType(IEnumerable<GoodNoteDetail> details, GoodNoteEnum type) =>
                details.Where(d => d.GoodNote.NoteType == type).Sum(d => d.Quantity);

            // Chuyển kho tính theo phiếu receive thôi để tránh double
            float SumTransfer(IEnumerable<GoodNoteDetail> details) =>
                details
                    .Where(d => d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer
                                && d.GoodNote.NoteType == GoodNoteEnum.Receive)
                    .Sum(d => d.Quantity);

            // Hàm tính phần trăm thay đổi
            double CalcChangePercent(float current, float previous)
            {
                if (previous == 0) return current == 0 ? 0 : 100;
                return Math.Round(((double)(current - previous) / previous) * 100, 2);
            }

            // Tháng này
            float totalPutIn = SumByNoteType(currentMonthDetails, GoodNoteEnum.Receive);
            float totalTakeOut = SumByNoteType(currentMonthDetails, GoodNoteEnum.Issue);
            float totalTransfer = SumTransfer(currentMonthDetails);
            

            // Tháng trước
            float lastPutIn = SumByNoteType(lastMonthDetails, GoodNoteEnum.Receive);
            float lastTakeOut = SumByNoteType(lastMonthDetails, GoodNoteEnum.Issue);
            float lastTransfer = SumTransfer(lastMonthDetails);
            float lastStockChange = lastPutIn - lastTakeOut;
            float currentStockChange = lastStockChange + totalPutIn - totalTakeOut;

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Warehouse statistics retrieved successfully.",
                Data = new
                {
                    TotalPutIn = totalPutIn,
                    ChangePutIn = CalcChangePercent(totalPutIn, lastPutIn),

                    TotalTakeOut = totalTakeOut,
                    ChangeTakeOut = CalcChangePercent(totalTakeOut, lastTakeOut),

                    CurrentStockChange = currentStockChange,
                    ChangeStock = CalcChangePercent(currentStockChange, lastStockChange),

                    TotalTransfer = totalTransfer,
                    ChangeTransfer = CalcChangePercent(totalTransfer, lastTransfer)
                }
            };
        }

        public async Task<ServiceResponse> GetGoodsFlowHistogramAsync(int? month, int? year, string? warehouseId)
        {
            DateTime now = DateTime.Now;
            int selectedMonth = month ?? now.Month;
            int selectedYear = year ?? now.Year;

            DateTime fromDate = new DateTime(selectedYear, selectedMonth, 1);
            DateTime toDate = fromDate.AddMonths(1).AddDays(-1);

            var details = await _unitOfWork.GoodNoteDetailRepository.Search(
                gnd => gnd.GoodNote.CreatedTime.HasValue &&
                       gnd.GoodNote.CreatedTime.Value.Date >= fromDate.Date &&
                       gnd.GoodNote.CreatedTime.Value.Date <= toDate.Date,
                includeProperties: "GoodNote,GoodNote.GoodRequest,GoodNote.GoodRequest.RequestedWarehouse,GoodNote.GoodRequest.Warehouse"
            );

            var warehouses = string.IsNullOrEmpty(warehouseId)
                ? await _unitOfWork.WarehouseRepository.Search()
                : new List<Warehouse> { await _unitOfWork.WarehouseRepository.GetByCondition(w => w.Id == warehouseId) };

            if (warehouses == null || !warehouses.Any())
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Warehouse not found."
                };
            }

            var result = new List<object>();

            foreach (var warehouse in warehouses)
            {
                float totalPutIn = 0;
                float totalTakeOut = 0;

                //int daysInMonth = (selectedMonth == now.Month && selectedYear == now.Year)
                //    ? now.Day
                //    : DateTime.DaysInMonth(selectedYear, selectedMonth);
                int daysInMonth = DateTime.DaysInMonth(selectedYear, selectedMonth);

                var dailyRecords = new List<object>();

                for (int day = 1; day <= daysInMonth; day++)
                {
                    DateTime currentDate = new DateTime(selectedYear, selectedMonth, day);

                    var dayDetails = details.Where(d => d.GoodNote.CreatedTime.Value.Date == currentDate);

                    var putIn = dayDetails
                        .Where(d =>
                         (
                                (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                 d.GoodNote.GoodRequest.WarehouseId == warehouse.Id) ||
                            d.GoodNote.NoteType == GoodNoteEnum.Receive &&
                            d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id)
                        )
                        .Sum(d => d.Quantity);

                    var takeOut = dayDetails
                        .Where(d =>
                            d.GoodNote.NoteType == GoodNoteEnum.Issue &&
                                (d.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                                 d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id)
                        )
                        .Sum(d => d.Quantity);

                    totalPutIn += putIn;
                    totalTakeOut += takeOut;

                    dailyRecords.Add(new
                    {
                        Date = currentDate.ToString("dd/MM/yyyy"),
                        PutIn = putIn,
                        TakeOut = takeOut
                    });
                }

                result.Add(new
                {
                    WarehouseName = warehouse.Name,
                    TotalPutIn = totalPutIn,
                    TotalTakeOut = totalTakeOut,
                    DailyRecords = dailyRecords
                });
            }

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Histogram data retrieved successfully.",
                Data = result
            };
        }
        public async Task<ServiceResponse> GetStockCard(string productId, string warehouseId, DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var goodNoteDetails = await _unitOfWork.GoodNoteDetailRepository
                    .Search(d =>
                        d.Batch.ProductId == productId &&
                        (
                            // Xuất: luôn là RequestedWarehouseId
                            (d.GoodNote.NoteType == GoodNoteEnum.Issue &&
                             d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId) ||

                            // Nhập:
                            (d.GoodNote.NoteType == GoodNoteEnum.Receive &&
                             (
                                 // Nếu điều chuyển: nhập về WarehouseId
                                 (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                  d.GoodNote.GoodRequest.WarehouseId == warehouseId) ||

                                 // Còn lại: nhập về RequestedWarehouseId
                                 (d.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                                  d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId)
                             ))
                        ) &&
                        (!from.HasValue || d.GoodNote.CreatedTime.Value >= from.Value) &&
                        (!to.HasValue || d.GoodNote.CreatedTime.Value < to.Value.Date.AddDays(1)),
                        includeProperties: "GoodNote,Batch,Batch.Product,Batch.Product.Unit,GoodNote.GoodRequest,GoodNote.GoodRequest.RequestedWarehouse,GoodNote.GoodRequest.Warehouse"
                    );

                var sortedDetails = goodNoteDetails.OrderBy(d => d.GoodNote.CreatedTime).ToList();

                var product = sortedDetails.FirstOrDefault()?.Batch.Product;
                var warehouse = sortedDetails.FirstOrDefault()?.GoodNote.GoodRequest.RequestedWarehouse
                                ?? sortedDetails.FirstOrDefault()?.GoodNote.GoodRequest.Warehouse;

                float stock = 0;
                var result = new List<StockCardDetailDTO>();

                foreach (var detail in sortedDetails)
                {
                    float importQty = 0, exportQty = 0;
                    var type = detail.GoodNote.NoteType;
                    var request = detail.GoodNote.GoodRequest;

                    if (type == GoodNoteEnum.Receive &&
                        (
                            (request.RequestType == GoodRequestEnum.Transfer &&
                             request.WarehouseId == warehouseId) ||

                            (request.RequestType != GoodRequestEnum.Transfer &&
                             request.RequestedWarehouseId == warehouseId)
                        ))
                    {
                        importQty = detail.Quantity;
                    }
                    else if (type == GoodNoteEnum.Issue &&
                             request.RequestedWarehouseId == warehouseId)
                    {
                        exportQty = detail.Quantity;
                    }

                    stock += importQty - exportQty;

                    result.Add(new StockCardDetailDTO
                    {
                        Date = detail.GoodNote.Date,
                        Code = detail.GoodNote.Code,
                        Description = detail.Note,
                        Import = importQty,
                        Export = exportQty,
                        Stock = stock,
                        Note = detail.Batch.Code
                    });
                }

                return new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Thẻ kho được truy xuất thành công.",
                    Data = new StockCardDTO
                    {
                        ProductCode = product?.Sku,
                        ProductName = product?.Name,
                        UnitName = product?.Unit?.Name,
                        WarehouseName = warehouse?.Name,
                        Details = result
                    }
                };
            }
            catch (Exception)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = "Lỗi khi lấy dữ liệu thẻ kho.",
                    Data = null
                };
            }
        }

        public async Task<ServiceResponse> GetStockLineChartAsync(int? year, int? startMonth, int? endMonth, string? warehouseId)
        {
            DateTime now = DateTime.Now;
            int targetYear = year ?? now.Year;
            int fromMonth = startMonth ?? 1;
            int toMonth = endMonth ?? now.Month;
            var months = Enumerable.Range(fromMonth, toMonth - fromMonth + 1).ToList();

            var warehouses = await _unitOfWork.WarehouseRepository.Search(w => string.IsNullOrEmpty(warehouseId) || w.Id == warehouseId);

            var result = new List<object>();
            var totalByMonth = new Dictionary<float, float>();

            // Lấy toàn bộ GoodNoteDetail kèm GoodNote và GoodRequest
            var details = await _unitOfWork.GoodNoteDetailRepository.Search(
                d => d.GoodNote.CreatedTime.HasValue &&
                     d.GoodNote.CreatedTime.Value.Year == targetYear &&
                     d.GoodNote.CreatedTime.Value.Month >= fromMonth &&
                     d.GoodNote.CreatedTime.Value.Month <= toMonth,
                includeProperties: "GoodNote,GoodNote.GoodRequest"
            );

            foreach (var warehouse in warehouses)
            {
                var monthlyStocks = new List<object>();
                float? firstMonthStock = null;

                foreach (var month in months)
                {
                    var stockIn = details
                        .Where(d =>
                            d.GoodNote.NoteType == GoodNoteEnum.Receive &&
                            d.GoodNote.CreatedTime.Value.Month <= month &&
                            (
                                (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                 d.GoodNote.GoodRequest.WarehouseId == warehouse.Id) ||

                                (d.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                                 d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id)
                            )
                        )
                        .Sum(d => d.Quantity);


                    var stockOut = details
                        .Where(d =>
                            d.GoodNote.NoteType == GoodNoteEnum.Issue &&
                            d.GoodNote.CreatedTime.Value.Month <= month &&
                            (
                                (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                 d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id) ||
                                (d.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                                 d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id)
                            )
                        )
                        .Sum(d => d.Quantity);

                    var stock = stockIn - stockOut;

                    if (firstMonthStock == null)
                        firstMonthStock = stock;

                    double changePercent = firstMonthStock == 0 ? 0 :
                        ((double)(stock - firstMonthStock.Value) / firstMonthStock.Value) * 100;

                    monthlyStocks.Add(new
                    {
                        Month = month,
                        Quantity = stock,
                        //ChangePercent = Math.Round(changePercent, 2)
                    });

                    if (totalByMonth.ContainsKey(month))
                        totalByMonth[month] += stock;
                    else
                        totalByMonth[month] = stock;
                }

                result.Add(new
                {
                    Warehouse = warehouse.Name,
                    Data = monthlyStocks
                });
            }

            // Tính % tăng giảm tổng toàn hệ thống
            var totalStats = new List<object>();
            float? firstMonthTotal = totalByMonth.ContainsKey(fromMonth) ? totalByMonth[fromMonth] : 0;

            foreach (var month in months)
            {
                var total = totalByMonth.ContainsKey(month) ? totalByMonth[month] : 0;
                double totalChangePercent = firstMonthTotal == 0 ? 0 :
                    ((double)(total - firstMonthTotal.Value) / firstMonthTotal.Value) * 100;

                //totalStats.Add(new
                //{
                //    Month = month,
                //    Quantity = total,
                //    //ChangePercent = Math.Round(totalChangePercent, 2)
                //});
            }

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Stock line chart data retrieved successfully.",
                Data = new
                {
                    Warehouses = result,
                    //TotalStats = totalStats
                }
            };
        }

        public async Task<ServiceResponse> GetStockPieChartAsync()
        {
            DateTime now = DateTime.Now;
            int currentMonth = now.Month;
            int currentYear = now.Year;

            var warehouses = await _unitOfWork.WarehouseRepository.Search();
            var pieChartData = new List<object>();
            float totalStock = 0;
            float lastMonthTotalStock = 0;

            var details = await _unitOfWork.GoodNoteDetailRepository.Search(
                includeProperties: "GoodNote,GoodNote.GoodRequest"
            );

            foreach (var warehouse in warehouses)
            {
                // Tính tồn kho hiện tại cho kho đó
                var stockIn = details
                    .Where(d =>
                        (d.GoodNote.NoteType == GoodNoteEnum.Receive &&
                        (
                             (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                              d.GoodNote.GoodRequest.WarehouseId == warehouse.Id) ||
                             (d.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                              d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id)
                         ))
                    )
                    .Sum(d => d.Quantity);

                var stockOut = details
                    .Where(d =>
                        (d.GoodNote.NoteType == GoodNoteEnum.Issue &&
                         (
                             (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                              d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id) ||
                             (d.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                              d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id)
                         ))
                    )
                    .Sum(d => d.Quantity);

                var currentStock = stockIn - stockOut;
                totalStock += currentStock;

                // Tính tồn kho tới hết tháng trước
                var lastMonthStockIn = details
                    .Where(d =>
                        d.GoodNote.CreatedTime.HasValue &&
                        (d.GoodNote.CreatedTime.Value.Year < currentYear ||
                         (d.GoodNote.CreatedTime.Value.Year == currentYear && d.GoodNote.CreatedTime.Value.Month < currentMonth)) &&
                        d.GoodNote.NoteType == GoodNoteEnum.Receive &&
                        (
                            (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                             d.GoodNote.GoodRequest.WarehouseId == warehouse.Id) ||
                            (d.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                             d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id)
                        )
                    )
                    .Sum(d => d.Quantity);

                var lastMonthStockOut = details
                    .Where(d =>
                        d.GoodNote.CreatedTime.HasValue &&
                        (d.GoodNote.CreatedTime.Value.Year < currentYear ||
                         (d.GoodNote.CreatedTime.Value.Year == currentYear && d.GoodNote.CreatedTime.Value.Month < currentMonth)) &&
                        d.GoodNote.NoteType == GoodNoteEnum.Issue &&
                        (
                            (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                             d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id) ||
                            (d.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                             d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id)
                        )
                    )
                    .Sum(d => d.Quantity);

                var lastMonthStock = lastMonthStockIn - lastMonthStockOut;
                lastMonthTotalStock += lastMonthStock;

                pieChartData.Add(new
                {
                    Warehouse = warehouse.Name,
                    Quantity = currentStock
                });
            }

            // Tính % tăng giảm so với tháng trước
            double changePercent = lastMonthTotalStock == 0 ? 0 :
                ((double)(totalStock - lastMonthTotalStock) / lastMonthTotalStock) * 100;

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Pie chart data retrieved successfully.",
                Data = new
                {
                    Warehouses = pieChartData,
                    TotalStock = totalStock,
                    ChangePercent = Math.Round(changePercent, 2)
                }
            };
        }

        public async Task<ServiceResponse> GetStockPieChartByWarehouseAsync(string warehouseId)
        {
            var now = DateTime.Now;
            var details = await _unitOfWork.GoodNoteDetailRepository.Search(
                d => d.GoodNote.CreatedTime.HasValue &&
                     (d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId ||
                      (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                       d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId)) ||
                     (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                      d.GoodNote.GoodRequest.WarehouseId == warehouseId),
                includeProperties: "GoodNote,GoodNote.GoodRequest,Batch,Batch.Product,Batch.Product.ProductType,Batch.Product.ProductType.Category"
            );

            var grouped = details.GroupBy(d => d.Batch.Product.ProductType.CategoryId).Select(g =>
            {
                var stockIn = g.Where(d => d.GoodNote.NoteType == GoodNoteEnum.Receive &&
                                            (
                                                (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                                 d.GoodNote.GoodRequest.WarehouseId == warehouseId) ||
                                                (d.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                                                 d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId)
                                            )).Sum(d => d.Quantity);

                var stockOut = g.Where(d => d.GoodNote.NoteType == GoodNoteEnum.Issue &&
                                            (
                                                (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                                 d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId) ||
                                                (d.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                                                 d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId)
                                            ))
                                .Sum(d => d.Quantity);

                return new
                {
                    Caregory = g.First().Batch.Product.ProductType.Category.Name,
                    Quantity = stockIn - stockOut
                };
            }).Where(x => x.Quantity > 0).ToList();

            var total = grouped.Sum(x => x.Quantity);

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Pie chart data retrieved for warehouse.",
                Data = new
                {
                    WarehouseId = warehouseId,
                    TotalStock = total,
                    Category = grouped
                }
            };
        }
        public async Task<ServiceResponse> GetStockBookAsync(string warehouseId, int month, int year, string userIds)
        {
            try
            {
                // 1. Lấy thông tin kho
                var warehouse = await _unitOfWork.WarehouseRepository.GetByCondition(w => w.Id == warehouseId, includeProperties: "AccountWarehouses,AccountWarehouses.Account,AccountWarehouses.Account.Profile");
                if (warehouse == null)
                    return new ServiceResponse { Status = SRStatus.Error, Message = "Không tìm thấy kho." };

                var profile = await _unitOfWork.ProfileRepository.GetByCondition(p => p.AccountId == userIds);
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1);

                // 2. Lấy tất cả các GoodNoteDetail liên quan đến kho này (cả nhập và xuất)
                var allDetails = await _unitOfWork.GoodNoteDetailRepository.Search(
                    d =>
                        d.GoodNote.CreatedTime >= startDate.AddMonths(-1) &&
                        d.GoodNote.CreatedTime < endDate &&
                        (
                            // Nhập vào RequestedWarehouse
                            (d.GoodNote.NoteType == GoodNoteEnum.Receive && (
                                    (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                     d.GoodNote.GoodRequest.WarehouseId == warehouseId) ||
                                    (d.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                                     d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId)
                                )) ||
                            // Xuất: điều chuyển từ WarehouseId
                            (d.GoodNote.NoteType == GoodNoteEnum.Issue &&
                                (
                                    (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                     d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId) ||
                                    (d.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                                     d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId)
                                ))
                        ),
                    includeProperties: "GoodNote,Batch,Batch.Product,GoodNote.GoodRequest"
                );

                var sortedDetails = allDetails.OrderBy(d => d.GoodNote.CreatedTime).ThenBy(d => d.GoodNote.Code).ToList();

                var result = new List<object>();
                var stockMap = new Dictionary<string, float>(); // batchId -> tồn

                foreach (var detail in sortedDetails)
                {
                    var date = detail.GoodNote.Date?.ToString("dd/MM/yyyy") ?? "";
                    var code = detail.GoodNote.Code;
                    var description = detail.Note;
                    var batch = detail.Batch;
                    var product = batch.Product;
                    var batchId = batch.Id;

                    float import = 0, export = 0;
                    float prevStock = stockMap.ContainsKey(batchId) ? stockMap[batchId] : 0;

                    if (detail.GoodNote.NoteType == GoodNoteEnum.Receive &&
                             (
                                 (detail.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                  detail.GoodNote.GoodRequest.WarehouseId == warehouseId) ||
                                 (detail.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                                  detail.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId)
                             ))
                    {
                        import = detail.Quantity;
                    }
                    else if (detail.GoodNote.NoteType == GoodNoteEnum.Issue &&
                             (
                                 (detail.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                  detail.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId) ||
                                 (detail.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                                  detail.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId)
                             ))
                    {
                        export = detail.Quantity;
                    }

                    float currentStock = prevStock + import - export;
                    stockMap[batchId] = currentStock;

                    // Chỉ lấy dữ liệu trong tháng yêu cầu
                    if (detail.GoodNote.Date >= startDate && detail.GoodNote.Date < endDate)
                    {
                        result.Add(new
                        {
                            Date = date,
                            Code = code,
                            Description = description,
                            Import = import,
                            Export = export,
                            OpeningStock = prevStock,
                            ClosingStock = currentStock,
                            ProductName = product?.Name,
                            Sku = product?.Sku,
                            BatchCode = batch?.Code,
                            //Note = $"Lô: {batch.Code} - {product?.Name}"
                        });
                    }
                }

                return new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = $"Sổ kho tháng {month:00}/{year} đã được lấy thành công.",
                    Data = new
                    {
                        WarehouseName = warehouse.Name,
                        Address = warehouse.Address,
                        InCharge = profile.LastName +" "+profile.FirstName,
                        Details = result
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = "Lỗi khi truy xuất sổ kho.",
                    Data = ex.Message
                };
            }
        }
        public async Task<ServiceResponse> GetAvailableProductsInWarehouse(string warehouseId)
        {
            if (string.IsNullOrEmpty(warehouseId))
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = "WarehouseId is required."
                };
            }

            var inventories = await _unitOfWork.InventoryRepository.Search(
                i => i.WarehouseId == warehouseId /*&& i.CurrentQuantity > 0*/,
                includeProperties: "Batch,Batch.Product,Batch.Product.Unit," +
                                    "Batch.Product.Brand," +
                                    "Batch.Product.ProductType," +
                                    "Batch.Product.ProductType.Category"
                                );

            if (inventories == null || !inventories.Any())
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "No available products found in the warehouse.",
                    Data = warehouseId
                };
            }
            var grouped = inventories
                .GroupBy(i => i.Batch.ProductId)
                .Select(g =>
                {
                    var firstProduct = g.First().Batch.Product;
                    return new ProductWithQuantityDTO
                    {
                        Id = g.Key,
                        Name = firstProduct.Name,
                        imageUrl = firstProduct.imageUrl,
                        Sku = firstProduct.Sku,
                        IsBatchManaged = firstProduct.IsBatchManaged,
                        ProductTypeName = firstProduct.ProductType.Name, 
                        CategoryName = firstProduct.ProductType.Category.Name,
                        UnitName = firstProduct.Unit?.Name,
                        UnitType = firstProduct.Unit.Type,
                        BrandName = firstProduct.Brand?.Name,
                        TotalQuantity = g.Sum(i => i.CurrentQuantity)
                    };
                })
                .ToList();

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Available products retrieved successfully.",
                Data = grouped
            };
        }
        //Get all staffs of warehouse
        public async Task<ServiceResponse> GetStaffsOfWarehouse(string warehouseId, int? pageIndex = null, int? pageSize = null,
                                                                     string? keyword = null)
        {
            var staffs = await _unitOfWork.AccountRepository.Search(
                a => a.AccountWarehouses.Any(aw => aw.WarehouseId == warehouseId)
                && a.AccountGroups.Any(ag => ag.Group.Name == "Nhân viên kho")
                && (string.IsNullOrEmpty(keyword) || a.Profile.FirstName.Contains(keyword) || a.Profile.LastName.Contains(keyword)),
                includeProperties: "AccountWarehouses,AccountGroups,AccountGroups.Group,Profile",
                pageSize: pageSize,
                pageIndex: pageIndex
            );

            if (staffs == null || !staffs.Any())
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Không tìm thấy nhân viên nào trong kho.",
                    Data = warehouseId
                };
            }

            var staffDtos = _mapper.Map<IEnumerable<AccountDTO>>(staffs);

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Lấy danh sách nhân viên thành công.",
                Data = staffDtos
            };
        }
    }
}