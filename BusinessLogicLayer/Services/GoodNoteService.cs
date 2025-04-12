using AutoMapper;
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

        public GoodNoteService(IGenericRepository<GoodNote> genericRepository, IUnitOfWork unitOfWork, IMapper mapper)
            : base(genericRepository, mapper, unitOfWork)
        {
            _goodNoteRepository = unitOfWork.GoodNoteRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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
                    Message = "Search successful!",
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
                    Message = "An error occurred while searching GoodNotes. Please try again later.",
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
                    Message = "Good note not found!",
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
                Message = "Get good note successfully!",
                Data = groupedResult
            };


        }

        public async Task<ServiceResponse> CreateAsync(GoodNoteCreateDTO request)
        {
            if (!string.IsNullOrEmpty(request.GoodRequestId))
            {
                var goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(x => x.Id == request.GoodRequestId);
                if (goodRequest == null)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Good Request not found.",
                        Data = request.GoodRequestId
                    };
                }
                var goodNoteExists = await _unitOfWork.GoodNoteRepository.GetByCondition(x => x.GoodRequestId == request.GoodRequestId);
                if(goodNoteExists != null)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Good note already exists for this Good Request ID.",
                        Data = request.GoodRequestId
                    };
                }
            }
            var goodNoteCodeExists = await _unitOfWork.GoodNoteRepository.GetByCondition(x => x.Code == request.Code);
            if (goodNoteCodeExists != null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = "Goodnote code already exists for this code.",
                    Data = request.Code
                };
            }
            if (request.GoodNoteDetails != null && request.GoodNoteDetails.Any())
            {
                foreach (var detail in request.GoodNoteDetails)
                {
                    if (string.IsNullOrEmpty(detail.BatchId))
                    {
                        return new ServiceResponse
                        {
                            Status = SRStatus.Error,
                            Message = "BatchId cannot be null or empty in GoodNoteDetails."
                        };
                    }

                    var batchExists = await _unitOfWork.BatchRepository.GetByCondition(x => x.Id == detail.BatchId);
                    if (batchExists == null)
                    {
                        return new ServiceResponse
                        {
                            Status = SRStatus.Error,
                            Message = $"Batch with ID {detail.BatchId} not found.",
                            Data = detail.BatchId
                        };
                    }
                }
            }
            try
            {
                //Tạo goodnote
                var entity = _mapper.Map<GoodNote>(request);
                entity.CreatedTime = DateTime.Now;
                entity.Status = GoodNoteStatusEnum.Completed;
                await _unitOfWork.GoodNoteRepository.Add(entity);
                //cập nhật goodrequest là thành công
                var goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(x => x.Id == request.GoodRequestId);
                goodRequest.Status = GoodRequestStatusEnum.Approved;
                _unitOfWork.GoodRequestRepository.Update(goodRequest);
                // thông báo cho người tạo yêu cầu
                // ++++

                //Tạo goodnoteDetail
                var goodNoteDetails = _mapper.Map<List<GoodNoteDetail>>(request.GoodNoteDetails);
                foreach (var detail in request.GoodNoteDetails!)
                {
                    detail.GoodNoteId = entity.Id;
                    detail.CreatedTime = DateTime.Now;
                    var goodNoteDetail = _mapper.Map<GoodNoteDetail>(detail);
                    await _unitOfWork.GoodNoteDetailRepository.Add(goodNoteDetail);
                }
                //await _unitOfWork.SaveAsync();
                //Cập nhật tồn kho
                await UpdateInventories(entity, goodNoteDetails);
                await _unitOfWork.SaveAsync();
                return new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Good note created successfully!",
                    Data = "GoodNoteId: " +entity.Id
                };
            }
            catch (DbUpdateException dbEx)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = $"Error creating good note: {ex.Message}"
                };
            }
        }

        private async Task UpdateInventories(GoodNote goodNote, List<GoodNoteDetail> details)
        {
            try
            {
                if (goodNote == null)
                {
                    throw new Exception("GoodNote cannot be null.");
                }

                if (details == null || !details.Any())
                {
                    throw new Exception("GoodNoteDetails cannot be null or empty.");
                }
                var batchIds = details.Select(d => d.BatchId).Distinct().ToList();
                if (!batchIds.Any())
                {
                    throw new Exception("No batch IDs found in GoodNoteDetails.");
                }
                var goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(g => g.Id == goodNote.GoodRequestId);
                if (goodRequest == null)
                {
                    throw new Exception($"Good request {goodNote.GoodRequestId} not found.");
                }

                var warehouseId = goodRequest.WarehouseId; // 🔥 Chỉ dùng khi Transfer
                var requestedWarehouseId = goodRequest.RequestedWarehouseId ?? throw new Exception("Requested warehouse ID is missing."); // 🔥 Dùng cho tất cả các loại phiếu

                // Lấy danh sách Inventory cho tất cả Batch liên quan
                var inventories = await _unitOfWork.InventoryRepository.Search(i => batchIds.Contains(i.BatchId));

                foreach (var detail in details)
                {
                    Inventory? sourceInventory = null;
                    Inventory? targetInventory = null;

                    switch (goodNote.NoteType)
                    {
                        case GoodNoteEnum.Receive:
                            // 🟢 Hàng nhập vào requestedWarehouseId
                            targetInventory = inventories.FirstOrDefault(i => i.BatchId == detail.BatchId && i.WarehouseId == requestedWarehouseId);
                            if (targetInventory == null)
                            {
                                targetInventory = new Inventory
                                {
                                    WarehouseId = requestedWarehouseId,
                                    BatchId = detail.BatchId,
                                    CurrentQuantity = detail.Quantity,
                                    //ArrangedQuantity = 0,
                                    //NotArrgangedQuantity = detail.Quantity,
                                };
                                await _unitOfWork.InventoryRepository.Add(targetInventory);
                                break;
                            }
                            targetInventory.CurrentQuantity += detail.Quantity;
                            //targetInventory.NotArrgangedQuantity += detail.Quantity;
                            _unitOfWork.InventoryRepository.Update(targetInventory);
                            break;
                        //case GoodNoteEnum.Return:
                        //    // 🟢 Hàng nhập vào requestedWarehouseId
                        //    targetInventory = inventories.FirstOrDefault(i => i.BatchId == detail.BatchId && i.WarehouseId == requestedWarehouseId);
                        //    if (targetInventory == null)
                        //    {
                        //        targetInventory = new Inventory
                        //        {
                        //            WarehouseId = requestedWarehouseId,
                        //            BatchId = detail.BatchId,
                        //            CurrentQuantity = detail.Quantity,
                        //            ArrangedQuantity = 0,
                        //            NotArrgangedQuantity = detail.Quantity,
                        //        };
                        //        await _unitOfWork.InventoryRepository.Add(targetInventory);
                        //        break;
                        //    }
                        //    targetInventory.CurrentQuantity += detail.Quantity;
                        //    targetInventory.NotArrgangedQuantity += detail.Quantity;
                        //    _unitOfWork.InventoryRepository.Update(targetInventory);
                        //    break;

                        case GoodNoteEnum.Issue:
                            // 🔴 Xuất hàng từ requestedWarehouseId
                            sourceInventory = inventories.FirstOrDefault(i => i.BatchId == detail.BatchId && i.WarehouseId == requestedWarehouseId);
                            if (sourceInventory == null || sourceInventory.CurrentQuantity < detail.Quantity)
                            {
                                throw new Exception($"Not enough 'Inventory not in location' in warehouse {requestedWarehouseId} for batch {detail.BatchId}.");
                            }
                            sourceInventory.CurrentQuantity -= detail.Quantity;
                            //sourceInventory.NotArrgangedQuantity -= detail.Quantity;
                            _unitOfWork.InventoryRepository.Update(sourceInventory);
                            break;

                        //case GoodNoteEnum.Transfer:
                        //    // 🔁 Điều chuyển hàng từ warehouseId -> requestedWarehouseId
                        //    sourceInventory = inventories.FirstOrDefault(i => i.BatchId == detail.BatchId && i.WarehouseId == warehouseId);
                        //    targetInventory = inventories.FirstOrDefault(i => i.BatchId == detail.BatchId && i.WarehouseId == requestedWarehouseId);

                        //    if (sourceInventory == null || sourceInventory.NotArrgangedQuantity < detail.Quantity)
                        //    {
                        //        throw new Exception($"Not enough 'Inventory not in location' in warehouse {warehouseId} for batch {detail.BatchId}.");
                        //    }

                        //    sourceInventory.CurrentQuantity -= detail.Quantity;
                        //    sourceInventory.NotArrgangedQuantity -= detail.Quantity;
                        //    _unitOfWork.InventoryRepository.Update(sourceInventory);

                        //    if (targetInventory == null)
                        //    {
                        //        targetInventory = new Inventory
                        //        {
                        //            WarehouseId = requestedWarehouseId,
                        //            BatchId = detail.BatchId,
                        //            CurrentQuantity = detail.Quantity,
                        //            ArrangedQuantity = 0,
                        //            NotArrgangedQuantity = detail.Quantity,
                        //        };
                        //        await _unitOfWork.InventoryRepository.Add(targetInventory);
                        //        break;
                        //    }

                        //    targetInventory.CurrentQuantity += detail.Quantity;
                        //    targetInventory.NotArrgangedQuantity += detail.Quantity;
                        //    _unitOfWork.InventoryRepository.Update(targetInventory);
                        //    break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating inventory: {ex.Message}");
            }
        }



        public async Task<ServiceResponse> DeleteAsync(string id)
        {
            var entity = await _goodNoteRepository.GetByCondition(x => x.Id == id);
            if (entity == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Good note not found!",
                    Data = id
                };
            }

            try
            {
                _goodNoteRepository.Delete(entity);
                await _unitOfWork.SaveAsync();

                return new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Good note deleted successfully!",
                    Data = id
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = $"Error deleting good note: {ex.Message}"
                };
            }
        }
        public async Task<ServiceResponse> UpdateStatusAsync(string id, GoodNoteStatusEnum newStatus)
        {
            var goodNote = await _goodNoteRepository.GetByCondition(x => x.Id == id);
            if (goodNote == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "GoodNote không tồn tại.",
                    Data = id
                };
            }

            // Kiểm tra quy tắc cập nhật trạng thái
            if (!CanUpdateStatus(goodNote.Status, newStatus))
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = $"Không thể chuyển từ trạng thái {goodNote.Status} sang {newStatus}.",
                    Data = id
                };
            }

            // Nếu chuyển sang Completed thì mới cập nhật Inventory
            if (newStatus == GoodNoteStatusEnum.Completed)
            {
                var goodNoteDetails = await _unitOfWork.GoodNoteDetailRepository.Search(x => x.GoodNoteId == id);
                if (!goodNoteDetails.Any())
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Không có chi tiết hàng hóa để cập nhật tồn kho.",
                        Data = id
                    };
                }

                try
                {
                    await UpdateInventories(goodNote, goodNoteDetails.ToList());
                }
                catch (Exception ex)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = $"Lỗi khi cập nhật tồn kho: {ex.Message}"
                    };
                }
            }

            goodNote.Status = newStatus;
            _goodNoteRepository.Update(goodNote);
            await _unitOfWork.SaveAsync();

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Cập nhật trạng thái thành công.",
                Data = new { goodNoteId = id, newStatus = goodNote.Status }
            };
        }


        /// <summary>
        /// Kiểm tra xem trạng thái có thể cập nhật không
        /// </summary>
        private bool CanUpdateStatus(GoodNoteStatusEnum currentStatus, GoodNoteStatusEnum newStatus)
        {
            var validTransitions = new Dictionary<GoodNoteStatusEnum, List<GoodNoteStatusEnum>>()
            {
                { GoodNoteStatusEnum.Pending, new List<GoodNoteStatusEnum> { GoodNoteStatusEnum.Completed, GoodNoteStatusEnum.Canceled, GoodNoteStatusEnum.Failed } },
                { GoodNoteStatusEnum.Completed, new List<GoodNoteStatusEnum>() }, // Không thể đổi từ Completed
                { GoodNoteStatusEnum.Canceled, new List<GoodNoteStatusEnum>() }, // Không thể đổi từ Canceled
                { GoodNoteStatusEnum.Failed, new List<GoodNoteStatusEnum> { GoodNoteStatusEnum.Pending } } // Có thể thử lại từ Failed -> Pending
            };
            return validTransitions.ContainsKey(currentStatus) && validTransitions[currentStatus].Contains(newStatus);
        }

        public async Task<ServiceResponse> UpdateAsync(string id, GoodNoteUpdateDTO request)
        {
            var goodNote = await _goodNoteRepository.GetByCondition(x => x.Id == id);
            if (goodNote == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "GoodNote không tồn tại.",
                    Data = id
                };
            }

            // ❌ Không cho cập nhật nếu trạng thái là Completed
            if (goodNote.Status == GoodNoteStatusEnum.Completed)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = "Không thể cập nhật GoodNote đã hoàn thành.",
                    Data = id
                };
            }

            // ✅ Chỉ cập nhật các trường được truyền xuống (PATCH)
            if (request.ShipperName != null) goodNote.ShipperName = request.ShipperName;
            if (request.ReceiverName != null) goodNote.ReceiverName = request.ReceiverName;
            if (request.Code != null)
            {
                var goodNoteCodeExists = await _unitOfWork.GoodNoteRepository.GetByCondition(x => x.Code == request.Code && x.Id != request.Id);
                if (goodNoteCodeExists != null)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Goodnote code already exists for this code.",
                        Data = request.Code
                    };
                }
                goodNote.Code = request.Code;
            }
            if (request.Date.HasValue) goodNote.Date = request.Date.Value;
            if (request.NoteType != null) goodNote.NoteType = request.NoteType.Value;

            // ⚡ Xử lý cập nhật danh sách GoodNoteDetails
            if (request.GoodNoteDetails != null && request.GoodNoteDetails.Any())
            {
                // 🗑️ Xóa từng phần tử trong danh sách cũ
                var oldDetails = await _unitOfWork.GoodNoteDetailRepository.Search(x => x.GoodNoteId == id);
                foreach (var oldDetail in oldDetails)
                {
                    _unitOfWork.GoodNoteDetailRepository.Delete(oldDetail);
                }

                // ➕ Thêm từng phần tử mới
                foreach (var detail in request.GoodNoteDetails)
                {
                    var newDetail = new GoodNoteDetail
                    {
                        GoodNoteId = id,
                        BatchId = detail.BatchId,
                        Quantity = detail.Quantity,
                        Note = detail.Note,
                        CreatedTime = DateTime.Now
                    };
                    await _unitOfWork.GoodNoteDetailRepository.Add(newDetail);
                }
            }

            _goodNoteRepository.Update(goodNote);
            await _unitOfWork.SaveAsync();

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Cập nhật GoodNote thành công.",
                Data = new { goodNoteId = id }
            };
        }
    }
}
