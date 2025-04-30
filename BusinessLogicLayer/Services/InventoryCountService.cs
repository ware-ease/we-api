using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.InventoryCount;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class InventoryCountService : GenericService<InventoryCount>, IInventoryCountService
    {
        private readonly IGenericRepository<Schedule> _scheduleRepository;
        private readonly IGenericRepository<Location> _locationRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<InventoryCountDetail> _inventoryCountDetailRepository;
        private readonly IGenericRepository<Inventory> _inventoryRepository;
        private readonly IGenericRepository<InventoryLocation> _inventoryLocationRepository;
        private readonly IGenericRepository<Batch> _batchRepository;
        public InventoryCountService(IGenericRepository<InventoryCount> genericRepository,
            IGenericRepository<Schedule> scheduleRepository,
            IGenericRepository<Location> locationRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<Inventory> inventoryRepository,
            IGenericRepository<InventoryLocation> inventoryLocationRepository,
            IGenericRepository<Batch> batchRepository,
            IGenericRepository<InventoryCountDetail> inventoryCountDetailRepository,
            IMapper mapper, IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _locationRepository = locationRepository;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
            _inventoryLocationRepository = inventoryLocationRepository;
            _batchRepository = batchRepository;
            _inventoryCountDetailRepository = inventoryCountDetailRepository;
        }


        public override async Task<ServiceResponse> Get<TResult>(string id)
        {
            var inventoryCount = await _genericRepository.GetByCondition(
                p => p.Id == id,
                includeProperties: "InventoryCheckDetails"
            );

            if (inventoryCount == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "InventoryCount not found!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(inventoryCount);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = result
            };
        }

        public async Task<InventoryCountDTO> AddInventoryCount(InventoryCountCreateDTO request)
        {
            if (request.Date > DateOnly.FromDateTime(DateTime.Now))
                throw new Exception("Date không được ở tương lai");

            if (request.EndTime < request.StartTime)
                throw new Exception("EndTime không được ở trước StartTime");

            /*var schedule = await _scheduleRepository.GetByCondition(p => p.Id == request.ScheduleId);
            if (schedule == null)
                throw new Exception("Schedule không tồn tại");
            var existedSchedule = await _genericRepository.GetByCondition(p => p.ScheduleId == request.ScheduleId);
            if (existedSchedule != null)
                throw new Exception("Schedule này đã có phiếu kiểm kê");

            var location = await _locationRepository.GetByCondition(p => p.Id == request.LocationId);
            if (location != null)
            {
                if (location.Level != 0)
                    throw new Exception("Level phải bằng 0");
            }
            else
                throw new Exception("Location không tồn tại");*/
            /*if (location == null)
                throw new Exception("Location không tồn tại");*/

            /*var product = await _productRepository.GetByCondition(p => p.Id == request.InventoryCountDetailCreateDTO.ProductId);
            if (location == null)
                throw new Exception("Product không tồn tại");*/

            var inventoryCount = _mapper.Map<InventoryCount>(request);
            //inventoryCount.LocationId = schedule.LocationId;
            //inventoryCount.LocationId = request.LocationId;

            await _genericRepository.Insert(inventoryCount);
            await _unitOfWork.SaveAsync();

            if (request.InventoryCountDetails != null && request.InventoryCountDetails.Any())
            {
                var inventoryCountDetails = new List<InventoryCountDetail>();

                foreach (var detail in request.InventoryCountDetails)
                {
                    var inventory = await _inventoryRepository.GetByCondition(p => p.Id == detail.InventoryId);
                    if (inventory == null)
                        throw new Exception($"Inventory với ID {detail.InventoryId} không tồn tại");

                    //var expectedQuantity = await SumInventoryLocationQuantityByLocationLevel0AndInventory(inventoryCount.LocationId, detail.InventoryId);
                    var expectedQuantity = inventory.CurrentQuantity;
                    switch(detail.CountedQuantity)
                    {
                        case var c when c < expectedQuantity:
                            detail.Status = InventoryCountDetailStatus.Understock;
                            break;

                        case var c when c > expectedQuantity:
                            detail.Status = InventoryCountDetailStatus.Overstock;
                            break;

                        case var c when c == expectedQuantity:
                            detail.Status = InventoryCountDetailStatus.Balanced;
                            break;
                    }

                    var inventoryCountDetail = _mapper.Map<InventoryCountDetail>(detail);
                    inventoryCountDetail.InventoryCountId = inventoryCount.Id;
                    //inventoryCountDetail.ExpectedQuantity = expectedQuantity;
                    inventoryCountDetail.CreatedBy = inventoryCount.CreatedBy;

                    await _inventoryCountDetailRepository.Insert(inventoryCountDetail);
                    await _unitOfWork.SaveAsync();
                }
            }

            return _mapper.Map<InventoryCountDTO>(inventoryCount);
        }

        public async Task<InventoryCountDTO> UpdateInventoryCount(InventoryCountUpdateDTO request)
        {
            var existingInventoryCount = await _genericRepository.GetByCondition(
                ic => ic.Id == request.Id,
                includeProperties: "InventoryCheckDetails");

            if (existingInventoryCount == null)
                throw new Exception("InventoryCount not found");

            if (request.Status.HasValue)
                existingInventoryCount.Status = request.Status.Value;

            if (!string.IsNullOrEmpty(request.Code))
                existingInventoryCount.Code = request.Code;

            if (!string.IsNullOrEmpty(request.Note))
                existingInventoryCount.Note = request.Note;

            if (request.Date.HasValue)
                existingInventoryCount.Date = request.Date;

            if (request.StartTime.HasValue)
                existingInventoryCount.StartTime = request.StartTime;

            if (request.EndTime.HasValue)
                existingInventoryCount.EndTime = request.EndTime;

            /*if (!string.IsNullOrEmpty(request.ScheduleId))
            {
                var schedule = await _scheduleRepository.GetByCondition(s => s.Id == request.ScheduleId);
                if (schedule == null)
                    throw new Exception("Schedule not found");

                existingInventoryCount.ScheduleId = request.ScheduleId;
                //existingInventoryCount.LocationId = schedule.LocationId;
            }*/


            if (request.InventoryCountDetails != null && request.InventoryCountDetails.Any())
            {
                foreach (var detailDto in request.InventoryCountDetails)
                {
                    // Nếu có Id, cập nhật chi tiết đã tồn tại
                    if (!string.IsNullOrEmpty(detailDto.Id))
                    {
                        var existingDetail = existingInventoryCount.InventoryCheckDetails
                                                .FirstOrDefault(d => d.Id == detailDto.Id);
                        if (existingDetail == null)
                            throw new Exception($"InventoryCountDetail with ID {detailDto.Id} not found");
                        if (detailDto.CountedQuantity != null)
                        {
                            existingDetail.CountedQuantity = detailDto.CountedQuantity.Value;
                            switch (existingDetail.CountedQuantity)
                            {
                                case var c when c < existingDetail.ExpectedQuantity:
                                    existingDetail.Status = InventoryCountDetailStatus.Understock;
                                    break;

                                case var c when c > existingDetail.ExpectedQuantity:
                                    existingDetail.Status = InventoryCountDetailStatus.Overstock;
                                    break;

                                case var c when c == existingDetail.ExpectedQuantity:
                                    existingDetail.Status = InventoryCountDetailStatus.Balanced;
                                    break;
                            }
                        }
                        if (!string.IsNullOrEmpty(detailDto.Note))
                            existingDetail.Note = detailDto.Note;
                        if (!string.IsNullOrEmpty(detailDto.InventoryId))
                        {
                            var inventory = await _inventoryRepository.GetByCondition(p => p.Id == detailDto.InventoryId);
                            if (inventory == null)
                                throw new Exception($"Inventory with ID {detailDto.InventoryId} not found");
                            existingDetail.InventoryId = detailDto.InventoryId;
                            //var expectedQuantity = await SumInventoryLocationQuantityByLocationLevel0AndInventory(existingInventoryCount.LocationId, existingDetail.InventoryId);
                            detailDto.ExpectedQuantity = inventory.CurrentQuantity;
                        }
                        if (!string.IsNullOrEmpty(detailDto.ErrorTicketId))
                            existingDetail.ErrorTicketId = detailDto.ErrorTicketId;
                    }
                    else
                    {
                        var inventory = await _inventoryRepository.GetByCondition(p => p.Id == detailDto.InventoryId);
                        if (inventory == null)
                            throw new Exception($"Inventory with ID {detailDto.InventoryId} not found");

                        var newDetail = _mapper.Map<InventoryCountDetail>(detailDto);
                        newDetail.InventoryCountId = existingInventoryCount.Id;
                        await _inventoryCountDetailRepository.Insert(newDetail);
                    }
                }
            }

            _genericRepository.Update(existingInventoryCount);
            await _unitOfWork.SaveAsync();

            var updatedInventoryCount = await _genericRepository.GetByCondition(
                ic => ic.Id == existingInventoryCount.Id,
                includeProperties: "InventoryCheckDetails.Product,InventoryCheckDetails"
            );

            if (updatedInventoryCount == null)
                throw new Exception("Update failed, InventoryCount not found after update");

            return _mapper.Map<InventoryCountDTO>(updatedInventoryCount);

        }

        public async Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null, InventoryCountStatus? status = null)
        {

            Expression<Func<InventoryCount, bool>> filter = p =>
            (p.Status == status &&
            (string.IsNullOrEmpty(keyword) || p.Code.Contains(keyword)
                || p.Note.Contains(keyword)
                || p.InventoryCheckDetails.Any(d => d.Note != null && d.Note.Contains(keyword)))
                //|| p.InventoryCheckDetails.Any(d => d.Product != null && d.Product.Name.Contains(keyword)))
                );/* &&
                (string.IsNullOrEmpty(warehouseId) || p.Location.Warehouse.Id == warehouseId);*/

            var totalRecords = await _genericRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize,
                includeProperties: "InventoryCheckDetails");

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

        private async Task<float> CalculateExpectedQuantity(string locationId, string productId)
        {
            var inventoryLocations = await _inventoryLocationRepository.GetAllNoPaging(
                filter: il => il.LocationId == locationId,
                includeProperties: "Inventory,Inventory.Batch"
                );


            var matchedInventories = inventoryLocations
                .Where(il => il.Inventory?.Batch != null && il.Inventory.Batch.ProductId == productId)
                .Select(il => il.Inventory)
                .Distinct()
                .ToList();

            float expectedQuantity = matchedInventories.Sum(inv => inv.CurrentQuantity);
            return expectedQuantity;
        }


        public async Task<InventoryByLocationDTO> GetInventoriesByLocationLevel0Async(string locationLevel0Id)
        {
            var locationLevel0 = await _locationRepository.GetByCondition(p => p.Id == locationLevel0Id);
            if (locationLevel0 != null)
            {
                if (locationLevel0.Level != 0)
                    throw new Exception("Location không phải là cấp độ 0");
            }
            else
                throw new Exception("Location không tồn tại");

            var level2LocationIds = await _locationRepository.GetAllNoPaging(
            x => x.Level == 2 && x.Parent!.ParentId == locationLevel0Id);

            var level2Ids = level2LocationIds.Select(x => x.Id).ToList();

            var inventoryLocations = await _inventoryLocationRepository.GetAllNoPaging(
            x => level2Ids.Contains(x.LocationId),
            includeProperties: "Inventory,Inventory.Batch");

            var inventories = inventoryLocations.Select(x => x.Inventory).Distinct().ToList();

            var dto = _mapper.Map<InventoryByLocationDTO>(locationLevel0);
            dto.InventoryLocations = _mapper.Map<List<CustomInventoryLocationDTO>>(inventoryLocations);
            dto.Inventories = _mapper.Map<List<InventoryWithProductDTO>>(inventories);

            return dto;

        }


        public async Task<float> SumInventoryLocationQuantityByLocationLevel0AndInventory(string locationLevel0Id, string inventoryId)
        {
            // Lấy tất cả Location level 2 thuộc location level 0
            var level2Locations = await _locationRepository.GetAllNoPaging(
                x => x.Level == 2 && x.Parent!.ParentId == locationLevel0Id
            );

            var level2LocationIds = level2Locations.Select(x => x.Id).ToList();

            // Lấy tất cả InventoryLocation thuộc các Location level 2 đó
            var inventoryLocations = await _inventoryLocationRepository.GetAllNoPaging(
                x => level2LocationIds.Contains(x.LocationId) && x.InventoryId == inventoryId,
                includeProperties: "Inventory"
            );

            // Lấy WarehouseId của Inventory truyền vào (chỉ để đảm bảo đúng warehouse)
            var inventory = await _inventoryRepository.GetByCondition(x => x.Id == inventoryId);
            if (inventory == null) return 0;

            var matchedInventoryLocations = inventoryLocations
                .Where(il => il.Inventory.WarehouseId == inventory.WarehouseId);

            return matchedInventoryLocations.Sum(il => (float)il.Quantity);
        }

    }
}
