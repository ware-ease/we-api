using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.GoodNote;
using Data.Model.Request.GoodRequest;
using Data.Model.Request.InventoryAdjustment;
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
using System.Transactions;

namespace BusinessLogicLayer.Services
{
    public class InventoryAdjustmentService : GenericService<InventoryAdjustment>, IInventoryAdjustmentService
    {
        IGenericRepository<InventoryAdjustmentDetail> _inventoryAdjustmentDetailRepository;
        IGenericRepository<Warehouse> _warehouseRepository;
        IGenericRepository<Inventory> _inventoryRepository;
        IGenericRepository<InventoryCount> _inventoryCountRepository;
        IGenericRepository<Batch> _batchRepository;
        private readonly IGoodRequestService _goodRequestService;
        private readonly IGoodNoteService _goodNoteService;
        public InventoryAdjustmentService(IGenericRepository<InventoryAdjustment> genericRepository,
            IGenericRepository<InventoryAdjustmentDetail> inventoryAdjustmentDetailRepository,
            IGenericRepository<Warehouse> warehouseRepository,
            IGenericRepository<Inventory> inventoryRepository,
            IGenericRepository<InventoryCount> inventoryCountRepository,
            IGenericRepository<Batch> batchRepository,
            IGoodRequestService goodRequestService,
            IGoodNoteService goodNoteService,
            IMapper mapper, IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            _inventoryAdjustmentDetailRepository = inventoryAdjustmentDetailRepository;
            _warehouseRepository = warehouseRepository;
            _inventoryRepository = inventoryRepository;
            _batchRepository = batchRepository;
            _inventoryCountRepository = inventoryCountRepository;
            _goodRequestService = goodRequestService;
            _goodNoteService = goodNoteService;

        }


