using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Enum;
using Data.Model.Request.GoodNote;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.UnitOfWork;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services
{
    public class GoodNoteService : GenericService<GoodNote>, IGoodNoteService
    {
        private readonly IGoodNoteRepository _goodNoteRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseService _firebaseService;
        private readonly ICodeGeneratorService _codeGeneratorService;

        public GoodNoteService(IGenericRepository<GoodNote> genericRepository, IUnitOfWork unitOfWork, IMapper mapper, IFirebaseService firebaseService, ICodeGeneratorService codeGeneratorService)
            : base(genericRepository, mapper, unitOfWork)
        {
            _goodNoteRepository = unitOfWork.GoodNoteRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _firebaseService = firebaseService;
            _codeGeneratorService = codeGeneratorService;
        }
        public async Task<ServiceResponse> SearchGoodNotes(int? pageIndex = null, int? pageSize = null,
                                                            string? keyword = null, GoodNoteEnum? goodNoteType = null,
                                                            GoodNoteStatusEnum? status = null, string? requestedWarehouseId = null)
        {
            try
            {
                //Filter GoodNotes based on the provided criteria
                Expression<Func<GoodNote, bool>> goodNoteFilter = g =>
                    (string.IsNullOrEmpty(keyword) ||
                        (g.ShipperName != null && g.ShipperName.Contains(keyword)) ||
                        (g.ReceiverName != null && g.ReceiverName.Contains(keyword)) ||
                        (g.Code != null && g.Code.Contains(keyword)) ||
                        (g.GoodRequest.RequestedWarehouse != null && g.GoodRequest.RequestedWarehouse.Name.Contains(keyword)) ||
                        (g.GoodRequest.Warehouse != null && g.GoodRequest.Warehouse.Name.Contains(keyword))) &&
                    (!goodNoteType.HasValue || g.NoteType == goodNoteType.Value) &&
                    (!status.HasValue || g.Status == status.Value) &&
                    (string.IsNullOrEmpty(requestedWarehouseId) || g.GoodRequest.RequestedWarehouseId == requestedWarehouseId || (g.GoodRequest.WarehouseId == requestedWarehouseId /*&& g.NoteType == GoodNoteEnum.Receive*/));

                //Use search method to get paged results
                var pagedGoodNotes = await _unitOfWork.GoodNoteRepository.Search(
                    filter: goodNoteFilter,
                    includeProperties: "GoodRequest,GoodRequest.Warehouse,GoodRequest.RequestedWarehouse,GoodRequest.Partner",
                    orderBy: q => q.OrderByDescending(g => g.CreatedTime),
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );

                //Get total number of records (count by GoodNote)
                int totalRecords = await _unitOfWork.GoodNoteRepository.Count(goodNoteFilter);
                int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? 5));

                //Get list of GoodNoteIds from paged results
                var goodNoteIds = pagedGoodNotes.Select(g => g.Id).ToList();

                //Get GoodNoteDetails for the GoodNotes
                Expression<Func<GoodNoteDetail, bool>> detailFilter = d => goodNoteIds.Contains(d.GoodNoteId);

                var details = await _unitOfWork.GoodNoteDetailRepository.Search(
                    filter: detailFilter,
                    includeProperties: "GoodNote,GoodNote.GoodRequest,GoodNote.GoodRequest.Warehouse," +
                                       "GoodNote.GoodRequest.RequestedWarehouse," +
                                       "Batch,Batch.Product,Batch.Product.Unit,Batch.Product.Brand"
                );

                //Group GoodNotes and map to DTO
                var groupedResults = pagedGoodNotes.Select(g => new GoodNoteDTO
                {
                    Id = g.Id,
                    ReceiverName = g.ReceiverName,
                    ShipperName = g.ShipperName,
                    NoteType = g.NoteType,
                    Status = g.Status,
                    GoodRequest = _mapper.Map<GoodRequestOfGoodNoteDTO>(g.GoodRequest),
                    Code = g.Code,
                    Date = g.Date,
                    CreatedTime = g.CreatedTime.ToString(),
                    CreatedBy = g.CreatedBy,
                    GoodNoteDetails = _mapper.Map<List<GoodNoteDetailDTO>>(details.Where(d => d.GoodNoteId == g.Id).ToList())
                }).ToList();

                foreach (var item in groupedResults)
                {
                    var createdByAccount = await _unitOfWork.AccountRepository.GetByCondition(a => a.Id == item.CreatedBy, "Profile,AccountGroups,AccountGroups.Group");
                    if (createdByAccount != null)
                    {
                        item.CreatedByAvatarUrl = createdByAccount.Profile!.AvatarUrl;
                        item.CreatedByFullName = $"{createdByAccount.Profile.FirstName} {createdByAccount.Profile.LastName}";
                        item.CreatedByGroup = createdByAccount.AccountGroups.FirstOrDefault()?.Group?.Name;
                    }
                }   
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Tìm kiếm thành công!",
                    Data = new
                    {
                        TotalRecords = totalRecords,
                        TotalPages = totalPages,
                        PageIndex = pageIndex ?? 1,
                        PageSize = pageSize ?? 5,
                        Records = groupedResults/*.OrderByDescending(g => g.CreatedTime).ToList()*/
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Error,
                    Message = "Đã xảy ra lỗi khi tìm kiếm GoodNotes." + ex,
                    Data = null
                };
            }
        }
        public async Task<ServiceResponse> GetById(string id)
        {
            var entities = await _unitOfWork.GoodNoteDetailRepository.Search(g => g.GoodNoteId == id, includeProperties: "GoodNote," +
                                                                                                                         "GoodNote.GoodRequest," +
                                                                                                                         "GoodNote.GoodRequest.Warehouse," +
                                                                                                                         "GoodNote.GoodRequest.RequestedWarehouse," +
                                                                                                                         "GoodNote.GoodRequest.Partner," +
                                                                                                                         "Batch," +
                                                                                                                         "Batch.Product," +
                                                                                                                         "Batch.Product.Unit," +
                                                                                                                         "Batch.Product.Brand");
            if (entities.Count() == 0)  //Check if no details found for the GoodNote
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Không tìm thấy phiếu kho!",
                    Data = id
                };
            }

            var groupedResult = new GoodNoteDTO
            {
                Id = entities.First().GoodNote.Id,
                ReceiverName = entities.First().GoodNote.ReceiverName,
                ShipperName = entities.First().GoodNote.ShipperName,
                NoteType = entities.First().GoodNote.NoteType,
                Status = entities.First().GoodNote.Status,
                GoodRequest = _mapper.Map<GoodRequestOfGoodNoteDTO>(entities.First().GoodNote.GoodRequest),
                Code = entities.First().GoodNote.Code,
                Date = entities.First().GoodNote.Date,
                CreatedTime = entities.First().GoodNote.CreatedTime.ToString(),
                CreatedBy = entities.First().GoodNote.CreatedBy,
                GoodNoteDetails = _mapper.Map<List<GoodNoteDetailDTO>>(entities)
            };

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Tìm phiếu kho thành công!",
                Data = groupedResult
            };


        }
        public async Task<ServiceResponse> CreateReceiveNoteAsync(GoodNoteCreateDTO request)
        {
            var serviceResponse = new ServiceResponse();
            try
            {
                //Validate
                //Check GoodRequest
                GoodRequest goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(x => x.Id == request.GoodRequestId);
                if (!string.IsNullOrEmpty(request.GoodRequestId))
                {
                    //goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(x => x.Id == request.GoodRequestId);
                    if (goodRequest == null)
                        return Fail("Không tìm thấy yêu cầu kho.", request.GoodRequestId);
                    if (goodRequest.Status == GoodRequestStatusEnum.Pending)
                    {
                        return Fail("Không thể tạo phiếu cho yêu cầu chưa được chấp thuận.", request.GoodRequestId);
                    }
                    if (goodRequest.Status == GoodRequestStatusEnum.Completed)
                    {
                        return Fail("Không thể tạo phiếu cho yêu cầu đã hoàn thành.", request.GoodRequestId);
                    }
                    if (goodRequest.Status == GoodRequestStatusEnum.Rejected)
                    {
                        return Fail("Không thể tạo phiếu cho yêu cầu đã bị từ chối.", request.GoodRequestId);
                    }
                    if (goodRequest.RequestType != GoodRequestEnum.Return && goodRequest.RequestType != GoodRequestEnum.Receive)
                    {
                        return Fail("Yêu cầu kho không hợp lệ.", request.GoodRequestId);
                    }
                }
                // Kiểm tra chi tiết có lô
                if (request.GoodNoteDetails == null || !request.GoodNoteDetails.Any())
                    return Fail("Danh sách chi tiết phiếu kho không được để trống.");

                foreach (var detail in request.GoodNoteDetails)
                {
                    if (detail.NewBatch == null)
                    {
                        return Fail("Thiếu thông tin lô trong chi tiết phiếu kho.", null!);
                    }
                }

                // Tạo phiếu nhập kho
                var goodNote = _mapper.Map<GoodNote>(request);
                //goodNote.Date = DateTime.Now;
                goodNote.CreatedTime = DateTime.Now;
                goodNote.Status = GoodNoteStatusEnum.Completed;
                // Sinh mã cho phiếu nhập kho
                goodNote.Code = await _codeGeneratorService.GenerateCodeAsync(CodeType.PN);

                // Kiểm tra mã phiếu trùng
                var existingNote = await _unitOfWork.GoodNoteRepository.GetByCondition(x => x.Code == goodNote.Code);
                if (existingNote != null)
                    return Fail("Mã phiếu đã tồn tại.", request.Code!);

                //bắt đầu transaction trước khi đụng đến db
                await _unitOfWork.BeginTransactionAsync();

                await _unitOfWork.GoodNoteRepository.Add(goodNote);

                // Tạo batch và chi tiết
                var goodNoteDetails = new List<GoodNoteDetail>();

                foreach (var detailDto in request.GoodNoteDetails)
                {
                    // Tạo chi tiết
                    var detail = _mapper.Map<GoodNoteDetail>(detailDto);
                    detail.GoodNoteId = goodNote.Id;
                    detail.CreatedTime = DateTime.Now;
                    if (detailDto.NewBatch != null)
                    {
                        //Validate quantity
                        var productExists = await _unitOfWork.ProductRepository.GetByCondition(x => x.Id == detailDto.NewBatch.ProductId, includeProperties: "Unit");
                        if (productExists == null)
                        {
                            throw new Exception($"Không tìm thấy sản phẩm với Id {detailDto.NewBatch.ProductId}!");
                        }
                        //Validate quantity based on Unit type
                        if (productExists.Unit.Type == UnitEnum.Int)
                        {
                            if (detail.Quantity <= 0 || detail.Quantity % 1 != 0)
                            {
                                throw new Exception($"Sản phẩm {productExists.Sku} yêu cầu số lượng là số nguyên dương!");
                            }
                        }
                        else
                        {
                            if (detail.Quantity <= 0)
                            {
                                throw new Exception($"Sản phẩm {productExists.Sku} yêu cầu số lượng lớn hơn 0!");
                            }
                        }

                        //Create new batch
                        var batch = _mapper.Map<Batch>(detailDto.NewBatch);
                        //Generate batch code
                        batch.Code = await _codeGeneratorService.GenerateBatchCodeByProductAsync(batch.ProductId);
                        batch.Name = goodNote.Code;
                        if (request.Date.HasValue)
                        {
                            batch.InboundDate = DateOnly.FromDateTime(request.Date.Value);
                        }
                        else
                        {
                            // Use current date if not provided
                            batch.InboundDate = DateOnly.FromDateTime(DateTime.Now);
                        }
                        batch.CreatedBy =  goodNote.CreatedBy;
                        await _unitOfWork.BatchRepository.Add(batch);
                        await _unitOfWork.SaveAsync();
                        detail.BatchId = batch.Id;
                    }
                    detail.CreatedBy = goodNote.CreatedBy;
                    await _unitOfWork.GoodNoteDetailRepository.Add(detail);

                    goodNoteDetails.Add(detail);
                }

                //Update inventory
                //Assign goodRequest to goodNote
                goodNote.GoodRequest = goodRequest;
                await UpdateInventories(goodNote, goodNoteDetails);

                //update goodRequest status
                if (goodRequest != null)
                {
                    //goodRequest.Status = GoodRequestStatusEnum.Approved;
                    //_unitOfWork.GoodRequestRepository.Update(goodRequest);

                    //Notify user who created the request
                    if (!string.IsNullOrEmpty(goodRequest.CreatedBy))
                    {
                        await _firebaseService.SendNotificationToUsersAsync(
                            new List<string> { goodRequest.CreatedBy },
                            "Thông báo",
                            $"Phiếu nhập kho {goodNote.Code} đã được tạo thành công từ yêu cầu kho {goodRequest.Code}.",
                            NotificationType.GOOD_REQUEST_APPROVED,
                            goodRequest.RequestedWarehouseId
                        );
                    }
                }
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                serviceResponse.Status = SRStatus.Success;
                serviceResponse.Message = "Phiếu nhập kho được tạo thành công!";
                serviceResponse.Data = goodNote.Id;
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Fail(ex.Message, null!);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                serviceResponse.Status = SRStatus.Error;
                serviceResponse.Message = $"Lỗi khi tạo phiếu kho: {ex.Message}";
            }

            return serviceResponse;
        }

        private async Task UpdateInventories(GoodNote goodNote, List<GoodNoteDetail> details)
        {
            if (goodNote.NoteType != GoodNoteEnum.Receive)
                throw new Exception("Chức năng này chỉ hỗ trợ cho phiếu nhập kho.");

            var requestedWarehouseId = goodNote.GoodRequest?.RequestedWarehouseId
                ?? throw new Exception("Yêu cầu kho chưa có kho nhận.");

            //Kho nhận là warehouseId nếu là yêu cầu chuyển kho
            if(goodNote.GoodRequest?.RequestType == GoodRequestEnum.Transfer)
            {
                requestedWarehouseId = goodNote.GoodRequest?.WarehouseId;
            }
            var batchIds = details.Select(d => d.BatchId).Distinct().ToList();
            var inventories = await _unitOfWork.InventoryRepository.Search(
                i => batchIds.Contains(i.BatchId)
                //includeProperties: "Warehouse"
            );

            foreach (var detail in details)
            {
                var inventory = inventories.FirstOrDefault(i => i.BatchId == detail.BatchId && i.WarehouseId == requestedWarehouseId);

                if (inventory == null)
                {
                    inventory = new Inventory
                    {
                        WarehouseId = requestedWarehouseId,
                        BatchId = detail.BatchId,
                        CurrentQuantity = detail.Quantity
                    };
                    inventory.CreatedBy = goodNote.CreatedBy;
                    await _unitOfWork.InventoryRepository.Add(inventory);
                }
                else
                {
                    inventory.CurrentQuantity += detail.Quantity;
                    _unitOfWork.InventoryRepository.Update(inventory);
                }
            }
        }

        // Helper fail response
        private ServiceResponse Fail(string message, object data = null!)
        {
            return new ServiceResponse
            {
                Status = SRStatus.Error,
                Message = message,
                Data = data
            };
        }

        public async Task<ServiceResponse> CreateIssueNoteAsync(GoodNoteIssueCreateDTO dto, CodeType codeType)
        {
            var serviceResponse = new ServiceResponse();
            try
            {
                //Validate
                var goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(x => x.Id == dto.GoodRequestId, includeProperties: "Warehouse,RequestedWarehouse");
                if (goodRequest == null)
                {
                    serviceResponse.Status = SRStatus.Error;
                    serviceResponse.Message = "Không tìm thấy yêu cầu kho.";
                    //await _unitOfWork.RollbackTransactionAsync();  // Rollback transaction if goodRequest is null
                    return serviceResponse;
                }
                if (goodRequest.RequestType != GoodRequestEnum.Issue && goodRequest.RequestType != GoodRequestEnum.Transfer)
                {
                    serviceResponse.Status = SRStatus.Error;
                    serviceResponse.Message = "Yêu cầu kho không phải là yêu cầu xuất kho hoặc điều chuyển kho.";
                    //await _unitOfWork.RollbackTransactionAsync();  // Rollback transaction if request type is invalid
                    return serviceResponse;
                }
                if (goodRequest.Status != GoodRequestStatusEnum.Approved)
                {
                    serviceResponse.Status = SRStatus.Error;
                    serviceResponse.Message = "Yêu cầu kho chưa được chấp thuận.";
                    //await _unitOfWork.RollbackTransactionAsync();  // Rollback transaction if request is not approved
                    return serviceResponse;
                }

                // Validate quantity for each detail based on Unit.Type
                foreach (var detail in dto.GoodNoteDetails)
                {
                    var product = await _unitOfWork.ProductRepository
                        .GetByCondition(x => x.Id == detail.ProductId, includeProperties: "Unit");

                    if (product == null)
                    {
                        return new ServiceResponse
                        {
                            Status = SRStatus.Error,
                            Message = $"Không tìm thấy sản phẩm với ID {detail.ProductId}.",
                            Data = detail.ProductId
                        };
                    }

                    //If Unit.Type is Int, check if quantity is a positive integer
                    if (product.Unit.Type == UnitEnum.Int)
                    {
                        if (detail.Quantity <= 0 || detail.Quantity % 1 != 0)
                        {
                            return new ServiceResponse
                            {
                                Status = SRStatus.Error,
                                Message = $"Sản phẩm '{product.Sku}' yêu cầu số lượng là số nguyên dương.",
                                Data = detail
                            };
                        }
                    }
                    else //If Unit.Type is not Int, check if quantity is a positive float
                    {
                        if (detail.Quantity <= 0)
                        {
                            return new ServiceResponse
                            {
                                Status = SRStatus.Error,
                                Message = $"Sản phẩm '{product.Sku}' yêu cầu số lượng lớn hơn 0.",
                                Data = detail
                            };
                        }
                    }
                }

                //Check warehouseId
                string? checkWarehouseId = codeType switch
                {
                    CodeType.PXNB => goodRequest.RequestedWarehouseId,
                    CodeType.PX => goodRequest.RequestedWarehouseId,
                    _ => null
                };

                if (string.IsNullOrEmpty(checkWarehouseId))
                {
                    serviceResponse.Status = SRStatus.Error;
                    serviceResponse.Message = "Không tìm thấy kho để kiểm tra và trừ tồn kho.";
                    return serviceResponse;
                }
                //Check inventory
                var checkResult = await CheckInventorySufficientAsync(dto, checkWarehouseId);
                if (!checkResult)
                {
                    serviceResponse.Status = SRStatus.Error;
                    serviceResponse.Message = "Tồn kho không đủ";
                    //await _unitOfWork.RollbackTransactionAsync();  // Rollback transaction if inventory is insufficient
                    return serviceResponse;
                }

                //Begin transaction
                await _unitOfWork.BeginTransactionAsync();

                //Create GoodNote
                var goodNote = await CreateGoodNoteIssueAsync(dto, codeType);

                //Deduct inventory and create good note details
                var deductResult = await DeductInventoryAndCreateDetailsAsync(dto, goodNote.Id, checkWarehouseId);
                if (!deductResult)
                {
                    serviceResponse.Status = SRStatus.Error;
                    serviceResponse.Message = "Lỗi khi trừ tồn kho";
                    await _unitOfWork.RollbackTransactionAsync();  // Rollback transaction if inventory deduction fails
                    return serviceResponse;
                }
                //update goodRequest status if necessary
                if (codeType == CodeType.PXNB && goodRequest.RequestType == GoodRequestEnum.Transfer)
                {
                    goodRequest.Status = GoodRequestStatusEnum.Issued;
                    _unitOfWork.GoodRequestRepository.Update(goodRequest);
                }

                //Save changes
                await _unitOfWork.SaveAsync();

                //Notify users
                var details = await _unitOfWork.GoodNoteDetailRepository.GetDetailsByGoodNoteIdAsync(goodNote.Id);

                var batchMessages = details
                    .Select(x => $"{x.Batch.Code} ({x.Quantity})")
                    .ToList();


                var keeperIds = await _unitOfWork.AccountRepository.GetUserIdsByWarehouseAndGroups(checkWarehouseId, new List<string> { "Thủ kho" });

                await SendIssueNoteNotificationAsync(
                    keeperIds,
                    goodRequest.CreatedBy!,
                    goodNote.Code!,
                    batchMessages,
                    checkWarehouseId
                );

                //Notify receiver if it's a transfer request
                if (codeType == CodeType.PXNB)
                {
                    var receiverIds = await _unitOfWork.AccountRepository.GetUserIdsByWarehouseAndGroups(goodRequest.WarehouseId!, new List<string> { "Thủ kho" });
                    await _firebaseService.SendNotificationToUsersAsync(
                        receiverIds,
                        "Thông báo điều chuyuển kho",
                        $"Phiếu xuất {goodNote.Code} đã xuất các lô: {string.Join(", ", batchMessages)} từ kho {goodRequest.RequestedWarehouse!.Name}",
                        NotificationType.ISSUE_NOTE_CREATED,
                        goodRequest.WarehouseId
                    );
                }
                await _unitOfWork.CommitTransactionAsync();  // Commit transaction if everything is successful

                serviceResponse.Status = SRStatus.Success;
                serviceResponse.Message = "Tạo phiếu xuất kho thành công";
                serviceResponse.Data = goodNote.Id;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();  // Rollback transaction if any error occurs
                serviceResponse.Status = SRStatus.Error;
                serviceResponse.Message = $"Có lỗi: {ex.Message}";
            }

            return serviceResponse;
        }

        private async Task<bool> CheckInventorySufficientAsync(GoodNoteIssueCreateDTO dto, string warehouseId)
        {
            var productGroups = dto.GoodNoteDetails
                .GroupBy(x => x.ProductId)
                .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(x => x.Quantity) })
                .ToList();

            foreach (var product in productGroups)
            {
                var inventories = await _unitOfWork.InventoryRepository.GetAvailableInventoriesAsync(product.ProductId, warehouseId);

                var totalStock = inventories.Sum(x => x.CurrentQuantity);

                if (totalStock < product.TotalQuantity)
                {
                    return false; // Tồn kho không đủ
                }
            }

            return true; // Tồn kho đủ
        }

        private async Task<GoodNote> CreateGoodNoteIssueAsync(GoodNoteIssueCreateDTO dto, CodeType codeType)
        {
            var goodNote = new GoodNote
            {
                Code = await _codeGeneratorService.GenerateCodeAsync(codeType),
                Date = dto.Date /*DateTime.Now*/,
                ReceiverName = dto.ReceiverName,
                ShipperName = dto.ShipperName,
                NoteType = GoodNoteEnum.Issue, // Phiếu xuất kho
                Status = GoodNoteStatusEnum.Completed,
                GoodRequestId = dto.GoodRequestId,
                CreatedBy = dto.CreatedBy,
            };
            // Kiểm tra mã phiếu xuất kho đã tồn tại chưa
            var existingNote = await _unitOfWork.GoodNoteRepository.GetByCondition(x => x.Code == goodNote.Code);
            if (existingNote != null)
            {
                throw new Exception($"Mã phiếu xuất kho {goodNote.Code} đã tồn tại.");
            }
            await _unitOfWork.GoodNoteRepository.Add(goodNote);
            return goodNote;
        }

        private async Task<bool> DeductInventoryAndCreateDetailsAsync(GoodNoteIssueCreateDTO dto, string goodNoteId, string warehouseId)
        {
            foreach (var detailDto in dto.GoodNoteDetails)
            {
                float remainQuantity = detailDto.Quantity;

                var availableInventories = await _unitOfWork.InventoryRepository.GetAvailableInventoriesAsync(detailDto.ProductId, warehouseId);

                foreach (var inventory in availableInventories)
                {
                    if (remainQuantity <= 0) break;

                    var usedQuantity = Math.Min(inventory.CurrentQuantity, remainQuantity);

                    var detail = new GoodNoteDetail
                    {
                        Quantity = usedQuantity,
                        Note = detailDto.Note,
                        GoodNoteId = goodNoteId,
                        BatchId = inventory.BatchId,
                        CreatedBy = dto.CreatedBy,
                    };
                    detail.CreatedBy = dto.CreatedBy;
                    await _unitOfWork.GoodNoteDetailRepository.Add(detail);

                    inventory.CurrentQuantity -= usedQuantity;
                    _unitOfWork.InventoryRepository.Update(inventory);

                    remainQuantity -= usedQuantity;
                }

                if (remainQuantity > 0)
                {
                    return false; // Không thể trừ hết tồn kho
                }
            }

            return true; // Trừ tồn kho và tạo chi tiết thành công
        }
        public async Task SendIssueNoteNotificationAsync(List<string> warehouseKeeperIds, string requesterId, string issueNoteCode, List<string> batchMessages, string requestedWarehouseId)
        {
            //var keeperMessage = $"Phiếu xuất {issueNoteCode} đã xuất các lô: {string.Join(", ", batchMessages)}";
            var requesterMessage = $"Yêu cầu của bạn đã được tạo phiếu xuất: {issueNoteCode}";

            //await _firebaseService.SendNotificationToUsersAsync(
            //    warehouseKeeperIds,
            //    "Thông báo xuất kho",
            //    keeperMessage,
            //    NotificationType.ISSUE_NOTE_CREATED,
            //    requestedWarehouseId
            //);

            await _firebaseService.SendNotificationToUsersAsync(
                new List<string> { requesterId },
                "Thông báo yêu cầu kho",
                requesterMessage,
                NotificationType.GOOD_REQUEST_APPROVED,
                requestedWarehouseId
            );
        }

        public async Task<ServiceResponse> CreateReceiveNoteWithExistingBatchAsync(GoodNoteCreateDTOv2 request, CodeType codeType)
        {
            var serviceResponse = new ServiceResponse();
            try
            {
                //Validate
                //Validate GoodRequest
                GoodRequest goodRequest = null!;
                if (!string.IsNullOrEmpty(request.GoodRequestId))
                {
                    goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(x => x.Id == request.GoodRequestId);
                    if (goodRequest == null)
                        return Fail("Không tìm thấy yêu cầu kho.", request.GoodRequestId);

                    if (goodRequest.Status == GoodRequestStatusEnum.Pending)
                        return Fail("Không thể tạo phiếu cho yêu cầu chưa được chấp thuận.", request.GoodRequestId);
                    if (goodRequest.Status == GoodRequestStatusEnum.Completed)
                        return Fail("Không thể tạo phiếu cho yêu cầu đã hoàn thành.", request.GoodRequestId);
                    if (goodRequest.Status == GoodRequestStatusEnum.Rejected)
                        return Fail("Không thể tạo phiếu cho yêu cầu đã bị từ chối.", request.GoodRequestId);
                    if(codeType == CodeType.PNNB && goodRequest.Status != GoodRequestStatusEnum.Issued && goodRequest.RequestType == GoodRequestEnum.Transfer)
                        return Fail("Không thể tạo phiếu nhập nội bộ cho yêu cầu điều chuyển chưa được xuất kho.", request.GoodRequestId);

                    if (goodRequest.RequestType != GoodRequestEnum.Transfer
                        && goodRequest.RequestType != GoodRequestEnum.Return)
                        return Fail("Yêu cầu kho không hợp lệ.", request.GoodRequestId);
                }

                if (request.GoodNoteDetails == null || !request.GoodNoteDetails.Any())
                    return Fail("Danh sách chi tiết phiếu kho không được để trống.");

                //Validate batch in each detail
                foreach (var detail in request.GoodNoteDetails)
                {
                    if (string.IsNullOrEmpty(detail.BatchId))
                        return Fail("Thiếu BatchId trong chi tiết phiếu kho.", null!);

                    var existingBatch = await _unitOfWork.BatchRepository.GetByCondition(x => x.Id == detail.BatchId);
                    if (existingBatch == null)
                        return Fail($"Không tìm thấy lô hàng với Id {detail.BatchId}.", null!);
                    

                    //Get product by BatchId
                    var product = await _unitOfWork.ProductRepository.GetByCondition(p => p.Id == existingBatch.ProductId, includeProperties: "Unit");
                    if (product == null)
                        return Fail($"Sản phẩm trong Batch {existingBatch.Name} không tồn tại.");
                    //Validate quantity based on Unit type of product
                    if (product.Unit.Type == UnitEnum.Int)
                    {
                        if (detail.Quantity <= 0 || detail.Quantity % 1 != 0)
                        {
                            return Fail($"Sản phẩm {product.Sku} yêu cầu số lượng là số nguyên dương.");
                        }
                    }
                    else
                    {
                        if (detail.Quantity <= 0)
                        {
                            return Fail($"Sản phẩm {product.Sku} yêu cầu số lượng lớn hơn 0.");
                        }
                    }
                }

                //Create GoodNote
                var goodNote = _mapper.Map<GoodNote>(request);
                //goodNote.Date = DateTime.Now;   
                goodNote.CreatedTime = DateTime.Now;
                goodNote.Status = GoodNoteStatusEnum.Completed;
                goodNote.Code = await _codeGeneratorService.GenerateCodeAsync(codeType); // codetype is PNNB or PN for Internal Receive Note or Return Receive Note

                var existingNote = await _unitOfWork.GoodNoteRepository.GetByCondition(x => x.Code == goodNote.Code);
                if (existingNote != null)
                    return Fail("Mã phiếu đã tồn tại.", goodNote.Code);

                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.GoodNoteRepository.Add(goodNote);

                var goodNoteDetails = new List<GoodNoteDetail>();

                foreach (var detailDto in request.GoodNoteDetails)
                {
                    var detail = _mapper.Map<GoodNoteDetail>(detailDto);
                    detail.GoodNoteId = goodNote.Id;
                    detail.CreatedTime = DateTime.Now;
                    detail.CreatedBy = goodNote.CreatedBy;
                    await _unitOfWork.GoodNoteDetailRepository.Add(detail);
                    goodNoteDetails.Add(detail);
                }

                goodNote.GoodRequest = goodRequest!;

                await UpdateInventories(goodNote, goodNoteDetails);

                await _unitOfWork.SaveAsync();

                if (goodRequest.RequestType == GoodRequestEnum.Return)
                {
                    if (goodRequest != null && !string.IsNullOrEmpty(goodRequest.CreatedBy))
                    {
                        await _firebaseService.SendNotificationToUsersAsync(
                            new List<string> { goodRequest.CreatedBy },
                            "Thông báo",
                            $"Phiếu nhập kho {goodNote.Code} đã được tạo từ yêu cầu kho {goodRequest.Code}.",
                            NotificationType.GOOD_REQUEST_APPROVED,
                            goodRequest.RequestedWarehouseId
                        );
                    }
                } 

                if (goodRequest!.RequestType == GoodRequestEnum.Transfer)
                {
                    if (goodRequest != null && !string.IsNullOrEmpty(goodRequest.CreatedBy))
                    {
                        await _firebaseService.SendNotificationToUsersAsync(
                            new List<string> { goodRequest.CreatedBy },
                            "Thông báo",
                            $"Phiếu nhập kho {goodNote.Code} đã được tạo từ yêu cầu kho {goodRequest.Code}.",
                            NotificationType.GOOD_REQUEST_APPROVED,
                            goodRequest.WarehouseId
                        );
                    }
                }
                //// Notify warehouse keepers
                //var batchMessages = goodNoteDetails
                //    .Select(x => $"{x.Batch.Code} ({x.Quantity})")
                //    .ToList();
                //var keeperIds = await _unitOfWork.AccountRepository.GetUserIdsByWarehouseAndGroups(goodRequest!.RequestedWarehouseId!, new List<string> { "Thủ kho" });
                //await _firebaseService.SendNotificationToUsersAsync(
                //    keeperIds,
                //    "Thông báo nhập kho",
                //    $"Phiếu nhập {goodNote.Code} đã nhập các lô: {string.Join(", ", batchMessages)}",
                //    NotificationType.RECEIVE_NOTE_CREATED,
                //    goodRequest.RequestedWarehouseId
                //);

                await _unitOfWork.CommitTransactionAsync();

                serviceResponse.Status = SRStatus.Success;
                serviceResponse.Message = "Phiếu nhập kho được tạo thành công!";
                serviceResponse.Data = goodNote.Id;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Fail($"Lỗi khi tạo phiếu kho: {ex.Message}", null!);
            }

            return serviceResponse;
        }

    }
}
