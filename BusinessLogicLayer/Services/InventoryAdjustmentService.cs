﻿using AutoMapper;
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
                        throw new Exception("Ngày không được ở tương lai");
                    var warehouse = await _warehouseRepository.GetByCondition(p => p.Id == request.WarehouseId);
                    if (warehouse == null)
                        throw new Exception("Kho không tồn tại");
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
                        throw new Exception("Loại tài liệu không hợp lệ");
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
                                        throw new Exception($"Inventory với Id {detailDto.InventoryId} không thuộc kho với Id {request.WarehouseId}");

                                    inventoryAdjustmentDetail.NewQuantity = inventory.CurrentQuantity + detailDto.ChangeInQuantity;
                                    //inventoryAdjustmentDetail.Inventory = null;

                                    await _inventoryAdjustmentDetailRepository.Insert(inventoryAdjustmentDetail);
                                    await _unitOfWork.SaveAsync();

                                    var goodRequestEntity = new GoodRequest
                                    {
                                        Note = "Tạo từ phiếu điều chỉnh với mã: " + inventoryAdjustment.Code,
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
                                        Note = "Phiếu nhập điều chỉnh từ phiếu điều chỉnh với mã: " + inventoryAdjustment.Code,
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
                                            throw new Exception($"Inventory với Id {detailDto.InventoryId} không thuộc kho với Id {request.WarehouseId}");

                                        inventoryAdjustmentDetail.NewQuantity = inventory.CurrentQuantity + detailDto.ChangeInQuantity;

                                        await _inventoryAdjustmentDetailRepository.Insert(inventoryAdjustmentDetail);
                                        await _unitOfWork.SaveAsync();

                                        var goodRequestEntity = new GoodRequest
                                        {
                                            Note = "Tạo từ phiếu điều chỉnh với mã: " + inventoryAdjustment.Code,
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
                                            Note = "Phiếu xuất điều chỉnh từ phiếu điều chỉnh với mã: " + inventoryAdjustment.Code,
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
                                throw new Exception("Số lượng thay đổi không hợp lệ");
                        }
                        //await _genericRepository.Insert(inventoryAdjustment);
                        //await _unitOfWork.SaveAsync();
                    }
                    scope.Complete();
                    return _mapper.Map<InventoryAdjustmentDTO>(inventoryAdjustment);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi thêm Phiếu điều chỉnh: {ex.Message}");
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
                        throw new Exception("Ngày không được ở tương lai");

                    var warehouse = await _warehouseRepository.GetByCondition(p => p.Id == request.WarehouseId);
                    if (warehouse == null)
                        throw new Exception("Kho không tồn tại");

                    var inventoryCount = await _inventoryCountRepository.GetByCondition(x => x.Id == request.InventoryCountId,
                        includeProperties: "InventoryCheckDetails,InventoryCheckDetails.Inventory.Batch.Product");
                    if (inventoryCount == null)
                        throw new Exception("Phiếu kiểm kê không tồn tại");
                    if (inventoryCount.Status == Data.Enum.InventoryCountStatus.Adjusted)
                        throw new Exception("Phiếu kiểm kê này đã được điều chỉnh");

                    inventoryCount.Status = Data.Enum.InventoryCountStatus.Adjusted;
                    _inventoryCountRepository.Update(inventoryCount);
                    await _unitOfWork.SaveAsync();




                    var inventoryAdjustment = _mapper.Map<InventoryAdjustment>(request);
                    inventoryAdjustment.WarehouseId = request.WarehouseId;
                    inventoryAdjustment.Code = await _codeGeneratorService.GenerateCodeAsync(Data.Enum.CodeType.PDC);

                    if (request.DocumentType.HasValue && (request.DocumentType != Data.Enum.DocumentType.GoodNote && request.DocumentType != Data.Enum.DocumentType.InventoryCount))
                        throw new Exception("Loại tài liệu không hợp lệ");
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
                                        Note = "Tạo từ phiếu điều chỉnh với mã code code: " + inventoryAdjustment.Code,
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
                                        Note = "Điều chỉnh do hàng thực tế nhiều hơn số hàng trong dữ liệu, tạo từ phiếu điều chỉnh với mã: " + inventoryAdjustment.Code,
                                        CreatedBy = request.CreatedBy,
                                        BatchId = detailDto.Inventory.BatchId,
                                        GoodNoteId = goodNoteEntity.Id
                                    };

                                    await _unitOfWork.GoodNoteDetailRepository.Insert(goodNoteDetailEntity);
                                    await _unitOfWork.SaveAsync();

                                    var inventoryUpdate = await _inventoryRepository.GetByCondition(p => p.Id == detailDto.InventoryId);
                                    if (inventoryUpdate == null)
                                        throw new Exception($"Inventory với Id {detailDto.InventoryId} không tồn tại");
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
                                        Note = "Tạo từ phiếu điều chỉnh với mã code: " + inventoryAdjustment.Code,
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
                                    await _unitOfWork.GoodRequestRepository.Insert(goodRequestEntity);
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
                                        Note = "Điều chỉnh do hàng thực tế ít hơn số hàng trong dữ liệu, tạo từ phiếu điều chỉnh với mã: " + inventoryAdjustment.Code,
                                        CreatedBy = request.CreatedBy,
                                        BatchId = detailDto.Inventory.BatchId,
                                        GoodNoteId = goodNoteEntity.Id
                                    };

                                    await _unitOfWork.GoodNoteDetailRepository.Insert(goodNoteDetailEntity);
                                    await _unitOfWork.SaveAsync();

                                    var inventoryUpdate = await _inventoryRepository.GetByCondition(p => p.Id == detailDto.InventoryId);
                                    if (inventoryUpdate == null)
                                        throw new Exception($"Inventory với Id {detailDto.InventoryId} không tồn tại");
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
                Message = "Lấy thành công!",
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
                orderBy: q => q.OrderByDescending(x => x.CreatedTime),
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
                Message = "Tìm kiếm thành công!",
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
