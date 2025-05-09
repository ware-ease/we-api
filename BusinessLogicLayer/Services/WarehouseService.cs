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

        //public async Task<ServiceResponse> GetWarehouseStatisticsAsync(string? warehouseId)
        //{
        //    // Tính ngày bắt đầu và kết thúc của tháng hiện tại và tháng trước
        //    var startOfMonth = new DateTime(year: DateTime.Now.Year, month: DateTime.Now.Month, 1);
        //    var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);
        //    var startOfLastMonth = startOfMonth.AddMonths(-1);
        //    var endOfLastMonth = startOfMonth.AddTicks(-1);

        //    // Lấy tất cả các GoodNoteDetail có liên kết tới kho và nằm trong 2 khoảng thời gian
        //    var allDetails = await _unitOfWork.GoodNoteDetailRepository.Search(
        //        filter: d => (string.IsNullOrEmpty(warehouseId) || d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId) &&
        //                     d.GoodNote.Date >= startOfLastMonth &&
        //                     d.GoodNote.Date <= endOfMonth,
        //        includeProperties: "GoodNote,GoodNote.GoodRequest"
        //    );

        //    // Phân loại theo tháng
        //    var currentMonthDetails = allDetails.Where(d =>
        //        d.GoodNote.Date >= startOfMonth &&
        //        d.GoodNote.Date <= endOfMonth);

        //    var lastMonthDetails = allDetails.Where(d =>
        //        d.GoodNote.Date >= startOfLastMonth &&
        //        d.GoodNote.Date <= endOfLastMonth);

        //    // Hàm tính tổng số lượng theo loại phiếu
        //    int SumByType(IEnumerable<GoodNoteDetail> details, GoodNoteEnum type) =>
        //        (int)details.Where(d => d.GoodNote.NoteType == type).Sum(d => d.Quantity);
        //    int SumByTypev2(IEnumerable<GoodNoteDetail> details, GoodRequestEnum type) =>
        //      (int)details.Where(d => d.GoodNote.GoodRequest.RequestType == type).Sum(d => d.Quantity);
        //    // Tính tổng từng loại trong tháng hiện tại
        //    int totalPutIn = SumByType(currentMonthDetails, GoodNoteEnum.Receive);
        //    int totalTakeOut = SumByType(currentMonthDetails, GoodNoteEnum.Issue);
        //    int totalTransfer = SumByTypev2(currentMonthDetails, GoodRequestEnum.Transfer);
        //    int currentStockChange = totalPutIn - totalTakeOut;

        //    // Tháng trước
        //    int lastPutIn = SumByType(lastMonthDetails, GoodNoteEnum.Receive) /*+ SumByType(lastMonthDetails, GoodNoteEnum.Return)*/;
        //    int lastTakeOut = SumByType(lastMonthDetails, GoodNoteEnum.Issue);
        //    int lastTransfer = SumByTypev2(lastMonthDetails, GoodRequestEnum.Transfer);
        //    int lastStockChange = lastPutIn - lastTakeOut;

        //    // Hàm tính phần trăm thay đổi
        //    double CalcChangePercent(int current, int previous)
        //    {
        //        if (previous == 0) return current == 0 ? 0 : 100;
        //        return Math.Round(((double)(current - previous) / previous) * 100, 2);
        //    }

        //    return new ServiceResponse
        //    {
        //        Status = SRStatus.Success,
        //        Message = "4 Cards statistics generated successfully.",
        //        Data = new
        //        {
        //            TotalPutIn = totalPutIn,
        //            ChangePutIn = CalcChangePercent(totalPutIn, lastPutIn),

        //            TotalTakeOut = totalTakeOut,
        //            ChangeTakeOut = CalcChangePercent(totalTakeOut, lastTakeOut),

        //            CurrentStockChange = currentStockChange,
        //            ChangeStock = CalcChangePercent(currentStockChange, lastStockChange),

        //            TotalTransfer = totalTransfer,
        //            ChangeTransfer = CalcChangePercent(totalTransfer, lastTransfer)
        //        }
        //    };
        //}

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

            // Chuyển kho tính theo phiếu Issue thôi để tránh double
            float SumTransfer(IEnumerable<GoodNoteDetail> details) =>
                details
                    .Where(d => d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer
                                && d.GoodNote.NoteType == GoodNoteEnum.Issue)
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
                            d.GoodNote.NoteType == GoodNoteEnum.Receive &&
                            d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id
                        )
                        .Sum(d => d.Quantity);

                    var takeOut = dayDetails
                        .Where(d =>
                            d.GoodNote.NoteType == GoodNoteEnum.Issue &&
                            (
                                (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                 d.GoodNote.GoodRequest.WarehouseId == warehouse.Id) ||
                                (d.GoodNote.GoodRequest.RequestType != GoodRequestEnum.Transfer &&
                                 d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id)
                            )
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
                            // Nhập: luôn về RequestedWarehouse
                            (d.GoodNote.NoteType == GoodNoteEnum.Receive &&
                             d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId) ||

                            // Xuất: tùy theo loại phiếu
                            (d.GoodNote.NoteType == GoodNoteEnum.Issue &&
                             (
                                 // Nếu là điều chuyển: kho xuất là WarehouseId
                                 (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                  d.GoodNote.GoodRequest.WarehouseId == warehouseId) ||

                                 // Nếu là xuất thường: kho vẫn là RequestedWarehouseId
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
                        request.RequestedWarehouseId == warehouseId)
                    {
                        importQty = detail.Quantity;
                    }
                    else if (type == GoodNoteEnum.Issue &&
                             (
                                 (request.RequestType == GoodRequestEnum.Transfer &&
                                  request.WarehouseId == warehouseId) ||

                                 (request.RequestType != GoodRequestEnum.Transfer &&
                                  request.RequestedWarehouseId == warehouseId)
                             ))
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
                                d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id ||
                                (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
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
                                 d.GoodNote.GoodRequest.WarehouseId == warehouse.Id) ||
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
                             d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id ||
                             (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                              d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id)
                         ))
                    )
                    .Sum(d => d.Quantity);

                var stockOut = details
                    .Where(d =>
                        (d.GoodNote.NoteType == GoodNoteEnum.Issue &&
                         (
                             (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                              d.GoodNote.GoodRequest.WarehouseId == warehouse.Id) ||
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
                            d.GoodNote.GoodRequest.RequestedWarehouseId == warehouse.Id ||
                            (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
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
                             d.GoodNote.GoodRequest.WarehouseId == warehouse.Id) ||
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
                                           d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId)
                               .Sum(d => d.Quantity);

                var stockOut = g.Where(d => d.GoodNote.NoteType == GoodNoteEnum.Issue &&
                                            (
                                                (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                                 d.GoodNote.GoodRequest.WarehouseId == warehouseId) ||
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
        public async Task<ServiceResponse> GetStockBookAsync(string warehouseId, int month, int year)
        {
            try
            {
                // 1. Lấy thông tin kho
                var warehouse = await _unitOfWork.WarehouseRepository.GetByCondition(w => w.Id == warehouseId, includeProperties: "AccountWarehouses,AccountWarehouses.Account,AccountWarehouses.Account.Profile");
                if (warehouse == null)
                    return new ServiceResponse { Status = SRStatus.Error, Message = "Không tìm thấy kho." };

                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1);

                // 2. Lấy tất cả các GoodNoteDetail liên quan đến kho này (cả nhập và xuất)
                var allDetails = await _unitOfWork.GoodNoteDetailRepository.Search(
                    d =>
                        d.GoodNote.CreatedTime >= startDate.AddMonths(-1) &&
                        d.GoodNote.CreatedTime < endDate &&
                        (
                            // Nhập vào RequestedWarehouse
                            (d.GoodNote.NoteType == GoodNoteEnum.Receive && d.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId) ||
                            // Xuất: điều chuyển từ WarehouseId
                            (d.GoodNote.NoteType == GoodNoteEnum.Issue &&
                                (
                                    (d.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                     d.GoodNote.GoodRequest.WarehouseId == warehouseId) ||
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
                        detail.GoodNote.GoodRequest.RequestedWarehouseId == warehouseId)
                    {
                        import = detail.Quantity;
                    }
                    else if (detail.GoodNote.NoteType == GoodNoteEnum.Issue &&
                             (
                                 (detail.GoodNote.GoodRequest.RequestType == GoodRequestEnum.Transfer &&
                                  detail.GoodNote.GoodRequest.WarehouseId == warehouseId) ||
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
                            Note = $"Lô: {batch.Code} - {product?.Name}"
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
                        InCharge = warehouse.AccountWarehouses.FirstOrDefault().Account.Profile.LastName +" "+ warehouse.AccountWarehouses.FirstOrDefault().Account.Profile.FirstName,
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

    }
}