using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.Batch;
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
        private readonly ICodeGeneratorService _codeGeneratorService;
        public InventoryAdjustmentService(IGenericRepository<InventoryAdjustment> genericRepository,
            IGenericRepository<InventoryAdjustmentDetail> inventoryAdjustmentDetailRepository,
            IGenericRepository<Warehouse> warehouseRepository,
            IGenericRepository<Inventory> inventoryRepository,
            IGenericRepository<InventoryCount> inventoryCountRepository,
            IGenericRepository<Batch> batchRepository,
            IGoodRequestService goodRequestService,
            IGoodNoteService goodNoteService,
            ICodeGeneratorService codeGeneratorService,
            IMapper mapper, IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            _inventoryAdjustmentDetailRepository = inventoryAdjustmentDetailRepository;
            _warehouseRepository = warehouseRepository;
            _inventoryRepository = inventoryRepository;
            _batchRepository = batchRepository;
            _inventoryCountRepository = inventoryCountRepository;
            _goodRequestService = goodRequestService;
            _goodNoteService = goodNoteService;
            _codeGeneratorService = codeGeneratorService;
        }

        public async Task<InventoryAdjustmentDTO> AddInventoryAdjustmentWithDetail(InventoryAdjustmentCreateDTOv2 request)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (request.Date > DateTime.Now)
                        throw new Exception("Date không được ở tương lai");
                    var warehouse = await _warehouseRepository.GetByCondition(p => p.Id == request.WarehouseId);
                    if (warehouse == null)
                        throw new Exception("Warehouse không tồn tại");
                    var inventoryAdjustment = new InventoryAdjustment
                    {
                        Date = request.Date,
                        Reason = request.Reason,
                        Note = request.Note,
                        Code = await _codeGeneratorService.GenerateCodeAsync(Data.Enum.CodeType.PDC),
                        DocumentType = request.DocumentType,
                        RelatedDocument = request.RelatedDocument,
                        WarehouseId = request.WarehouseId,
                        CreatedBy = request.CreatedBy,
                    };
                    if (request.DocumentType.HasValue && (request.DocumentType != Data.Enum.DocumentType.GoodNote && request.DocumentType != Data.Enum.DocumentType.InventoryCount))
                        throw new Exception("DocumentType không hợp lệ");
                    /*if (!string.IsNullOrEmpty(request.RelatedDocument))
                    {
                        var inventoryCount = await _inventoryCountRepository.GetByCondition(p => p.Id == request.RelatedDocument);
                        if (inventoryCount != null)
                        {
                            inventoryCount.Status = Data.Enum.InventoryCountStatus.OnTime;
                            _inventoryCountRepository.Update(inventoryCount);
                        }
                    }*/
                    await _genericRepository.Insert(inventoryAdjustment);
                    await _unitOfWork.SaveAsync();
                    foreach (var detailDto in request.InventoryAdjustmentDetails)
                    {
                        var inventoryAdjustmentDetail = new InventoryAdjustmentDetail
                        {
                            InventoryAdjustmentId = inventoryAdjustment.Id,
                            InventoryId = detailDto.InventoryId,
                            ChangeInQuantity = detailDto.ChangeInQuantity,
                            CreatedBy = request.CreatedBy
                        };
                        //inventoryAdjustmentDetail.InventoryAdjustmentId = inventoryAdjustment.Id;
                        switch (detailDto.ChangeInQuantity)
                        {
                            case > 0:
                                {
                                    var inventory = await _inventoryRepository.GetByCondition(p => p.Id == detailDto.InventoryId,
                                        includeProperties: "Batch");
                                    if (inventory == null)
                                        throw new Exception($"Inventory với Id {detailDto.InventoryId} không tồn tại");
                                    if (inventory.WarehouseId != request.WarehouseId)
                                        throw new Exception($"Inventory với Id {detailDto.InventoryId} không thuộc Warehouse với Id {request.WarehouseId}");

                                    inventoryAdjustmentDetail.NewQuantity = inventory.CurrentQuantity + detailDto.ChangeInQuantity;
                                    //inventoryAdjustmentDetail.Inventory = null;

                                    await _inventoryAdjustmentDetailRepository.Insert(inventoryAdjustmentDetail);
                                    await _unitOfWork.SaveAsync();

                                    var goodRequestEntity = new GoodRequest
                                    {
                                        Note = "From adjustment with code: " + inventoryAdjustment.Code,
                                        //Code = await _codeGeneratorService.GenerateCodeAsync(Data.Enum.CodeType.YCN),
                                        Code = "AR",
                                        RequestType = Data.Enum.GoodRequestEnum.Receive,
                                        //WarehouseId = null,
                                        RequestedWarehouseId = request.WarehouseId,
                                        CreatedBy = request.CreatedBy,
                                        Status = Data.Enum.GoodRequestStatusEnum.Completed,
                                        GoodRequestDetails = new List<GoodRequestDetail>
                                        {
                                            new GoodRequestDetail
                                            {
                                                Quantity = detailDto.ChangeInQuantity,
                                                ProductId = inventory.Batch.ProductId,
                                                CreatedBy = request.CreatedBy
                                            }
                                        }
                                    };
                                    await _unitOfWork.GoodRequestRepository.Insert(goodRequestEntity);
                                    await _unitOfWork.SaveAsync();


                                    var goodNoteEntity = new GoodNote
                                    {
                                        NoteType = Data.Enum.GoodNoteEnum.Receive,
                                        ShipperName = null,
                                        ReceiverName = null,
                                        Code = await _codeGeneratorService.GenerateCodeAsync(Data.Enum.CodeType.PNDC),
                                        Date = request.Date,
                                        GoodRequestId = goodRequestEntity.Id,
                                    };
                                    await _unitOfWork.GoodNoteRepository.Insert(goodNoteEntity);
                                    await _unitOfWork.SaveAsync();

                                    var goodNoteDetailEntity = new GoodNoteDetail
                                    {
                                        Quantity = detailDto.ChangeInQuantity,
                                        Note = "Receive Adjustment by adjustment code:" + inventoryAdjustment.Code + ", GoodNote code:" + goodNoteEntity.Code,
                                        CreatedBy = request.CreatedBy,
                                        BatchId = inventory.BatchId,
                                        GoodNoteId = goodNoteEntity.Id
                                    };
                                    await _unitOfWork.GoodNoteDetailRepository.Insert(goodNoteDetailEntity);
                                    await _unitOfWork.SaveAsync();

                                    
                                    inventory.CurrentQuantity = inventory.CurrentQuantity + detailDto.ChangeInQuantity;
                                    //inventory.InventoryAdjustmentDetails = null;
                                    _inventoryRepository.Update(inventory);
                                    await _unitOfWork.SaveAsync();

                                    break;
                                }
                            case < 0:
                                {
                                    {
                                        var inventory = await _inventoryRepository.GetByCondition(p => p.Id == detailDto.InventoryId,
                                            includeProperties: "Batch");
                                        if (inventory == null)
                                            throw new Exception($"Inventory với Id {detailDto.InventoryId} không tồn tại");
                                        if (inventory.WarehouseId != request.WarehouseId)
                                            throw new Exception($"Inventory với Id {detailDto.InventoryId} không thuộc Warehouse với Id {request.WarehouseId}");

                                        inventoryAdjustmentDetail.NewQuantity = inventory.CurrentQuantity + detailDto.ChangeInQuantity;

                                        await _inventoryAdjustmentDetailRepository.Insert(inventoryAdjustmentDetail);
                                        await _unitOfWork.SaveAsync();

                                        var goodRequestEntity = new GoodRequest
                                        {
                                            Note = "From adjustment with code: " + inventoryAdjustment.Code,
                                            //Code = await _codeGeneratorService.GenerateCodeAsync(Data.Enum.CodeType.YCX),
                                            Code = "AR",
                                            RequestType = Data.Enum.GoodRequestEnum.Issue,
                                            //WarehouseId = null,
                                            RequestedWarehouseId = request.WarehouseId,
                                            CreatedBy = request.CreatedBy,
                                            Status = Data.Enum.GoodRequestStatusEnum.Completed,
                                            GoodRequestDetails = new List<GoodRequestDetail>
                                        {
                                            new GoodRequestDetail
                                            {
                                                Quantity = Math.Abs(detailDto.ChangeInQuantity),
                                                ProductId = inventory.Batch.ProductId,
                                                CreatedBy = request.CreatedBy
                                            }
                                        }
                                        };
                                        await _unitOfWork.GoodRequestRepository.Insert(goodRequestEntity);
                                        await _unitOfWork.SaveAsync();


                                        var goodNoteEntity = new GoodNote
                                        {
                                            NoteType = Data.Enum.GoodNoteEnum.Issue,
                                            ShipperName = null,
                                            ReceiverName = null,
                                            Code = await _codeGeneratorService.GenerateCodeAsync(Data.Enum.CodeType.PXDC),
                                            Date = request.Date,
                                            GoodRequestId = goodRequestEntity.Id,
                                        };
                                        await _unitOfWork.GoodNoteRepository.Insert(goodNoteEntity);
                                        await _unitOfWork.SaveAsync();

                                        var goodNoteDetailEntity = new GoodNoteDetail
                                        {
                                            Quantity = Math.Abs(detailDto.ChangeInQuantity),
                                            Note = "Issue Adjustment by adjustment code:" + inventoryAdjustment.Code + ", GoodNote code:" + goodNoteEntity.Code,
                                            CreatedBy = request.CreatedBy,
                                            BatchId = inventory.BatchId,
                                            GoodNoteId = goodNoteEntity.Id
                                        };
                                        await _unitOfWork.GoodNoteDetailRepository.Insert(goodNoteDetailEntity);
                                        await _unitOfWork.SaveAsync();

                                        inventory.CurrentQuantity = inventory.CurrentQuantity + detailDto.ChangeInQuantity;
                                        if (inventory.CurrentQuantity < 0)
                                            throw new Exception($"Inventory với Id {detailDto.InventoryId} không đủ số lượng để xuất");
                                        //inventory.InventoryAdjustmentDetails = null;
                                        _inventoryRepository.Update(inventory);
                                        await _unitOfWork.SaveAsync();

                                        break;
                                    }
                                }
                            default:
                                throw new Exception("ChangeInQuantity không hợp lệ");
                        }
                        //await _genericRepository.Insert(inventoryAdjustment);
                        //await _unitOfWork.SaveAsync();
                    }
                    scope.Complete();
                    return _mapper.Map<InventoryAdjustmentDTO>(inventoryAdjustment);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi thêm InventoryAdjustment: {ex.Message}");
                }
            }
        }

        public async Task<InventoryAdjustmentDTO> AddInventoryAdjustment(InventoryAdjustmentCreateDTO request)
        {

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (request.Date > DateTime.Now)
                        throw new Exception("Date không được ở tương lai");

                    var warehouse = await _warehouseRepository.GetByCondition(p => p.Id == request.WarehouseId);
                    if (warehouse == null)
                        throw new Exception("Warehouse không tồn tại");

                    var inventoryCount = await _inventoryCountRepository.GetByCondition(x => x.Id == request.InventoryCountId,
                        includeProperties: "InventoryCheckDetails,InventoryCheckDetails.Inventory.Batch.Product");
                    if (inventoryCount == null)
                        throw new Exception("InventoryCount không tồn tại");

                    inventoryCount.Status = Data.Enum.InventoryCountStatus.Adjusted;
                    _inventoryCountRepository.Update(inventoryCount);
                    await _unitOfWork.SaveAsync();




                    var inventoryAdjustment = _mapper.Map<InventoryAdjustment>(request);
                    inventoryAdjustment.WarehouseId = request.WarehouseId;
                    inventoryAdjustment.Code = await _codeGeneratorService.GenerateCodeAsync(Data.Enum.CodeType.PDC);

                    if (request.DocumentType.HasValue && (request.DocumentType != Data.Enum.DocumentType.GoodNote && request.DocumentType != Data.Enum.DocumentType.InventoryCount))
                        throw new Exception("DocumentType không hợp lệ");
                    /*if (!string.IsNullOrEmpty(request.RelatedDocument))
                    {
                        //var inventoryCount = await _inventoryCountRepository.GetByCondition(p => p.Id == request.RelatedDocument);
                        if (inventoryCount != null)
                        {
                            //inventoryCount.Status = Data.Enum.InventoryCountStatus.Balanced;
                            _inventoryCountRepository.Update(inventoryCount);
                        }
                    }*/

                    await _genericRepository.Insert(inventoryAdjustment);
                    await _unitOfWork.SaveAsync();




                    //==================================================================================================//


                    foreach (var detailDto in inventoryCount.InventoryCheckDetails)
                    {
                        /*var inventoryAdjustmentDetail = _mapper.Map<InventoryAdjustmentDetail>(detailDto);
                        inventoryAdjustmentDetail.InventoryAdjustmentId = inventoryAdjustment.Id;*/
                        float changeInQuantity = 0;
                        float newQuantity = 0;



                        switch (detailDto.Status)
                        {
                            case Data.Enum.InventoryCountDetailStatus.Overstock:
                                {
                                    changeInQuantity = detailDto.CountedQuantity.Value - detailDto.ExpectedQuantity;
                                    newQuantity = detailDto.CountedQuantity.Value;

                                    var adjustmentDetail = new InventoryAdjustmentDetailCreateDTO
                                    {
                                        NewQuantity = newQuantity,
                                        ChangeInQuantity = changeInQuantity,
                                        Note = detailDto.Note,
                                        //ProductId = detailDto.ProductId,
                                        InventoryId = detailDto.InventoryId,
                                    };
                                    var inventoryAdjustmentDetail = _mapper.Map<InventoryAdjustmentDetail>(adjustmentDetail);
                                    inventoryAdjustmentDetail.InventoryAdjustmentId = inventoryAdjustment.Id;

                                    await _inventoryAdjustmentDetailRepository.Insert(inventoryAdjustmentDetail);

                                    var goodRequestEntity = new GoodRequest
                                    {
                                        Note = "From adjustment with code: " + inventoryAdjustment.Code,
                                        //Code = await _codeGeneratorService.GenerateCodeAsync(Data.Enum.CodeType.YCN),
                                        Code = "AR",
                                        RequestType = Data.Enum.GoodRequestEnum.Receive,
                                        //WarehouseId = null,
                                        RequestedWarehouseId = request.WarehouseId,
                                        CreatedBy = request.CreatedBy,
                                        Status = Data.Enum.GoodRequestStatusEnum.Completed,
                                        GoodRequestDetails = new List<GoodRequestDetail>
                                    {
                                        new GoodRequestDetail
                                        {
                                            Quantity = changeInQuantity,
                                            ProductId = detailDto.Inventory.Batch.ProductId,
                                            CreatedBy = request.CreatedBy
                                        }
                                    }


                                        //new List<GoodRequestDetailDTO>
                                        //{
                                        //    new GoodRequestDetailDTO
                                        //    {
                                        //        Quantity = changeInQuantity,
                                        //        ProductId = detailDto.Inventory.Batch.ProductId,
                                        //        CreatedBy = request.CreatedBy
                                        //    }
                                        //}
                                    };

                                    //var serviceResponse = await _goodRequestService.CreateAsync<GoodRequestDTO>(goodRequestDTO);
                                    await _unitOfWork.GoodRequestRepository.Insert(goodRequestEntity);
                                    await _unitOfWork.SaveAsync();

                                    //var goodRequestData = serviceResponse.Data as GoodRequestDTO;
                                    //if (goodRequestData == null)
                                    //    throw new Exception($"Không thể lấy thông tin GoodRequest từ ServiceResponse. Data: {System.Text.Json.JsonSerializer.Serialize(serviceResponse.Data)}");

                                    // Fix for CS0747: Invalid initializer member declarator
                                    // The issue is caused by the incorrect use of an object initializer for `BatchCreateDTOv2`.
                                    // The `new` keyword is missing for the `BatchCreateDTOv2` object inside the `GoodNoteDetailCreateDTO` initializer.

                                    var goodNoteEntity = new GoodNote
                                    {
                                        NoteType = Data.Enum.GoodNoteEnum.Receive,
                                        ShipperName = null,
                                        ReceiverName = null,
                                        Code = await _codeGeneratorService.GenerateCodeAsync(Data.Enum.CodeType.PNDC),
                                        Date = request.Date,
                                        GoodRequestId = goodRequestEntity.Id,
                                        /*GoodNoteDetails = new List<GoodNoteDetailCreateDTO>
                                        {
                                            new GoodNoteDetailCreateDTO
                                            {
                                                Quantity = changeInQuantity,
                                                Note = "Adjustment for overstock",
                                                CreatedBy = request.CreatedBy,
                                                NewBatch = new BatchCreateDTOv2
                                                {
                                                    ProductId = detailDto.Inventory.Batch.ProductId,
                                                    Name = detailDto.Inventory.Batch.Name,
                                                    Code = detailDto.Inventory.Batch.Code,
                                                }
                                            }
                                        }*/
                                    };

                                    await _unitOfWork.GoodNoteRepository.Insert(goodNoteEntity);
                                    await _unitOfWork.SaveAsync();

                                    var goodNoteDetailEntity = new GoodNoteDetail
                                    {
                                        Quantity = changeInQuantity,
                                        Note = "Adjustment for overstock from adjustment code:" + inventoryAdjustment.Code,
                                        CreatedBy = request.CreatedBy,
                                        BatchId = detailDto.Inventory.BatchId,
                                        GoodNoteId = goodNoteEntity.Id
                                    };

                                    await _unitOfWork.GoodNoteDetailRepository.Insert(goodNoteDetailEntity);
                                    await _unitOfWork.SaveAsync();

                                    var inventoryUpdate = await _inventoryRepository.GetByCondition(p => p.Id == detailDto.InventoryId);
                                    if (inventoryUpdate == null)
                                        throw new Exception($"Inventory with Id {detailDto.InventoryId} doesnt exist");
                                    inventoryUpdate.CurrentQuantity = inventoryUpdate.CurrentQuantity + changeInQuantity;

                                    _inventoryRepository.Update(inventoryUpdate);
                                    await _unitOfWork.SaveAsync();
                                    //var goodNote = await _goodNoteService.CreateReceiveNoteAsync(goodNoteDTO);
                                    //var goodNoteData = goodNote.Data as GoodNoteDTO;
                                    //if (goodNoteData == null)
                                    //    throw new Exception($"Không thể lấy thông tin GoodNote từ ServiceResponse. Data: {System.Text.Json.JsonSerializer.Serialize(goodNote.Data)}");
                                    detailDto.Status = Data.Enum.InventoryCountDetailStatus.Balanced;
                                    break;
                                }
                            case Data.Enum.InventoryCountDetailStatus.Understock:
                                {
                                    changeInQuantity = detailDto.ExpectedQuantity - detailDto.CountedQuantity.Value;
                                    newQuantity = detailDto.CountedQuantity.Value;

                                    var adjustmentDetail = new InventoryAdjustmentDetailCreateDTO
                                    {
                                        NewQuantity = newQuantity,
                                        ChangeInQuantity = changeInQuantity,
                                        Note = detailDto.Note,
                                        //ProductId = detailDto.ProductId,
                                        InventoryId = detailDto.InventoryId,
                                    };

                                    var inventoryAdjustmentDetail = _mapper.Map<InventoryAdjustmentDetail>(adjustmentDetail);
                                    inventoryAdjustmentDetail.InventoryAdjustmentId = inventoryAdjustment.Id;

                                    await _inventoryAdjustmentDetailRepository.Insert(inventoryAdjustmentDetail);

                                    var goodRequestEntity = new GoodRequest
                                    {
                                        Note = "From adjustment with code: " + inventoryAdjustment.Code,
                                        //Code = await _codeGeneratorService.GenerateCodeAsync(Data.Enum.CodeType.YCX),
                                        Code = "AR",
                                        RequestType = Data.Enum.GoodRequestEnum.Issue,
                                        //WarehouseId = null,
                                        RequestedWarehouseId = request.WarehouseId,
                                        CreatedBy = request.CreatedBy,
                                        Status = Data.Enum.GoodRequestStatusEnum.Completed,
                                        GoodRequestDetails = new List<GoodRequestDetail>
                                    {
                                        new GoodRequestDetail
                                        {
                                            Quantity = changeInQuantity,
                                            ProductId = detailDto.Inventory.Batch.ProductId,
                                            CreatedBy = request.CreatedBy
                                        }
                                    }
                                    };

                                    //var serviceResponse = await _goodRequestService.CreateAsync<GoodRequestDTO>(goodRequestDTO);
                                    await _unitOfWork.SaveAsync();

                                    /*var goodRequestData = serviceResponse.Data as GoodRequestDTO;
                                    if (goodRequestData == null)
                                        throw new Exception($"Không thể lấy thông tin GoodRequest từ ServiceResponse. Data: {System.Text.Json.JsonSerializer.Serialize(serviceResponse.Data)}");*/

                                    // Fix for CS0747: Invalid initializer member declarator
                                    // The issue is caused by the incorrect use of an object initializer for `BatchCreateDTOv2`.
                                    // The `new` keyword is missing for the `BatchCreateDTOv2` object inside the `GoodNoteDetailCreateDTO` initializer.

                                    var goodNoteEntity = new GoodNote
                                    {
                                        NoteType = Data.Enum.GoodNoteEnum.Issue,
                                        ShipperName = null,
                                        ReceiverName = null,
                                        Code = await _codeGeneratorService.GenerateCodeAsync(Data.Enum.CodeType.PXDC),
                                        Date = request.Date,
                                        GoodRequestId = goodRequestEntity.Id,
                                    };

                                    await _unitOfWork.GoodNoteRepository.Insert(goodNoteEntity);
                                    await _unitOfWork.SaveAsync();

                                    var goodNoteDetailEntity = new GoodNoteDetail
                                    {
                                        Quantity = changeInQuantity,
                                        Note = "Adjustment for understock from adjustment code:" + inventoryAdjustment.Code,
                                        CreatedBy = request.CreatedBy,
                                        BatchId = detailDto.Inventory.BatchId,
                                        GoodNoteId = goodNoteEntity.Id
                                    };

                                    await _unitOfWork.GoodNoteDetailRepository.Insert(goodNoteDetailEntity);
                                    await _unitOfWork.SaveAsync();

                                    var inventoryUpdate = await _inventoryRepository.GetByCondition(p => p.Id == detailDto.InventoryId);
                                    if (inventoryUpdate == null)
                                        throw new Exception($"Inventory with Id {detailDto.InventoryId} doesnt exist");
                                    inventoryUpdate.CurrentQuantity = inventoryUpdate.CurrentQuantity - changeInQuantity;
                                    if (inventoryUpdate.CurrentQuantity < 0)
                                        throw new Exception($"Inventory với Id {detailDto.InventoryId} không đủ số lượng để xuất");

                                    _inventoryRepository.Update(inventoryUpdate);

                                    /*var goodNoteDTO = new GoodNoteCreateDTO
                                    {
                                        NoteType = Data.Enum.GoodNoteEnum.Issue,
                                        ShipperName = null,
                                        ReceiverName = null,
                                        Code = "PXDC",
                                        Date = request.Date,
                                        GoodRequestId = goodRequestData.Id,
                                        GoodNoteDetails = new List<GoodNoteDetailCreateDTO>
                                        {
                                            new GoodNoteDetailCreateDTO
                                            {
                                                Quantity = changeInQuantity,
                                                Note = "Adjustment for understock",
                                                CreatedBy = request.CreatedBy,
                                                NewBatch = new BatchCreateDTOv2
                                                {
                                                    ProductId = detailDto.Inventory.Batch.ProductId,
                                                    Name = detailDto.Inventory.Batch.Name,
                                                    Code = detailDto.Inventory.Batch.Code,
                                                }
                                            }
                                        }
                                    };*/
                                    /*var goodNote = await _goodNoteService.CreateReceiveNoteAsync(goodNoteDTO);
                                    var goodNoteData = goodNote.Data as GoodNoteDTO;
                                    if (goodNoteData == null)
                                        throw new Exception($"Không thể lấy thông tin GoodNote từ ServiceResponse. Data: {System.Text.Json.JsonSerializer.Serialize(goodNote.Data)}");*/

                                    detailDto.Status = Data.Enum.InventoryCountDetailStatus.Balanced;
                                    break;
                                }
                            default:
                                continue;
                        }
                        /*var goodRequestDTO = new GoodRequestCreateDTO
                        {
                            Note = request.Note,
                            RequestType = Data.Enum.GoodRequestEnum.Transfer,
                            WarehouseId = request.WarehouseId,
                            RequestedWarehouseId = request.WarehouseId,
                            CreatedBy = request.CreatedBy,
                            GoodRequestDetails = new List<GoodRequestDetailDTO>
                            {
                                new GoodRequestDetailDTO
                                {
                                    Quantity = changeInQuantity,

                                    CreatedBy = request.CreatedBy
                                }
                            }
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
                        };*/
                        /*var inventory = await _inventoryRepository.GetByCondition(p => p.Id == detailDto.InventoryId);
                        if (inventory == null)
                            throw new Exception($"Inventory với Id {detailDto.InventoryId} không tồn tại");
                        if (inventory.WarehouseId != request.WarehouseId)
                            throw new Exception($"Inventory với Id {detailDto.InventoryId} không thuộc Warehouse với Id {request.WarehouseId}");
                        var batch = await _batchRepository.GetByCondition(p => p.Id == inventory.BatchId);
                        if (batch == null)
                            throw new Exception($"Batch với Id {inventory.BatchId} không tồn tại");
                        if (detailDto.ProductId != batch.ProductId)
                            throw new Exception($"Batch với Id {inventory.BatchId} không thuộc Product với Id {detailDto.ProductId}");*/


                        //await _inventoryAdjustmentDetailRepository.Insert(inventoryAdjustmentDetail);
                        //await _unitOfWork.SaveAsync();

                        //await _inventoryAdjustmentDetailRepository.Insert(inventoryAdjustmentDetail);
                        await _unitOfWork.SaveAsync();
                    }
                    scope.Complete();
                    return _mapper.Map<InventoryAdjustmentDTO>(inventoryAdjustment);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi thêm InventoryAdjustment: {ex.Message}");
                }

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
                includeProperties: "InventoryAdjustmentDetails,Warehouse," +
                "InventoryAdjustmentDetails.Inventory.Batch.Product.ProductType.Category," +
                "InventoryAdjustmentDetails.Inventory.Batch.Product.Brand," +
                "InventoryAdjustmentDetails.Inventory.Batch.Product.Unit");

            var mappedResults = _mapper.Map<List<InventoryAdjustmentDTO>>(results);

            foreach (var dto in mappedResults)
            {
                if (!string.IsNullOrEmpty(dto.CreatedBy))
                {
                    var account = await _unitOfWork.AccountRepository.GetByCondition(a => a.Id == dto.CreatedBy, "Profile,AccountGroups,AccountGroups.Group");
                    if (account != null)
                    {
                        dto.CreatedByAvatarUrl = account.Profile!.AvatarUrl;
                        dto.CreatedByFullName = $"{account.Profile.FirstName} {account.Profile.LastName}";
                        dto.CreatedByGroup = account.AccountGroups.FirstOrDefault()?.Group?.Name;
                    }
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
