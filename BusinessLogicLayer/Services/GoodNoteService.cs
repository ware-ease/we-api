using AutoMapper;
using Azure;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Enum;
using Data.Model.Request.GoodNote;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services
{
    public class GoodNoteService : GenericService<GoodNote>, IGoodNoteService
    {
        private readonly IGoodNoteRepository _goodNoteRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseService _firebaseService;


        public GoodNoteService(IGenericRepository<GoodNote> genericRepository, IUnitOfWork unitOfWork, IMapper mapper, IFirebaseService firebaseService)
            : base(genericRepository, mapper, unitOfWork)
        {
            _goodNoteRepository = unitOfWork.GoodNoteRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _firebaseService = firebaseService;
        }
        public async Task<ServiceResponse> SearchGoodNotes(int? pageIndex = null, int? pageSize = null,
                                                                    string? keyword = null, GoodNoteEnum? goodNoteType = null,
                                                                    GoodNoteStatusEnum? status = null, string? requestedWarehouseId = null)
        {
            try
            {
                // Bước 1: Lọc danh sách GoodNote trước
                Expression<Func<GoodNote, bool>> goodNoteFilter = g =>
                    (string.IsNullOrEmpty(keyword) ||
                        (g.ShipperName != null && g.ShipperName.Contains(keyword)) ||
                        (g.ReceiverName != null && g.ReceiverName.Contains(keyword)) ||
                        (g.Code != null && g.Code.Contains(keyword)) ||
                        (g.GoodRequest.RequestedWarehouse != null && g.GoodRequest.RequestedWarehouse.Name.Contains(keyword)) ||
                        (g.GoodRequest.Warehouse != null && g.GoodRequest.Warehouse.Name.Contains(keyword))) &&
                    (!goodNoteType.HasValue || g.NoteType == goodNoteType.Value) &&
                    (!status.HasValue || g.Status == status.Value) &&
                    (string.IsNullOrEmpty(requestedWarehouseId) || g.GoodRequest.RequestedWarehouseId == requestedWarehouseId);

                // Dùng Search
                var pagedGoodNotes = await _unitOfWork.GoodNoteRepository.Search(
                    filter: goodNoteFilter,
                    includeProperties: "GoodRequest,GoodRequest.Warehouse,GoodRequest.RequestedWarehouse,GoodRequest.Partner",
                    orderBy: q => q.OrderByDescending(g => g.CreatedTime),
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );

                // Lấy tổng số bản ghi (đếm theo GoodNote)
                int totalRecords = await _unitOfWork.GoodNoteRepository.Count(goodNoteFilter);
                int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? 5));

                // Lấy danh sách GoodNoteId để truy vấn chi tiết
                var goodNoteIds = pagedGoodNotes.Select(g => g.Id).ToList();

                // Bước 2: Lấy danh sách GoodNoteDetail theo danh sách GoodNoteId
                Expression<Func<GoodNoteDetail, bool>> detailFilter = d => goodNoteIds.Contains(d.GoodNoteId);

                var details = await _unitOfWork.GoodNoteDetailRepository.Search(
                    filter: detailFilter,
                    includeProperties: "GoodNote,GoodNote.GoodRequest,GoodNote.GoodRequest.Warehouse," +
                                       "GoodNote.GoodRequest.RequestedWarehouse," +
                                       "Batch,Batch.Product,Batch.Product.Unit,Batch.Product.Brand"
                );

                // Bước 3: Nhóm dữ liệu theo GoodNote
                var groupedResults = pagedGoodNotes.Select(g => new GoodNoteDTO
                {
                    Id = g.Id,
                    ReceiverName = g.ReceiverName,
                    ShipperName = g.ShipperName,
                    NoteType = g.NoteType,
                    Status = g.Status,
                    //GoodRequestId = g.GoodRequestId,
                    //GoodRequestCode = g.GoodRequest?.Code,
                    //RequestedWarehouseName = g.GoodRequest?.RequestedWarehouse?.Name,
                    GoodRequest = _mapper.Map<GoodRequestOfGoodNoteDTO>(g.GoodRequest),
                    Code = g.Code,
                    Date = g.Date,
                    CreatedTime = g.CreatedTime.ToString(),
                    CreatedBy = g.CreatedBy,
                    GoodNoteDetails = _mapper.Map<List<GoodNoteDetailDTO>>(details.Where(d => d.GoodNoteId == g.Id).ToList())
                }).ToList();

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
                        Records = groupedResults
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Error,
                    Message = "An error occurred while searching GoodNotes. Please try again later." + ex,
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
            if (entities.Count() == 0)  // Chỉ đếm một lần, không gọi truy vấn lại
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
                //GoodRequestId = entities.First().GoodNote.GoodRequestId,
                //GoodRequestCode = entities.First().GoodNote.GoodRequest.Code,
                //RequestedWarehouseName = entities.First().GoodNote.GoodRequest.RequestedWarehouse?.Name,
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

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Kiểm tra yêu cầu kho nếu có
                GoodRequest goodRequest = null;
                if (!string.IsNullOrEmpty(request.GoodRequestId))
                {
                    goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(x => x.Id == request.GoodRequestId);
                    if (goodRequest == null)
                        return Fail("Không tìm thấy yêu cầu kho.", request.GoodRequestId);
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

                // Kiểm tra mã phiếu trùng
                var existingNote = await _unitOfWork.GoodNoteRepository.GetByCondition(x => x.Code == request.Code);
                if (existingNote != null)
                    return Fail("Mã phiếu đã tồn tại.", request.Code);

                // Kiểm tra chi tiết có lô
                if (request.GoodNoteDetails == null || !request.GoodNoteDetails.Any())
                    return Fail("Danh sách chi tiết phiếu kho không được để trống.");

                foreach (var detail in request.GoodNoteDetails)
                {
                    if (detail.NewBatch == null)
                        return Fail("Thiếu thông tin lô cho sản phẩm.", null);
                }

                // Tạo phiếu nhập kho
                var goodNote = _mapper.Map<GoodNote>(request);
                goodNote.CreatedTime = DateTime.Now;
                goodNote.Status = GoodNoteStatusEnum.Completed;

                await _unitOfWork.GoodNoteRepository.Add(goodNote);

                // Tạo batch và chi tiết
                var goodNoteDetails = new List<GoodNoteDetail>();

                foreach (var detailDto in request.GoodNoteDetails)
                {
                    // Tạo batch
                    var batch = _mapper.Map<Batch>(detailDto.NewBatch);
                    await _unitOfWork.BatchRepository.Add(batch);

                    // Tạo chi tiết
                    var detail = _mapper.Map<GoodNoteDetail>(detailDto);
                    detail.GoodNoteId = goodNote.Id;
                    detail.CreatedTime = DateTime.Now;
                    detail.BatchId = batch.Id;

                    await _unitOfWork.GoodNoteDetailRepository.Add(detail);

                    goodNoteDetails.Add(detail);
                }

                // Cập nhật tồn kho
                await UpdateInventories(goodNote, goodNoteDetails);

                // Cập nhật trạng thái yêu cầu kho nếu có
                if (goodRequest != null)
                {
                    goodRequest.Status = GoodRequestStatusEnum.Approved;
                    _unitOfWork.GoodRequestRepository.Update(goodRequest);

                    // Thông báo cho người yêu cầu
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

            var requestedWarehouseId = goodNote.GoodRequest?.RequestedWarehouseId;
            if (string.IsNullOrEmpty(requestedWarehouseId))
            {
                var goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(g => g.Id == goodNote.GoodRequestId)
                    ?? throw new Exception($"Không tìm thấy yêu cầu kho với ID: {goodNote.GoodRequestId}.");
                requestedWarehouseId = goodRequest.RequestedWarehouseId
                    ?? throw new Exception("Yêu cầu kho chưa có kho nhận.");
            }

            var batchIds = details.Select(d => d.BatchId).Distinct().ToList();
            var inventories = await _unitOfWork.InventoryRepository.Search(
                i => batchIds.Contains(i.BatchId),
                includeProperties: "Warehouse"
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
        private ServiceResponse Fail(string message, object data = null)
        {
            return new ServiceResponse
            {
                Status = SRStatus.Error,
                Message = message,
                Data = data
            };
        }

        //public async Task<ServiceResponse> CreateReceiveNoteAsync(GoodNoteCreateDTO request)
        //{
        //    await _unitOfWork.BeginTransactionAsync();

        //    if (!string.IsNullOrEmpty(request.GoodRequestId))
        //    {
        //        var goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(x => x.Id == request.GoodRequestId);
        //        if (goodRequest == null)
        //        {
        //            return new ServiceResponse
        //            {
        //                Status = SRStatus.Error,
        //                Message = "Không tìm thấy yêu cầu kho.",
        //                Data = request.GoodRequestId
        //            };
        //        }
        //    }
        //    var goodNoteCodeExists = await _unitOfWork.GoodNoteRepository.GetByCondition(x => x.Code == request.Code);
        //    if (goodNoteCodeExists != null)
        //    {
        //        return new ServiceResponse
        //        {
        //            Status = SRStatus.Error,
        //            Message = "Mã phiếu đã tồn tại.",
        //            Data = request.Code
        //        };
        //    }
        //    if (request.GoodNoteDetails != null && request.GoodNoteDetails.Any())
        //    {
        //        foreach (var detail in request.GoodNoteDetails)
        //        {
        //            if (detail.NewBatch == null)
        //            {
        //                return new ServiceResponse
        //                {
        //                    Status = SRStatus.Error,
        //                    Message = $"Thiếu thông tin lô!",
        //                    Data = null
        //                };
        //            }
        //        }
        //    }
        //    try
        //    {
        //        //Tạo goodnote
        //        var entity = _mapper.Map<GoodNote>(request);
        //        entity.CreatedTime = DateTime.Now;
        //        entity.Status = GoodNoteStatusEnum.Completed;
        //        await _unitOfWork.GoodNoteRepository.Add(entity);

        //        //cập nhật goodrequest là thành công
        //        var goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(x => x.Id == request.GoodRequestId);
        //        goodRequest.Status = GoodRequestStatusEnum.Approved;
        //        _unitOfWork.GoodRequestRepository.Update(goodRequest);

        //        //Tạo goodnoteDetail
        //        var goodNoteDetails = _mapper.Map<List<GoodNoteDetail>>(request.GoodNoteDetails);
        //        foreach (var detail in request.GoodNoteDetails!)
        //        {
        //            // Nếu có NewBatch thì tạo mới
        //            var batch = _mapper.Map<Batch>(detail.NewBatch);
        //            await _unitOfWork.BatchRepository.Add(batch);

        //            //Thêm GoodNoteDetail
        //            detail.GoodNoteId = entity.Id;
        //            detail.CreatedTime = DateTime.Now;
        //            var goodNoteDetail = _mapper.Map<GoodNoteDetail>(detail);
        //            goodNoteDetail.BatchId = batch.Id; // Gán BatchId mới cho GoodNoteDetail
        //            await _unitOfWork.GoodNoteDetailRepository.Add(goodNoteDetail);

        //        }
        //        //Cập nhật tồn kho
        //        await UpdateInventories(entity, goodNoteDetails);

        //        await _unitOfWork.SaveAsync();

        //         // thông báo cho người tạo yêu cầu
        //        if (goodRequest.CreatedBy != null)
        //        {
        //            await _firebaseService.SendNotificationToUsersAsync(
        //                new List<string> { goodRequest.CreatedBy },
        //                "Thông báo",
        //                $"Phiếu kho {entity.Code} đã được tạo thành công từ yêu cầu kho {goodRequest.Code}.",
        //                NotificationType.GOOD_REQUEST_APPROVED,
        //                goodRequest.RequestedWarehouseId
        //            );
        //        }

        //        await _unitOfWork.CommitTransactionAsync();

        //        return new ServiceResponse
        //        {
        //            Status = SRStatus.Success,
        //            Message = "Phiếu kho được tạo thành công!",
        //            Data = "GoodNoteId: " + entity.Id
        //        };
        //    }
        //    catch (DbUpdateException dbEx)
        //    {
        //        await _unitOfWork.RollbackTransactionAsync();  // Rollback transaction nếu có lỗi
        //        return new ServiceResponse
        //        {
        //            Status = SRStatus.Error,
        //            Message = $"Lỗi dữ liệu: {dbEx.InnerException?.Message ?? dbEx.Message}"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await _unitOfWork.RollbackTransactionAsync();  // Rollback transaction nếu có lỗi
        //        return new ServiceResponse
        //        {
        //            Status = SRStatus.Error,
        //            Message = $"Lỗi khi tạo phiếu kho: {ex.Message}"
        //        };
        //    }
        //}

        //private async Task UpdateInventories(GoodNote goodNote, List<GoodNoteDetail> details)
        //{
        //    try
        //    {
        //        if (goodNote == null)
        //        {
        //            throw new Exception("Phiếu kho không được để trống.");
        //        }

        //        if (details == null || !details.Any())
        //        {
        //            throw new Exception("Danh sách chi tiết phiếu kho không được để trống.");
        //        }

        //        if (goodNote.NoteType != GoodNoteEnum.Receive)
        //        {
        //            throw new Exception("Chức năng này chỉ hỗ trợ cho phiếu nhập kho.");
        //        }

        //        var batchIds = details.Select(d => d.BatchId).Distinct().ToList();
        //        if (!batchIds.Any())
        //        {
        //            throw new Exception("Không tìm thấy BatchId trong chi tiết phiếu kho.");
        //        }

        //        var goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(g => g.Id == goodNote.GoodRequestId);
        //        if (goodRequest == null)
        //        {
        //            throw new Exception($"Không tìm thấy yêu cầu kho với ID: {goodNote.GoodRequestId}.");
        //        }

        //        var requestedWarehouseId = goodRequest.RequestedWarehouseId
        //            ?? throw new Exception("Yêu cầu kho chưa có kho nhận.");

        //        // Lấy danh sách tồn kho hiện tại của các Batch liên quan
        //        var inventories = await _unitOfWork.InventoryRepository.Search(
        //            i => batchIds.Contains(i.BatchId),
        //            includeProperties: "Warehouse"
        //        );

        //        foreach (var detail in details)
        //        {
        //            var targetInventory = inventories.FirstOrDefault(i => i.BatchId == detail.BatchId && i.WarehouseId == requestedWarehouseId);

        //            if (targetInventory == null)
        //            {
        //                // Nếu chưa có tồn kho → tạo mới
        //                targetInventory = new Inventory
        //                {
        //                    WarehouseId = requestedWarehouseId,
        //                    BatchId = detail.BatchId,
        //                    CurrentQuantity = detail.Quantity
        //                };
        //                await _unitOfWork.InventoryRepository.Add(targetInventory);
        //            }
        //            else
        //            {
        //                // Nếu đã có tồn kho → cộng thêm số lượng
        //                targetInventory.CurrentQuantity += detail.Quantity;
        //                _unitOfWork.InventoryRepository.Update(targetInventory);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Lỗi khi cập nhật tồn kho: {ex.Message}");
        //    }
        //}

        //private async Task UpdateInventories(GoodNote goodNote, List<GoodNoteDetail> details)
        //{
        //    try
        //    {
        //        if (goodNote == null)
        //        {
        //            throw new Exception("GoodNote cannot be null.");
        //        }

        //        if (details == null || !details.Any())
        //        {
        //            throw new Exception("GoodNoteDetails cannot be null or empty.");
        //        }
        //        var batchIds = details.Select(d => d.BatchId).Distinct().ToList();
        //        if (!batchIds.Any())
        //        {
        //            throw new Exception("No batch IDs found in GoodNoteDetails.");
        //        }
        //        var goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(g => g.Id == goodNote.GoodRequestId);
        //        if (goodRequest == null)
        //        {
        //            throw new Exception($"Good request {goodNote.GoodRequestId} not found.");
        //        }

        //        var warehouseId = goodRequest.WarehouseId; // 🔥 Chỉ dùng khi Transfer
        //        var requestedWarehouseId = goodRequest.RequestedWarehouseId ?? throw new Exception("Requested warehouse ID is missing."); // 🔥 Dùng cho tất cả các loại phiếu

        //        // Lấy danh sách Inventory cho tất cả Batch liên quan
        //        var inventories = await _unitOfWork.InventoryRepository.Search(i => batchIds.Contains(i.BatchId), includeProperties: "Warehouse");

        //        foreach (var detail in details)
        //        {
        //            Inventory? sourceInventory = null;
        //            Inventory? targetInventory = null;

        //            switch (goodNote.NoteType)
        //            {
        //                case GoodNoteEnum.Receive:
        //                    // 🟢 Hàng nhập vào requestedWarehouseId
        //                    targetInventory = inventories.FirstOrDefault(i => i.BatchId == detail.BatchId && i.WarehouseId == requestedWarehouseId);
        //                    if (targetInventory == null)
        //                    {
        //                        targetInventory = new Inventory
        //                        {
        //                            WarehouseId = requestedWarehouseId,
        //                            BatchId = detail.BatchId,
        //                            CurrentQuantity = detail.Quantity,
        //                            //ArrangedQuantity = 0,
        //                            //NotArrgangedQuantity = detail.Quantity,
        //                        };
        //                        await _unitOfWork.InventoryRepository.Add(targetInventory);
        //                        break;
        //                    }
        //                    targetInventory.CurrentQuantity += detail.Quantity;
        //                    //targetInventory.NotArrgangedQuantity += detail.Quantity;
        //                    _unitOfWork.InventoryRepository.Update(targetInventory);
        //                    break;
        //                case GoodNoteEnum.Issue:
        //                    // 🔴 Xuất hàng từ requestedWarehouseId
        //                    sourceInventory = inventories.FirstOrDefault(i => i.BatchId == detail.BatchId && i.WarehouseId == requestedWarehouseId);
        //                    if (sourceInventory == null || sourceInventory.CurrentQuantity < detail.Quantity)
        //                    {
        //                        throw new Exception($"Không đủ hàng trong lô  {detail.Batch.Code} để xuất ở kho {sourceInventory.Warehouse.Name ?? null}.");
        //                    }
        //                    sourceInventory.CurrentQuantity -= detail.Quantity;
        //                    //sourceInventory.NotArrgangedQuantity -= detail.Quantity;
        //                    _unitOfWork.InventoryRepository.Update(sourceInventory);
        //                    break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Lỗi khi tạo tồn kho: {ex.Message}");
        //    }
        //}


        //public async Task<ServiceResponse> DeleteAsync(string id)
        //{
        //    var entity = await _goodNoteRepository.GetByCondition(x => x.Id == id);
        //    if (entity == null)
        //    {
        //        return new ServiceResponse
        //        {
        //            Status = SRStatus.NotFound,
        //            Message = "Good note not found!",
        //            Data = id
        //        };
        //    }

        //    try
        //    {
        //        _goodNoteRepository.Delete(entity);
        //        await _unitOfWork.SaveAsync();

        //        return new ServiceResponse
        //        {
        //            Status = SRStatus.Success,
        //            Message = "Good note deleted successfully!",
        //            Data = id
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse
        //        {
        //            Status = SRStatus.Error,
        //            Message = $"Error deleting good note: {ex.Message}"
        //        };
        //    }
        //}
        //public async Task<ServiceResponse> UpdateStatusAsync(string id, GoodNoteStatusEnum newStatus)
        //{
        //    var goodNote = await _goodNoteRepository.GetByCondition(x => x.Id == id);
        //    if (goodNote == null)
        //    {
        //        return new ServiceResponse
        //        {
        //            Status = SRStatus.NotFound,
        //            Message = "GoodNote không tồn tại.",
        //            Data = id
        //        };
        //    }

        //    // Kiểm tra quy tắc cập nhật trạng thái
        //    if (!CanUpdateStatus(goodNote.Status, newStatus))
        //    {
        //        return new ServiceResponse
        //        {
        //            Status = SRStatus.Error,
        //            Message = $"Không thể chuyển từ trạng thái {goodNote.Status} sang {newStatus}.",
        //            Data = id
        //        };
        //    }

        //    // Nếu chuyển sang Completed thì mới cập nhật Inventory
        //    if (newStatus == GoodNoteStatusEnum.Completed)
        //    {
        //        var goodNoteDetails = await _unitOfWork.GoodNoteDetailRepository.Search(x => x.GoodNoteId == id);
        //        if (!goodNoteDetails.Any())
        //        {
        //            return new ServiceResponse
        //            {
        //                Status = SRStatus.Error,
        //                Message = "Không có chi tiết hàng hóa để cập nhật tồn kho.",
        //                Data = id
        //            };
        //        }

        //        try
        //        {
        //            await UpdateInventories(goodNote, goodNoteDetails.ToList());
        //        }
        //        catch (Exception ex)
        //        {
        //            return new ServiceResponse
        //            {
        //                Status = SRStatus.Error,
        //                Message = $"Lỗi khi cập nhật tồn kho: {ex.Message}"
        //            };
        //        }
        //    }

        //    goodNote.Status = newStatus;
        //    _goodNoteRepository.Update(goodNote);
        //    await _unitOfWork.SaveAsync();

        //    return new ServiceResponse
        //    {
        //        Status = SRStatus.Success,
        //        Message = "Cập nhật trạng thái thành công.",
        //        Data = new { goodNoteId = id, newStatus = goodNote.Status }
        //    };
        //}


        ///// <summary>
        ///// Kiểm tra xem trạng thái có thể cập nhật không
        ///// </summary>
        //private bool CanUpdateStatus(GoodNoteStatusEnum currentStatus, GoodNoteStatusEnum newStatus)
        //{
        //    var validTransitions = new Dictionary<GoodNoteStatusEnum, List<GoodNoteStatusEnum>>()
        //    {
        //        { GoodNoteStatusEnum.Pending, new List<GoodNoteStatusEnum> { GoodNoteStatusEnum.Completed, GoodNoteStatusEnum.Canceled, GoodNoteStatusEnum.Failed } },
        //        { GoodNoteStatusEnum.Completed, new List<GoodNoteStatusEnum>() }, // Không thể đổi từ Completed
        //        { GoodNoteStatusEnum.Canceled, new List<GoodNoteStatusEnum>() }, // Không thể đổi từ Canceled
        //        { GoodNoteStatusEnum.Failed, new List<GoodNoteStatusEnum> { GoodNoteStatusEnum.Pending } } // Có thể thử lại từ Failed -> Pending
        //    };
        //    return validTransitions.ContainsKey(currentStatus) && validTransitions[currentStatus].Contains(newStatus);
        //}

        //public async Task<ServiceResponse> UpdateAsync(string id, GoodNoteUpdateDTO request)
        //{
        //    var goodNote = await _goodNoteRepository.GetByCondition(x => x.Id == id);
        //    if (goodNote == null)
        //    {
        //        return new ServiceResponse
        //        {
        //            Status = SRStatus.NotFound,
        //            Message = "GoodNote không tồn tại.",
        //            Data = id
        //        };
        //    }

        //    // ❌ Không cho cập nhật nếu trạng thái là Completed
        //    if (goodNote.Status == GoodNoteStatusEnum.Completed)
        //    {
        //        return new ServiceResponse
        //        {
        //            Status = SRStatus.Error,
        //            Message = "Không thể cập nhật GoodNote đã hoàn thành.",
        //            Data = id
        //        };
        //    }

        //    // ✅ Chỉ cập nhật các trường được truyền xuống (PATCH)
        //    if (request.ShipperName != null) goodNote.ShipperName = request.ShipperName;
        //    if (request.ReceiverName != null) goodNote.ReceiverName = request.ReceiverName;
        //    if (request.Code != null)
        //    {
        //        var goodNoteCodeExists = await _unitOfWork.GoodNoteRepository.GetByCondition(x => x.Code == request.Code && x.Id != request.Id);
        //        if (goodNoteCodeExists != null)
        //        {
        //            return new ServiceResponse
        //            {
        //                Status = SRStatus.Error,
        //                Message = "Goodnote code already exists for this code.",
        //                Data = request.Code
        //            };
        //        }
        //        goodNote.Code = request.Code;
        //    }
        //    if (request.Date.HasValue) goodNote.Date = request.Date.Value;
        //    if (request.NoteType != null) goodNote.NoteType = request.NoteType.Value;

        //    // ⚡ Xử lý cập nhật danh sách GoodNoteDetails
        //    if (request.GoodNoteDetails != null && request.GoodNoteDetails.Any())
        //    {
        //        // 🗑️ Xóa từng phần tử trong danh sách cũ
        //        var oldDetails = await _unitOfWork.GoodNoteDetailRepository.Search(x => x.GoodNoteId == id);
        //        foreach (var oldDetail in oldDetails)
        //        {
        //            _unitOfWork.GoodNoteDetailRepository.Delete(oldDetail);
        //        }

        //        // ➕ Thêm từng phần tử mới
        //        foreach (var detail in request.GoodNoteDetails)
        //        {
        //            var newDetail = new GoodNoteDetail
        //            {
        //                GoodNoteId = id,
        //                BatchId = detail.BatchId,
        //                Quantity = detail.Quantity,
        //                Note = detail.Note,
        //                CreatedTime = DateTime.Now
        //            };
        //            await _unitOfWork.GoodNoteDetailRepository.Add(newDetail);
        //        }
        //    }

        //    _goodNoteRepository.Update(goodNote);
        //    await _unitOfWork.SaveAsync();

        //    return new ServiceResponse
        //    {
        //        Status = SRStatus.Success,
        //        Message = "Cập nhật GoodNote thành công.",
        //        Data = new { goodNoteId = id }
        //    };
        //}
        public async Task<ServiceResponse> CreateIssueNoteAsync(GoodNoteIssueCreateDTO dto)
        {
            var serviceResponse = new ServiceResponse();

            await _unitOfWork.BeginTransactionAsync();  // Bắt đầu transaction

            try
            {
                // Kiểm tra tồn kho
                var checkResult = await CheckInventorySufficientAsync(dto);
                if (!checkResult)
                {
                    serviceResponse.Status = SRStatus.Error;
                    serviceResponse.Message = "Tồn kho không đủ";
                    await _unitOfWork.RollbackTransactionAsync();  // Rollback transaction nếu kiểm tra tồn kho thất bại
                    return serviceResponse;
                }

                // Tạo phiếu xuất kho
                var goodNote = await CreateGoodNoteIssueAsync(dto);

                // Trừ tồn kho và tạo chi tiết phiếu xuất
                var deductResult = await DeductInventoryAndCreateDetailsAsync(dto, goodNote.Id);
                if (!deductResult)
                {
                    serviceResponse.Status = SRStatus.Error;
                    serviceResponse.Message = "Lỗi khi trừ tồn kho";
                    await _unitOfWork.RollbackTransactionAsync();  // Rollback transaction nếu không trừ được tồn kho
                    return serviceResponse;
                }

                var goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(x => x.Id == dto.GoodRequestId);
                if (goodRequest == null)
                {
                    serviceResponse.Status = SRStatus.Error;
                    serviceResponse.Message = "Không tìm thấy yêu cầu kho.";
                    await _unitOfWork.RollbackTransactionAsync();  // Rollback transaction nếu không tìm thấy yêu cầu kho
                    return serviceResponse;
                }
                if (goodRequest.RequestType != GoodRequestEnum.Issue && goodRequest.RequestType != GoodRequestEnum.Transfer)
                {
                    serviceResponse.Status = SRStatus.Error;
                    serviceResponse.Message = "Yêu cầu kho không phải là yêu cầu xuất kho hoặc điều chuyển kho.";
                    await _unitOfWork.RollbackTransactionAsync();  // Rollback transaction nếu yêu cầu không phải xuất kho
                    return serviceResponse;
                }
                // Cập nhật trạng thái yêu cầu kho
                goodRequest.Status = GoodRequestStatusEnum.Approved;
                _unitOfWork.GoodRequestRepository.Update(goodRequest);

                // Lưu vào cơ sở dữ liệu và commit transaction
                await _unitOfWork.SaveAsync();

                // Gửi thông báo
                var details = await _unitOfWork.GoodNoteDetailRepository.GetDetailsByGoodNoteIdAsync(goodNote.Id);

                var batchMessages = details
                    .Select(x => $"{x.Batch.Code} ({x.Quantity})")
                    .ToList();


                var keeperIds = await _unitOfWork.AccountRepository.GetUserIdsByRequestedWarehouseAndGroups(goodRequest.RequestedWarehouseId, new List<string> { "Thủ kho" });
               
                await SendIssueNoteNotificationAsync(
                    keeperIds,
                    goodRequest.CreatedBy,
                    goodNote.Code,
                    batchMessages,
                    goodRequest.RequestedWarehouseId
                );

                await _unitOfWork.CommitTransactionAsync();  // Commit transaction nếu mọi thứ thành công

                serviceResponse.Status = SRStatus.Success;
                serviceResponse.Message = "Tạo phiếu xuất kho thành công";
                serviceResponse.Data = goodNote.Id;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();  // Rollback transaction nếu có lỗi
                serviceResponse.Status = SRStatus.Error;
                serviceResponse.Message = $"Có lỗi: {ex.Message}";
            }

            return serviceResponse;
        }

        private async Task<bool> CheckInventorySufficientAsync(GoodNoteIssueCreateDTO dto)
        {
            var productGroups = dto.GoodNoteDetails
                .GroupBy(x => x.ProductId)
                .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(x => x.Quantity) })
                .ToList();

            foreach (var product in productGroups)
            {
                var inventories = await _unitOfWork.InventoryRepository.GetAvailableInventoriesAsync(product.ProductId);

                var totalStock = inventories.Sum(x => x.CurrentQuantity);

                if (totalStock < product.TotalQuantity)
                {
                    return false; // Tồn kho không đủ
                }
            }

            return true; // Tồn kho đủ
        }

        private async Task<GoodNote> CreateGoodNoteIssueAsync(GoodNoteIssueCreateDTO dto)
        {
            var goodNote = new GoodNote
            {
                Code = dto.Code,
                Date = dto.Date,
                ReceiverName = dto.ReceiverName,
                ShipperName = dto.ShipperName,
                NoteType = GoodNoteEnum.Issue, // Phiếu xuất kho
                Status = GoodNoteStatusEnum.Completed, // Trạng thái ban đầu là Chờ duyệt
                GoodRequestId = dto.GoodRequestId
            };

            await _unitOfWork.GoodNoteRepository.Add(goodNote);
            return goodNote;
        }

        private async Task<bool> DeductInventoryAndCreateDetailsAsync(GoodNoteIssueCreateDTO dto, string goodNoteId)
        {
            foreach (var detailDto in dto.GoodNoteDetails)
            {
                float remainQuantity = detailDto.Quantity;

                var availableInventories = await _unitOfWork.InventoryRepository.GetAvailableInventoriesAsync(detailDto.ProductId);

                foreach (var inventory in availableInventories)
                {
                    if (remainQuantity <= 0) break;

                    var usedQuantity = Math.Min(inventory.CurrentQuantity, remainQuantity);

                    var detail = new GoodNoteDetail
                    {
                        Quantity = usedQuantity,
                        Note = detailDto.Note,
                        GoodNoteId = goodNoteId,
                        BatchId = inventory.BatchId
                    };
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
            var keeperMessage = $"Phiếu xuất {issueNoteCode} đã xuất các lô: {string.Join(", ", batchMessages)}";
            var requesterMessage = $"Yêu cầu của bạn đã được tạo phiếu xuất: {issueNoteCode}";

            await _firebaseService.SendNotificationToUsersAsync(
                warehouseKeeperIds,
                "Thông báo xuất kho",
                keeperMessage,
                NotificationType.ISSUE_NOTE_CREATED,
                requestedWarehouseId
            );

            await _firebaseService.SendNotificationToUsersAsync(
                new List<string> { requesterId },
                "Thông báo yêu cầu kho",
                requesterMessage,
                NotificationType.GOOD_REQUEST_APPROVED,
                requestedWarehouseId
            );
        }

    }
}
