using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
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
        public InventoryCountService(IGenericRepository<InventoryCount> genericRepository,
            IGenericRepository<Schedule> scheduleRepository,
            IGenericRepository<Location> locationRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<InventoryCountDetail> inventoryCountDetailRepository,
            IMapper mapper, IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _locationRepository = locationRepository;
            _productRepository = productRepository;
            _inventoryCountDetailRepository = inventoryCountDetailRepository;
        }


        public override async Task<ServiceResponse> Get<TResult>(string id)
        {
            var inventoryCount = await _genericRepository.GetByCondition(
                p => p.Id == id,
                includeProperties: "Schedule,Location,Location.Warehouse,InventoryCheckDetails,InventoryCheckDetails.Product"
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

            var schedule = await _scheduleRepository.GetByCondition(p => p.Id == request.ScheduleId);
            if (schedule == null)
                throw new Exception("Schedule không tồn tại");

            var location = await _locationRepository.GetByCondition(p => p.Id == schedule.LocationId);
            if (location != null)
            {
                if (location.Level != 0)
                    throw new Exception("Level phải bằng 0");
            }
            else
                throw new Exception("Location không tồn tại");

            /*var product = await _productRepository.GetByCondition(p => p.Id == request.InventoryCountDetailCreateDTO.ProductId);
            if (location == null)
                throw new Exception("Product không tồn tại");*/

            var inventoryCount = _mapper.Map<InventoryCount>(request);
            inventoryCount.LocationId = schedule.LocationId;

            await _genericRepository.Insert(inventoryCount);
            await _unitOfWork.SaveAsync();

            if (request.InventoryCountDetails != null && request.InventoryCountDetails.Any())
            {
                var inventoryCountDetails = new List<InventoryCountDetail>();

                foreach (var detail in request.InventoryCountDetails)
                {
                    var product = await _productRepository.GetByCondition(p => p.Id == detail.ProductId);
                    if (product == null)
                        throw new Exception($"Sản phẩm với ID {detail.ProductId} không tồn tại");

                    var inventoryCountDetail = _mapper.Map<InventoryCountDetail>(detail);
                    inventoryCountDetail.InventoryCountId = inventoryCount.Id;
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

            /*if (request.Status.HasValue)
                existingInventoryCount.Status = request.Status.Value;*/

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

            if (!string.IsNullOrEmpty(request.ScheduleId))
            {
                var schedule = await _scheduleRepository.GetByCondition(s => s.Id == request.ScheduleId);
                if (schedule == null)
                    throw new Exception("Schedule not found");

                existingInventoryCount.ScheduleId = request.ScheduleId;
                existingInventoryCount.LocationId = schedule.LocationId;
            }


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

                        if (detailDto.ExpectedQuantity != null)
                            existingDetail.ExpectedQuantity = detailDto.ExpectedQuantity.Value;
                        if (detailDto.CountedQuantity != null)
                            existingDetail.CountedQuantity = detailDto.CountedQuantity.Value;
                        if (!string.IsNullOrEmpty(detailDto.Note))
                            existingDetail.Note = detailDto.Note;
                        if (!string.IsNullOrEmpty(detailDto.ProductId))
                        {
                            var product = await _productRepository.GetByCondition(p => p.Id == detailDto.ProductId);
                            if (product == null)
                                throw new Exception($"Product with ID {detailDto.ProductId} not found");
                            existingDetail.ProductId = detailDto.ProductId;
                        }
                        if (!string.IsNullOrEmpty(detailDto.ErrorTicketId))
                            existingDetail.ErrorTicketId = detailDto.ErrorTicketId;
                    }
                    else
                    {
                        var product = await _productRepository.GetByCondition(p => p.Id == detailDto.ProductId);
                        if (product == null)
                            throw new Exception($"Product with ID {detailDto.ProductId} not found");

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
                includeProperties: "Schedule,Location,InventoryCheckDetails.Product,InventoryCheckDetails"
            );

            if (updatedInventoryCount == null)
                throw new Exception("Update failed, InventoryCount not found after update");

            return _mapper.Map<InventoryCountDTO>(updatedInventoryCount);

        }

        public async Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null, bool? status = null, string? warehouseId = null)
        {

            Expression<Func<InventoryCount, bool>> filter = p =>
            (//p.Status == status &&
            (string.IsNullOrEmpty(keyword) || p.Code.Contains(keyword)
                || p.Note.Contains(keyword)
                || p.Location.Name.Contains(keyword)
                || p.Location.Code.Contains(keyword)
                || p.Location.Warehouse.Name.Contains(keyword)
                || p.InventoryCheckDetails.Any(d => d.Note != null && d.Note.Contains(keyword))
                || p.InventoryCheckDetails.Any(d => d.Product != null && d.Product.Name.Contains(keyword)))
                ) &&
                (string.IsNullOrEmpty(warehouseId) || p.Location.Warehouse.Id == warehouseId);

            var totalRecords = await _genericRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize,
                includeProperties: "Schedule,Location,Location.Warehouse,InventoryCheckDetails.Product,InventoryCheckDetails");

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

    }
}
