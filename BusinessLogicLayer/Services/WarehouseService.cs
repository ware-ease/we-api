using AutoMapper;
using Azure;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Enum;
using Data.Model.DTO.Base;
using Data.Model.Request.Area;
using Data.Model.Request.Inventory;
using Data.Model.Request.InventoryLocation;
using Data.Model.Request.LocationLog;
using Data.Model.Request.Warehouse;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Tls;
using System;
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
                Message = "Get successfully!",
                Data = mappedResults
            };
        }

        public async Task<ServiceResponse> GetFullWarehouseInfo<TResult>(string id)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByCondition(
                w => w.Id == id,
                includeProperties: "Locations"
            );

            if (warehouse == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Warehouse not found!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(warehouse);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get full warehouse info successfully!",
                Data = result
            };
        }
        public async Task<ServiceResponse> CreateStructureAsync(CreateWarehouseStructureRequest request)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByCondition(x => x.Id == request.Id!);

            if (warehouse == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Warehouse not found",
                    Data = request.Id
                };
            }

            try
            {
                foreach (var locationDto in request.Locations)
                {
                    // 🔥 Kiểm tra nếu location có ParentId thì phải tồn tại trong DB
                    if (!string.IsNullOrWhiteSpace(locationDto.ParentId))
                    {
                        var parentExists = await _unitOfWork.LocationRepository.GetByCondition(l => l.Id == locationDto.ParentId);
                        if (parentExists == null)
                        {
                            return new ServiceResponse
                            {
                                Status = Data.Enum.SRStatus.Error,
                                Message = $"Parent location with ID '{locationDto.ParentId}' not found.",
                                Data = locationDto.ParentId
                            };
                        }
                        if (parentExists.Level == 4)
                        {
                            return new ServiceResponse
                            {
                                Status = Data.Enum.SRStatus.Error,
                                Message = $"The maximum level of a Location is 4. The parent location with ID '{locationDto.ParentId}' has already reached this limit.",
                                Data = locationDto.ParentId
                            };
                        }
                        locationDto.Level = parentExists.Level + 1; // Cấp độ của location mới là cấp độ của parent + 1
                    }
                    var location = _mapper.Map<Location>(locationDto);
                    location.WarehouseId = request.Id!;
                    location.Warehouse = warehouse;
                    // 🔥 Kiểm tra ParentId: nếu là "" thì chuyển thành null
                    location.ParentId = string.IsNullOrWhiteSpace(locationDto.ParentId) ? null : locationDto.ParentId;
                    await _unitOfWork.LocationRepository.Add(location);

                }

                await _unitOfWork.SaveAsync();
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Warehouse structure created successfully",
                    Data = warehouse.Id
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Error,
                    Message = $"Error creating warehouse structure: {ex.Message}"
                };
            }
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
                    Message = "Update successfully!",
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

        public async Task<ServiceResponse> SearchWarehouses<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                     string? keyword = null, float? minArea = null, float? maxArea = null)
        {
            Expression<Func<Warehouse, bool>> filter = w =>
                (string.IsNullOrEmpty(keyword) || w.Name.Contains(keyword)) &&
                (!minArea.HasValue || w.Area >= minArea) &&
                (!maxArea.HasValue || w.Area <= maxArea);

            var totalRecords = await _warehouseRepository.Count(filter);

            var results = await _warehouseRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize);

            var mappedResults = _mapper.Map<IEnumerable<TResult>>(results);

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
                    Message = "Warehouse not found.",
                    Data = warehouseId
                };
            }
            var result = _mapper.Map<WarehouseInventoryDTO>(warehouse);
            result.Inventories = _mapper.Map<IEnumerable<InventoryDTO>>(warehouse.Inventories);

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Inventory retrieved successfully.",
                Data = result
            };
        }

        public async Task<ServiceResponse> InventoryLocationInOutAsync(CreateInventoryLocationDTO request)
        {
            // Lấy thông tin inventory và location từ request
            var inventory = await _unitOfWork.InventoryRepository.GetByCondition(x => x.Id == request.InventoryId);
            var location = await _unitOfWork.LocationRepository.GetByCondition(x => x.Id == request.LocationId);

            // Kiểm tra nếu inventory hoặc location không tồn tại
            if (inventory == null || location == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Inventory or Location not found.",
                    Data = request
                };
            }

            // Kiểm tra số lượng dương (put-away) hay âm (take-out)
            if (request.Quantity == 0)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = "Quantity must be greater than 0 or less than 0."
                };
            }

            if (request.Quantity > 0)
            {
                // PUT-AWAY: Thêm vào
                // Lấy tất cả các InventoryLocation với InventoryId tương ứng
                var inventoryLocations = await _unitOfWork.InventoryLocationRepository.Search(
                    x => x.InventoryId == request.InventoryId);

                // Tính tổng số lượng từ tất cả các InventoryLocation bằng cách cộng dồn
                int totalQuantityInLocations = 0;
                foreach (var location1 in inventoryLocations)
                {
                    totalQuantityInLocations += location1.Quantity;
                }

                // Kiểm tra xem tổng số lượng trong các InventoryLocation cộng với số lượng mới có vượt quá số lượng trong Inventory không
                if (totalQuantityInLocations + request.Quantity > inventory.CurrentQuantity)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Not enough stock in inventory to put away.",
                        Data = request
                    };
                }

                // Kiểm tra nếu đã tồn tại bản ghi trong InventoryLocation
                var existing = await _unitOfWork.InventoryLocationRepository.GetByCondition(
                    x => x.InventoryId == request.InventoryId && x.LocationId == request.LocationId);

                if (existing != null)
                {
                    // Cập nhật số lượng mới cho InventoryLocation
                    existing.Quantity += request.Quantity;
                    _unitOfWork.InventoryLocationRepository.Update(existing);

                    // Thêm bản ghi vào bảng LocationLog để ghi nhận sự thay đổi
                    var locationLog = new LocationLog
                    {
                        InventoryLocationId = existing.Id,  // Lưu ID của InventoryLocation vào LocationLog
                        NewQuantity = existing.Quantity,    // Số lượng mới sau khi thay đổi
                        ChangeInQuantity = request.Quantity, // Sự thay đổi trong số lượng
                        Note = request.Note // Ghi chú từ request
                    };
                    await _unitOfWork.LocationLogRepository.Add(locationLog);
                    inventory.ArrangedQuantity += request.Quantity;
                    inventory.NotArrgangedQuantity -= request.Quantity;
                    _unitOfWork.InventoryRepository.Update(inventory);
                }
                else
                {
                    // Nếu chưa có bản ghi InventoryLocation, tạo mới
                    var newEntry = new InventoryLocation
                    {
                        InventoryId = request.InventoryId,
                        LocationId = request.LocationId,
                        Quantity = request.Quantity,
                    };

                    // Lưu bản ghi mới vào InventoryLocation
                    await _unitOfWork.InventoryLocationRepository.Add(newEntry);

                    // Thêm bản ghi vào LocationLog cho trường hợp mới
                    var locationLog = new LocationLog
                    {
                        InventoryLocationId = newEntry.Id,  // ID của InventoryLocation mới
                        NewQuantity = newEntry.Quantity,    // Số lượng sau khi thêm vào
                        ChangeInQuantity = newEntry.Quantity, // Sự thay đổi là số lượng ban đầu                      
                        Note = request.Note // Ghi chú từ request
                    };
                    await _unitOfWork.LocationLogRepository.Add(locationLog);
                    inventory.ArrangedQuantity += request.Quantity;
                    inventory.NotArrgangedQuantity -= request.Quantity;
                    _unitOfWork.InventoryRepository.Update(inventory);
                }
            }
            else
            {
                // TAKE-OUT: Lấy ra
                // Kiểm tra số lượng trong InventoryLocation và đảm bảo đủ hàng để lấy ra
                var existingLocation = await _unitOfWork.InventoryLocationRepository.GetByCondition(
                    x => x.InventoryId == request.InventoryId && x.LocationId == request.LocationId);

                if (existingLocation == null || existingLocation.Quantity < Math.Abs(request.Quantity))
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Not enough stock in location to take out.",
                        Data = request
                    };
                }

                // Cập nhật số lượng trong InventoryLocation
                existingLocation.Quantity += request.Quantity;
                _unitOfWork.InventoryLocationRepository.Update(existingLocation);

                // Thêm bản ghi vào bảng LocationLog để ghi nhận sự thay đổi
                var locationLog = new LocationLog
                {
                    InventoryLocationId = existingLocation.Id,
                    NewQuantity = existingLocation.Quantity,
                    ChangeInQuantity = request.Quantity, // Sự thay đổi trong số lượng
                    Note = request.Note // Ghi chú từ request
                };
                await _unitOfWork.LocationLogRepository.Add(locationLog);
                inventory.ArrangedQuantity += request.Quantity;
                inventory.NotArrgangedQuantity -= request.Quantity;
                _unitOfWork.InventoryRepository.Update(inventory);
            }

            // Lưu tất cả thay đổi vào database
            await _unitOfWork.SaveAsync();

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Inventory Location operation completed successfully.",
                Data = request.InventoryId
            };
        }

        public async Task<ServiceResponse> GetInventoriesInLocation(string locationId)
        {
            var location = await _unitOfWork.LocationRepository.GetByCondition(
                l => l.Id == locationId,
                includeProperties: "InventoryLocations," +
                                   "InventoryLocations.Inventory," +
                                   "InventoryLocations.Inventory.Warehouse," +
                                   "InventoryLocations.Inventory.Batch," +
                                   "InventoryLocations.Inventory.Batch.Product," +
                                   "InventoryLocations.Inventory.Batch.Product.Unit," +
                                   "InventoryLocations.Inventory.Batch.Product.Brand"
            );

            if (location == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Location not found.",
                    Data = locationId
                };
            }

            var inventoryItems = _mapper.Map<LocationInventoryDTO>(location);

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Inventories retrieved successfully.",
                Data = inventoryItems
            };

        }
        public async Task<ServiceResponse> GetLocationLogsAsync(string warehouseId, string? locationId, int pageIndex, int pageSize)
        {
            // Khởi tạo filter với điều kiện warehouseId và locationId (nếu có)
            Expression<Func<LocationLog, bool>> filter = log =>
                log.InventoryLocation.Location.WarehouseId == warehouseId &&
                (string.IsNullOrEmpty(locationId) || log.InventoryLocation.LocationId == locationId);

            // Gọi hàm search đã có sẵn từ repository với phân trang và lọc
            var totalRecords = await _unitOfWork.LocationLogRepository.Count(filter);
            var logs = await _unitOfWork.LocationLogRepository.Search(
                filter: filter,
                orderBy: l => l.OrderByDescending(x => x.CreatedTime),
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "InventoryLocation,InventoryLocation.Location,InventoryLocation.Location.Warehouse"
            );

            // Nếu không có log nào
            if (logs == null || !logs.Any())
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "No logs found.",
                    Data = null
                };
            }

            // Map logs sang DTO nếu cần
            var logsDto = _mapper.Map<IEnumerable<LocationLogDTO>>(logs);

            // Tính số trang
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Location logs retrieved successfully.",
                Data = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Records = logsDto
                }
            };
        }

        public async Task<ServiceResponse> GetWarehouseStatisticsAsync(string? warehouseId)
        {
            // Tính ngày bắt đầu và kết thúc của tháng hiện tại và tháng trước
            var startOfMonth = new DateTime(year: DateTime.Now.Year, month: DateTime.Now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);
            var startOfLastMonth = startOfMonth.AddMonths(-1);
            var endOfLastMonth = startOfMonth.AddTicks(-1);

            // Lấy tất cả các GoodNoteDetail có liên kết tới kho và nằm trong 2 khoảng thời gian
            var allDetails = await _unitOfWork.GoodNoteDetailRepository.Search(
                filter: d => /*d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId &&*/
                             d.GoodNote.CreatedTime >= startOfLastMonth &&
                             d.GoodNote.CreatedTime <= endOfMonth,
                includeProperties: "GoodNote"
            );

            // Phân loại theo tháng
            var currentMonthDetails = allDetails.Where(d =>
                d.GoodNote.CreatedTime >= startOfMonth &&
                d.GoodNote.CreatedTime <= endOfMonth);

            var lastMonthDetails = allDetails.Where(d =>
                d.GoodNote.CreatedTime >= startOfLastMonth &&
                d.GoodNote.CreatedTime <= endOfLastMonth);

            // Hàm tính tổng số lượng theo loại phiếu
            int SumByType(IEnumerable<GoodNoteDetail> details, GoodNoteEnum type) =>
                (int)details.Where(d => d.GoodNote.NoteType == type).Sum(d => d.Quantity);

            // Tính tổng từng loại trong tháng hiện tại
            int totalPutIn = SumByType(currentMonthDetails, GoodNoteEnum.Receive) + SumByType(currentMonthDetails, GoodNoteEnum.Return);
            int totalTakeOut = SumByType(currentMonthDetails, GoodNoteEnum.Issue);
            int totalTransfer = SumByType(currentMonthDetails, GoodNoteEnum.Transfer);
            int currentStockChange = totalPutIn - totalTakeOut;

            // Tháng trước
            int lastPutIn = SumByType(lastMonthDetails, GoodNoteEnum.Receive) + SumByType(lastMonthDetails, GoodNoteEnum.Return);
            int lastTakeOut = SumByType(lastMonthDetails, GoodNoteEnum.Issue);
            int lastTransfer = SumByType(lastMonthDetails, GoodNoteEnum.Transfer);
            int lastStockChange = lastPutIn - lastTakeOut;

            // Hàm tính phần trăm thay đổi
            double CalcChangePercent(int current, int previous)
            {
                if (previous == 0) return current == 0 ? 0 : 100;
                return Math.Round(((double)(current - previous) / previous) * 100, 2);
            }

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "4 Cards statistics generated successfully.",
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
        public async Task<ServiceResponse> GetGoodsFlowHistogramAsync(string? warehouseId)
        {
            // Mặc định khoảng thời gian là từ đầu đến cuối tháng hiện tại
            DateTime now = DateTime.Now;
            DateTime fromDate = new DateTime(now.Year, now.Month, 1);
            DateTime toDate = fromDate.AddMonths(1).AddDays(-1);

            var details = await _unitOfWork.GoodNoteDetailRepository.Search(
                gnd =>
                    gnd.GoodNote.CreatedTime.HasValue &&
                    gnd.GoodNote.CreatedTime.Value.Date >= fromDate.Date &&
                    gnd.GoodNote.CreatedTime.Value.Date <= toDate.Date &&
                    (string.IsNullOrEmpty(warehouseId) || gnd.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId),
                includeProperties: "GoodNote,GoodNote.GoodRequest"
            );

            var groupedData = details
                .GroupBy(d => d.GoodNote.CreatedTime.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    PutIn = g.Where(x => x.GoodNote.NoteType == GoodNoteEnum.Receive || x.GoodNote.NoteType == GoodNoteEnum.Return).Sum(x => x.Quantity),
                    TakeOut = g.Where(x => x.GoodNote.NoteType == GoodNoteEnum.Issue /*|| x.GoodNote.NoteType == GoodNoteEnum.Transfer*/).Sum(x => x.Quantity)
                })
                .OrderBy(x => x.Date)
                .ToList();
            // Tính tổng nhập và tổng xuất cả tháng
            int totalPutIn = (int)groupedData.Sum(x => x.PutIn);
            int totalTakeOut = (int)groupedData.Sum(x => x.TakeOut);
            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Histogram data retrieved successfully.",
                Data = new
                {
                    TotalPutIn = totalPutIn,
                    TotalTakeOut = totalTakeOut,
                    Records = groupedData.Select(d => new
                    {
                        Date = d.Date.ToString("dd/MM/yyyy"),
                        PutIn = d.PutIn,
                        TakeOut = d.TakeOut
                    })
                }
            };
        }
        public async Task<ServiceResponse> GetStockCard(string productId, string warehouseId, DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var goodNoteDetails = await _unitOfWork.GoodNoteDetailRepository
                    .Search(d => d.Batch.ProductId == productId &&
                                      d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId &&
                                      (!from.HasValue || d.GoodNote.CreatedTime.Value >= from.Value) &&
                                      (!to.HasValue || d.GoodNote.CreatedTime.Value < to.Value.Date.AddDays(1)),
                                 includeProperties: "GoodNote,Batch,Batch.Product,Batch.Product.Unit,GoodNote.GoodRequest.RequestedWarehouse");

                var sortedDetails = goodNoteDetails.OrderBy(d => d.GoodNote.Date).ToList();

                var product = sortedDetails.FirstOrDefault()?.Batch.Product;
                var warehouse = sortedDetails.FirstOrDefault()?.GoodNote?.GoodRequest.RequestedWarehouse;

                float stock = 0;
                var result = new List<StockCardDetailDTO>();

                foreach (var detail in sortedDetails)
                {
                    var type = detail.GoodNote.NoteType;
                    float importQty = 0, exportQty = 0;

                    if (type == GoodNoteEnum.Receive || type == GoodNoteEnum.Return)
                        importQty = detail.Quantity;
                    else if (type == GoodNoteEnum.Issue || type == GoodNoteEnum.Transfer)
                        exportQty = detail.Quantity;

                    stock += importQty - exportQty;

                    result.Add(new StockCardDetailDTO
                    {
                        Date = detail.GoodNote.CreatedTime,
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
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = "Lỗi khi lấy dữ liệu thẻ kho.",
                    Data = null
                };
            }
        }

    }
}