        public async Task<InventoryAdjustmentDTO> AddInventoryAdjustment(InventoryAdjustmentCreateDTO request)
        {


            try
            {
                if (request.Date > DateTime.Now)
                    throw new Exception("Date không được ở tương lai");

                var warehouse = await _warehouseRepository.GetByCondition(p => p.Id == request.WarehouseId);
                if (warehouse == null)
                    throw new Exception("Warehouse không tồn tại");

                var inventoryAdjustment = _mapper.Map<InventoryAdjustment>(request);
                inventoryAdjustment.WarehouseId = request.WarehouseId;

                if (request.DocumentType.HasValue && (request.DocumentType != Data.Enum.DocumentType.GoodNote && request.DocumentType != Data.Enum.DocumentType.InventoryCount))
                    throw new Exception("DocumentType không hợp lệ");
                if (!string.IsNullOrEmpty(request.RelatedDocument))
                {
                    var inventoryCount = await _inventoryCountRepository.GetByCondition(p => p.Id == request.RelatedDocument);
                    if (inventoryCount != null)
                    {
                        //inventoryCount.Status = Data.Enum.InventoryCountStatus.Balanced;
                        _inventoryCountRepository.Update(inventoryCount);
                    }
                }

                await _genericRepository.Insert(inventoryAdjustment);
                //await _unitOfWork.SaveAsync();




                //==================================================================================================//


                foreach (var detailDto in request.InventoryAdjustmentDetails)
                {
                    var inventoryAdjustmentDetail = _mapper.Map<InventoryAdjustmentDetail>(detailDto);
                    inventoryAdjustmentDetail.InventoryAdjustmentId = inventoryAdjustment.Id;

                    var inventory = await _inventoryRepository.GetByCondition(p => p.Id == detailDto.InventoryId);
                    if (inventory == null)
                        throw new Exception($"Inventory với Id {detailDto.InventoryId} không tồn tại");
                    if (inventory.WarehouseId != request.WarehouseId)
                        throw new Exception($"Inventory với Id {detailDto.InventoryId} không thuộc Warehouse với Id {request.WarehouseId}");
                    var batch = await _batchRepository.GetByCondition(p => p.Id == inventory.BatchId);
                    if (batch == null)
                        throw new Exception($"Batch với Id {inventory.BatchId} không tồn tại");
                    if (detailDto.ProductId != batch.ProductId)
                        throw new Exception($"Batch với Id {inventory.BatchId} không thuộc Product với Id {detailDto.ProductId}");


                    //await _inventoryAdjustmentDetailRepository.Insert(inventoryAdjustmentDetail);
                    //await _unitOfWork.SaveAsync();

                }

                var goodRequestDTO = new GoodRequestCreateDTO
                {
                    Note = request.Note,
                    RequestType = request.RequestType,
                    WarehouseId = request.WarehouseId,
                    RequestedWarehouseId = request.WarehouseId,
                    CreatedBy = request.CreatedBy,
                    GoodRequestDetails = request.InventoryAdjustmentDetails.Select(d => new GoodRequestDetailDTO
                    {
                        Quantity = d.NewQuantity,
                        ProductId = d.ProductId,
                        CreatedBy = request.CreatedBy,
                    }).ToList()
                };

                var serviceResponse = await _goodRequestService.CreateAsync<GoodRequestDTO>(goodRequestDTO);
                await _unitOfWork.SaveAsync();

                var goodRequestData = serviceResponse.Data as GoodRequestDTO;
                if (goodRequestData == null)
                    throw new Exception("Không thể lấy thông tin GoodRequest từ ServiceResponse.");

                var goodNoteDTO = new GoodNoteCreateDTO
                {
                    NoteType = Data.Enum.GoodNoteEnum.Receive,
                    ShipperName = null,
                    ReceiverName = null,
                    Date = request.Date,
                    GoodRequestId = goodRequestData.Id,
                    GoodNoteDetails = request.InventoryAdjustmentDetails.Select(d => new GoodNoteDetailCreateDTO
                    {
                        Quantity = d.NewQuantity,
                        Note = d.Note,
                        CreatedBy = request.CreatedBy,
                    }).ToList()
                };

                return _mapper.Map<InventoryAdjustmentDTO>(inventoryAdjustment);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm InventoryAdjustment: {ex.Message}");
            }


        }


        public override async Task<ServiceResponse> Get<TResult>()
        {
            var inventoryAdjustments = await _genericRepository.GetAllNoPaging(
                includeProperties: "InventoryAdjustmentDetails,Warehouse"
            );

            List<TResult> mappedResults = _mapper.Map<List<TResult>>(inventoryAdjustments);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = mappedResults
            };
        }

        public async Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null, string? warehouseId = null)
        {

            Expression<Func<InventoryAdjustment, bool>> filter = p =>
                (string.IsNullOrEmpty(keyword) || p.Reason.Contains(keyword)
                || p.Note.Contains(keyword)
                || p.Warehouse.Name.Contains(keyword)
                || p.InventoryAdjustmentDetails.Any(d => d.Note != null && d.Note.Contains(keyword))
                &&
                (string.IsNullOrEmpty(warehouseId) || p.WarehouseId == warehouseId));

            var totalRecords = await _genericRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize,
                includeProperties: "InventoryAdjustmentDetails,Warehouse");

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


        /*public override async Task<ServiceResponse> Get<TResult>()
        {
            var inventoryAdjustments = await _genericRepository.GetAllNoPaging(
                includeProperties: "InventoryAdjustmentDetails,Warehouse,InventoryAdjustmentDetails.LocationLog"
            );

            List<TResult> mappedResults = _mapper.Map<List<TResult>>(inventoryAdjustments);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = mappedResults
            };
        }

        public async Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null, string? warehouseId = null)
        {

            Expression<Func<InventoryAdjustment, bool>> filter = p =>
                (string.IsNullOrEmpty(keyword) || p.Reason.Contains(keyword)
                || p.Note.Contains(keyword)
                || p.Warehouse.Name.Contains(keyword)
                || p.InventoryAdjustmentDetails.Any(d => d.Note != null && d.Note.Contains(keyword))
                &&
                (string.IsNullOrEmpty(warehouseId) || p.WarehouseId == warehouseId));

            var totalRecords = await _genericRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize,
                includeProperties: "InventoryAdjustmentDetails,Warehouse,InventoryAdjustmentDetails.LocationLog");

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

        public async Task<InventoryAdjustmentDTO> AddInventoryAdjustment(InventoryAdjustmentCreateDTO request)
        {//1

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {//2
                try
                {//3
                    if (request.Date > DateTime.Now)
                        throw new Exception("Date không được ở tương lai");

                    var warehouse = await _warehouseRepository.GetByCondition(p => p.Id == request.WarehouseId);
                    if (warehouse == null)
                        throw new Exception("Warehouse không tồn tại");

                    var inventoryAdjustment = _mapper.Map<InventoryAdjustment>(request);
                    inventoryAdjustment.WarehouseId = request.WarehouseId;

                    if (request.DocumentType.HasValue && (request.DocumentType != Data.Enum.DocumentType.GoodNote || request.DocumentType != Data.Enum.DocumentType.InventoryCount))
                        throw new Exception("DocumentType không hợp lệ");
                    if (!string.IsNullOrEmpty(request.RelatedDocument))
                    {
                        var inventoryCount = await _inventoryCountRepository.GetByCondition(p => p.Id == request.RelatedDocument);
                        if (inventoryCount != null)
                        {
                            inventoryCount.Status = Data.Enum.InventoryCountStatus.Balanced;
                            _inventoryCountRepository.Update(inventoryCount);
                        }
                    }

                    await _genericRepository.Insert(inventoryAdjustment);
                    await _unitOfWork.SaveAsync();



                    //==================================================================================================//


                    foreach (var detailDto in request.InventoryAdjustmentDetails)
                    {
                        var inventoryAdjustmentDetail = _mapper.Map<InventoryAdjustmentDetail>(detailDto);
                        inventoryAdjustmentDetail.InventoryAdjustmentId = inventoryAdjustment.Id;

                        var location = await _locationRepository.GetByCondition(p => p.Id == detailDto.LocationId);
                        if (location == null)
                            throw new Exception($"Location với Id {detailDto.LocationId} không tồn tại");
                        if (location.WarehouseId != request.WarehouseId)
                            throw new Exception($"Location với Id {detailDto.LocationId} không thuộc Warehouse với Id {request.WarehouseId}");

                        var inventory = await _inventoryRepository.GetByCondition(p => p.Id == detailDto.InventoryId);
                        if (inventory == null)
                            throw new Exception($"Inventory với Id {detailDto.InventoryId} không tồn tại");
                        if (inventory.WarehouseId != request.WarehouseId)
                            throw new Exception($"Inventory với Id {detailDto.InventoryId} không thuộc Warehouse với Id {request.WarehouseId}");

                        var inventoryLocation = await _inventoryLocationRepository
                            .GetByCondition(p => p.InventoryId == detailDto.InventoryId && p.LocationId == detailDto.LocationId);
                        if (inventoryLocation == null)
                        {
                            inventoryLocation = new InventoryLocation
                            {
                                InventoryId = detailDto.InventoryId,
                                LocationId = detailDto.LocationId,
                                Quantity = (int)detailDto.NewQuantity
                            };
                            await _inventoryLocationRepository.Insert(inventoryLocation);
                            await _unitOfWork.SaveAsync();
                        }

                        await _inventoryAdjustmentDetailRepository.Insert(inventoryAdjustmentDetail);
                        await _unitOfWork.SaveAsync();

                        var locationLog = new LocationLog
                        {
                            InventoryLocationId = inventoryLocation.Id,
                            NewQuantity = detailDto.NewQuantity,
                            ChangeInQuantity = detailDto.ChangeInQuantity
                        };

                        await _locationLogRepository.Insert(locationLog);
                        await _unitOfWork.SaveAsync();



                        inventoryAdjustmentDetail.LocationLogId = locationLog.Id;
                        _inventoryAdjustmentDetailRepository.Update(inventoryAdjustmentDetail);
                        await _unitOfWork.SaveAsync();

                        int inventoryLocation_OldQuantity = inventoryLocation.Quantity;
                        inventoryLocation.Quantity = (int)locationLog.NewQuantity;
                        _inventoryLocationRepository.Update(inventoryLocation);
                        await _unitOfWork.SaveAsync();

                        if (inventoryLocation_OldQuantity < inventoryLocation.Quantity)
                        {
                            inventory.CurrentQuantity = inventory.CurrentQuantity + (inventoryLocation.Quantity - inventoryLocation_OldQuantity);
                        }
                        else
                        {
                            if (inventoryLocation_OldQuantity > inventoryLocation.Quantity)
                            {
                                if ((inventory.CurrentQuantity - (inventoryLocation_OldQuantity - inventoryLocation.Quantity)) < 0)
                                {
                                    throw new Exception("Số lượng tồn kho không đủ để thực hiện điều chỉnh kho");
                                }
                                inventory.CurrentQuantity = inventory.CurrentQuantity - (inventoryLocation_OldQuantity - inventoryLocation.Quantity);
                            }
                        }
                        _inventoryRepository.Update(inventory);
                        await _unitOfWork.SaveAsync();
                    }
                    await _unitOfWork.SaveAsync();
                    scope.Complete();
                    return _mapper.Map<InventoryAdjustmentDTO>(inventoryAdjustment);
                }//3
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi thêm InventoryAdjustment: {ex.Message}");
                }
            }//2

        }//1

        public async Task<InventoryAdjustmentDTO> UpdateInventoryAdjustment(InventoryAdjustmentUpdateDTO request)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var inventoryAdjustment = await _genericRepository.GetByCondition(p => p.Id == request.Id);
                    if (inventoryAdjustment == null)
                        throw new Exception("InventoryAdjustment không tồn tại");

                    if (request.Date.HasValue)
                    {
                        if (request.Date > DateTime.Now)
                            throw new Exception("Date không được ở tương lai");
                        inventoryAdjustment.Date = request.Date.Value;
                    }
                    if (!string.IsNullOrEmpty(request.Reason))
                        inventoryAdjustment.Reason = request.Reason;
                    if (!string.IsNullOrEmpty(request.Note))
                        inventoryAdjustment.Note = request.Note;
                    if (request.DocumentType.HasValue && (request.DocumentType != Data.Enum.DocumentType.GoodNote || request.DocumentType != Data.Enum.DocumentType.InventoryCount))
                    {
                        throw new Exception("DocumentType không hợp lệ");
                    }
                    else inventoryAdjustment.DocumentType = request.DocumentType;


                    if (!string.IsNullOrEmpty(request.RelatedDocument))
                    {
                        var inventoryCount = await _inventoryCountRepository.GetByCondition(p => p.Id == request.RelatedDocument);
                        if (inventoryCount != null)
                        {
                            inventoryCount.Status = Data.Enum.InventoryCountStatus.Balanced;
                            _inventoryCountRepository.Update(inventoryCount);
                        }
                    }

                    if (!string.IsNullOrEmpty(request.WarehouseId))
                    {
                        var warehouse = await _warehouseRepository.GetByCondition(p => p.Id == request.WarehouseId);
                        if (warehouse == null)
                            throw new Exception("Warehouse không tồn tại");
                        inventoryAdjustment.WarehouseId = request.WarehouseId;
                    }

                    _genericRepository.Update(inventoryAdjustment);
                    await _unitOfWork.SaveAsync();

                    // ==================================================================================== //
                    if (request.InventoryAdjustmentDetails != null)
                    {
                        foreach (var detailDto in request.InventoryAdjustmentDetails)
                        {
                            var inventoryAdjustmentDetail = await _inventoryAdjustmentDetailRepository
                                .GetByCondition(p => p.Id == detailDto.Id);
                            if (inventoryAdjustmentDetail == null)
                                throw new Exception($"InventoryAdjustmentDetail với Id {detailDto.Id} không tồn tại");

                            bool newQuantityChanged = false;

                            if (detailDto.NewQuantity.HasValue)
                            {
                                inventoryAdjustmentDetail.NewQuantity = detailDto.NewQuantity.Value;
                                newQuantityChanged = true;
                            }
                            if (detailDto.ChangeInQuantity.HasValue)
                                inventoryAdjustmentDetail.ChangeInQuantity = detailDto.ChangeInQuantity.Value;


                            if (!string.IsNullOrEmpty(detailDto.Note))
                                inventoryAdjustmentDetail.Note = detailDto.Note;

                            string oldLocationLogId = inventoryAdjustmentDetail.LocationLogId;
                            var locationLog = await _locationLogRepository.GetByCondition(p => p.Id == oldLocationLogId);
                            if (locationLog == null)
                                throw new Exception("LocationLog không tồn tại");
                            string oldInventoryLocationId = locationLog.InventoryLocationId;
                            var inventoryLocation = await _inventoryLocationRepository.GetByCondition(p => p.Id == oldInventoryLocationId);
                            if (inventoryLocation == null)
                                throw new Exception("InventoryLocation không tồn tại");


                            if (newQuantityChanged == true)
                            {
                                var NewLocationLog = new LocationLog
                                {
                                    InventoryLocationId = inventoryLocation.Id,
                                    NewQuantity = inventoryAdjustmentDetail.NewQuantity,
                                    ChangeInQuantity = inventoryAdjustmentDetail.ChangeInQuantity,
                                };
                                await _locationLogRepository.Insert(NewLocationLog);
                                await _unitOfWork.SaveAsync();

                                inventoryAdjustmentDetail.LocationLogId = locationLog.Id;
                                _inventoryAdjustmentDetailRepository.Update(inventoryAdjustmentDetail);
                                await _unitOfWork.SaveAsync();



                                int inventoryLocation_OldQuantity = inventoryLocation.Quantity;
                                inventoryLocation.Quantity = (int)locationLog.NewQuantity;
                                _inventoryLocationRepository.Update(inventoryLocation);
                                await _unitOfWork.SaveAsync();

                                var inventory = await _inventoryRepository.GetByCondition(p => p.Id == inventoryLocation.InventoryId);
                                if (inventoryLocation_OldQuantity < inventoryLocation.Quantity)
                                {
                                    inventory.CurrentQuantity = inventory.CurrentQuantity + (inventoryLocation.Quantity - inventoryLocation_OldQuantity);
                                }
                                else
                                {
                                    if (inventoryLocation_OldQuantity > inventoryLocation.Quantity)
                                    {
                                        if ((inventory.CurrentQuantity - (inventoryLocation_OldQuantity - inventoryLocation.Quantity)) < 0)
                                        {
                                            throw new Exception("Số lượng tồn kho không đủ để thực hiện điều chỉnh kho");
                                        }
                                        inventory.CurrentQuantity = inventory.CurrentQuantity - (inventoryLocation_OldQuantity - inventoryLocation.Quantity);
                                    }
                                }
                                _inventoryRepository.Update(inventory);
                                await _unitOfWork.SaveAsync();
                            }

                            _inventoryAdjustmentDetailRepository.Update(inventoryAdjustmentDetail);
                            await _unitOfWork.SaveAsync();


                        }
                    }

                    scope.Complete();
                    return _mapper.Map<InventoryAdjustmentDTO>(inventoryAdjustment);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi cập nhật InventoryAdjustment: {ex.Message}");
                }
            }
        }*/

    }
}
