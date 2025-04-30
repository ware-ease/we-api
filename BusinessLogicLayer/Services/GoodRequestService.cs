using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Enum;
using Data.Model.Request.GoodNote;
using Data.Model.Request.GoodRequest;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.UnitOfWork;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services
{
    public class GoodRequestService : GenericService<GoodRequest>, IGoodRequestService
    {
        private readonly IGoodRequestRepository _goodRequestRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseService _firebaseService;
        private readonly ICodeGeneratorService _codeGeneratorService;

        public GoodRequestService(
            IGenericRepository<GoodRequest> genericRepository, IUnitOfWork unitOfWork, IMapper mapper,
            IFirebaseService firebaseService, ICodeGeneratorService codeGeneratorService) : base(genericRepository, mapper, unitOfWork)
        {
            _goodRequestRepository = unitOfWork.GoodRequestRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _firebaseService = firebaseService;
            _codeGeneratorService = codeGeneratorService;
        }
        public async Task<ServiceResponse> GetById(string id)
        {
            var entity = await _goodRequestRepository.GetByCondition(g => g.Id == id, includeProperties: "GoodRequestDetails,Warehouse,RequestedWarehouse,Partner," +
                                                                                                         "GoodNotes," +
                                                                                                         "GoodRequestDetails.Product," +
                                                                                                         "GoodRequestDetails.Product.Unit," +
                                                                                                         "GoodRequestDetails.Product.Brand");
            if (entity == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Good request not found!",
                    Data = id
                };
            }

            var goodNote = await _unitOfWork.GoodNoteRepository.GetByCondition(g => g.GoodRequestId == id);
            var result = _mapper.Map<GoodRequestDTO>(entity);
            result.GoodNoteCount = await _unitOfWork.GoodNoteRepository.Count(g => g.GoodRequestId == id);
            await AttachGoodNoteToGoodRequest(result); // Với 1 đối tượng

            if (goodNote != null)
            {
                //result.GoodNote = _mapper.Map<GoodNoteOfGoodRequestDTO>(goodNote);
            }
            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Get good request successfully!",
                Data = result
            };
        }
        private CodeType GetCodeTypeFromRequestEnum(GoodRequestEnum requestType)
        {
            return requestType switch
            {
                GoodRequestEnum.Receive => CodeType.YCN,
                GoodRequestEnum.Issue => CodeType.YCX,
                GoodRequestEnum.Transfer => CodeType.YCC,
                GoodRequestEnum.Return => CodeType.YCT,
                _ => throw new ArgumentOutOfRangeException(nameof(requestType), "Invalid request type")
            };
        }

        public async Task<ServiceResponse> CreateAsync<TResult>(GoodRequestCreateDTO request)
        {
            // 1️⃣ Generate Code
            var codeType = GetCodeTypeFromRequestEnum(request.RequestType);
            request.Code = await _codeGeneratorService.GenerateCodeAsync(codeType);

            // Kiểm tra Code có bị trùng không
            var existingCode = await _goodRequestRepository.GetByCondition(g => g.Code == request.Code);
            if (existingCode != null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = "Mã yêu cầu đã tồn tại!",
                    Data = request.Code
                };
            }
            // 2️⃣ Kiểm tra PartnerId có tồn tại trong DB không (nếu có giá trị)
            if (!string.IsNullOrEmpty(request.PartnerId))
            {
                var partnerExists = await _unitOfWork.PartnerRepository.GetByCondition(x => x.Id == request.PartnerId);
                if (partnerExists == null)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Không tìm thấy đối tác!",
                        Data = request.PartnerId
                    };
                }
            }

            // 3️⃣ Kiểm tra WarehouseId có tồn tại không (nếu là Transfer thì bắt buộc)
            if (request.RequestType == GoodRequestEnum.Transfer && string.IsNullOrEmpty(request.WarehouseId))
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = "Vui lòng chọn kho gửi (WarehouseId) cho yêu cầu chuyển kho!"
                };
            }

            if (!string.IsNullOrEmpty(request.WarehouseId))
            {
                var warehouseExists = await _unitOfWork.WarehouseRepository.GetByCondition(x => x.Id == request.WarehouseId);
                if (warehouseExists == null)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Kho gửi không tồn tại!",
                        Data = request.WarehouseId
                    };
                }
            }

            // 4️⃣ Kiểm tra RequestedWarehouseId có tồn tại không (bắt buộc với tất cả loại yêu cầu)
            if (string.IsNullOrEmpty(request.RequestedWarehouseId))
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = "Vui lòng chọn kho nhận (RequestedWarehouseId)!"
                };
            }
            else
            {
                var requestedWarehouseExists = await _unitOfWork.WarehouseRepository.GetByCondition(x => x.Id == request.RequestedWarehouseId);
                if (requestedWarehouseExists == null)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Kho nhận không tồn tại!",
                        Data = request.RequestedWarehouseId
                    };
                }
            }


            // 5️⃣ Kiểm tra danh sách GoodRequestDetails nếu có
            if (request.GoodRequestDetails != null && request.GoodRequestDetails.Any())
            {
                foreach (var detail in request.GoodRequestDetails)
                {
                    if (string.IsNullOrEmpty(detail.ProductId))
                    {
                        return new ServiceResponse
                        {
                            Status = SRStatus.Error,
                            Message = "Vui lòng chọn sản phẩm trong chi tiết yêu cầu!"
                        };
                    }

                    var productExists = await _unitOfWork.ProductRepository.GetByCondition(x => x.Id == detail.ProductId);
                    if (productExists == null)
                    {
                        return new ServiceResponse
                        {
                            Status = SRStatus.Error,
                            Message = $"không tìm thấy sản phẩm với Id {detail.ProductId}!",
                            Data = detail.ProductId
                        };
                    }
                }
            }

            // 6️⃣ Mapping và thêm vào DB
            var entity = _mapper.Map<GoodRequest>(request);
            entity.CreatedTime = DateTime.Now;
            // Bắt đầu transaction
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _goodRequestRepository.Add(entity);
                await _unitOfWork.SaveAsync();
                // Gửi thông báo đến người dùng
                var userOfRequestedWarehouseIds = await _unitOfWork.AccountRepository.GetUserIdsByWarehouseAndGroups(request.RequestedWarehouseId, new List<string> { "Thủ kho" });
                await _firebaseService.SendNotificationToUsersAsync(userOfRequestedWarehouseIds, "Yêu cầu mới vừa được tạo.",
                                                                    $"Một yêu cầu kho vừa được tạo với mã yêu cầu: {entity.Code}",
                                                                    NotificationType.GOOD_REQUEST_CREATED, request.RequestedWarehouseId);
                // Nếu là chuyển kho thì notify thêm WarehouseId
                if (request.RequestType == GoodRequestEnum.Transfer)
                {
                    var userOfWarehouseIds = await _unitOfWork.AccountRepository.GetUserIdsByWarehouseAndGroups(request.WarehouseId, new List<string> { "Thủ kho" });
                    await _firebaseService.SendNotificationToUsersAsync(userOfWarehouseIds, "Yêu cầu mới vừa được tạo.",
                                                                        $"Một yêu cầu kho vừa được tạo với mã yêu cầu: {entity.Code}",
                                                                        NotificationType.GOOD_REQUEST_CREATED, request.WarehouseId);
                }
                var goodRequest = await _goodRequestRepository.GetByCondition(x => x.Id == entity.Id, includeProperties: "GoodRequestDetails,Warehouse,RequestedWarehouse,Partner," +
                                                                                                                         "GoodRequestDetails.Product," +
                                                                                                                         "GoodRequestDetails.Product.Unit," +
                                                                                                                         "GoodRequestDetails.Product.Brand");
                var result = _mapper.Map<TResult>(goodRequest);
                // Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                return new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Good request created successfully!",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, rollback transaction
                await _unitOfWork.RollbackTransactionAsync();
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = $"Error creating good request: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse> UpdateAsync(string id, GoodRequestUpdateDTO request)
        {
            // 1️⃣ Tìm `GoodRequest` trong DB
            var entity = await _goodRequestRepository.GetByCondition(x => x.Id == id, includeProperties: "GoodRequestDetails,Warehouse,RequestedWarehouse,Partner," +
                                                                                                         "GoodRequestDetails.Product," +
                                                                                                         "GoodRequestDetails.Product.Unit," +
                                                                                                         "GoodRequestDetails.Product.Brand");

            if (entity == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Không tìm thấy yêu cầu!",
                    Data = id
                };
            }

            // 🔴 Kiểm tra nếu trạng thái không phải PENDING thì không cho chỉnh sửa
            if (entity.Status != GoodRequestStatusEnum.Pending)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = "Chỉ có thể cập nhật phiếu có trạng thái là chờ xử lí.",
                    Data = entity.Status
                };
            }

            if (!string.IsNullOrEmpty(request.Code) && request.Code != entity.Code)
            {
                var existingCode = await _goodRequestRepository.GetByCondition(g => g.Code == request.Code && g.Id == request.Id);
                if (existingCode != null)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Mã yêu cầu đã tồn tại.",
                        Data = request.Code
                    };
                }
            }

            // 2️⃣ Kiểm tra và gán dữ liệu (nếu request null thì giữ nguyên giá trị cũ)
            request.Note = request.Note ?? entity.Note;
            request.Code = request.Code ?? entity.Code;
            request.PartnerId = request.PartnerId ?? entity.PartnerId;
            request.WarehouseId = request.WarehouseId ?? entity.WarehouseId;
            request.RequestedWarehouseId = request.RequestedWarehouseId ?? entity.RequestedWarehouseId;

            // 2️⃣ Cập nhật thông tin chung
            if (!string.IsNullOrEmpty(request.PartnerId))
            {
                var partnerExists = await _unitOfWork.PartnerRepository.GetByCondition(x => x.Id == request.PartnerId);
                if (partnerExists == null)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Partner not found.",
                        Data = request.PartnerId
                    };
                }
            }

            // 3️⃣ Kiểm tra WarehouseId có tồn tại không
            if (!string.IsNullOrEmpty(request.WarehouseId))
            {
                var warehouseExists = await _unitOfWork.WarehouseRepository.GetByCondition(x => x.Id == request.WarehouseId);
                if (warehouseExists == null)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Warehouse not found.",
                        Data = request.WarehouseId
                    };
                }
            }

            // 4️⃣ Kiểm tra RequestedWarehouseId có tồn tại không
            if (!string.IsNullOrEmpty(request.RequestedWarehouseId))
            {
                var requestedWarehouseExists = await _unitOfWork.WarehouseRepository.GetByCondition(x => x.Id == request.RequestedWarehouseId);
                if (requestedWarehouseExists == null)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Requested Warehouse not found.",
                        Data = request.RequestedWarehouseId
                    };
                }
            }

            // 3️⃣ Kiểm tra và cập nhật chi tiết `GoodRequestDetails`
            if (request.GoodRequestDetails != null && request.GoodRequestDetails.Any())
            {
                // 🔥 Xóa từng `GoodRequestDetail` cũ
                foreach (var oldDetail in entity.GoodRequestDetails.ToList())
                {
                    _unitOfWork.GoodRequestDetailRepository.DeletePermanently(oldDetail);
                }

                // 🔥 Thêm `GoodRequestDetails` mới
                //var newDetails = new List<GoodRequestDetail>();

                foreach (var detail in request.GoodRequestDetails)
                {
                    if (string.IsNullOrEmpty(detail.ProductId))
                    {
                        return new ServiceResponse
                        {
                            Status = SRStatus.Error,
                            Message = "ProductId cannot be null or empty in GoodRequestDetails."
                        };
                    }

                    // Kiểm tra sản phẩm có tồn tại không
                    var productExists = await _unitOfWork.ProductRepository.GetByCondition(x => x.Id == detail.ProductId);
                    if (productExists == null)
                    {
                        return new ServiceResponse
                        {
                            Status = SRStatus.Error,
                            Message = $"Product with ID {detail.ProductId} not found.",
                            Data = detail.ProductId
                        };
                    }
                }
            }

            try
            {
                _mapper.Map(request, entity);
                _unitOfWork.GoodRequestRepository.Update(entity);
                await _unitOfWork.SaveAsync();

                var goodnote = await _unitOfWork.GoodNoteRepository.GetByCondition(g => g.GoodRequestId == id);

                var result = _mapper.Map<GoodRequestDTO>(entity);
                result.GoodNoteCount = await _unitOfWork.GoodNoteRepository.Count(g => g.GoodRequestId == id);

                await AttachGoodNoteToGoodRequest(result); // Với 1 đối tượng

                if (goodnote != null)
                {
                    //result.GoodNote = _mapper.Map<GoodNoteOfGoodRequestDTO>(goodnote);
                }
                return new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Good request updated successfully!",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = $"Error updating good request: {ex.Message}"
                };
            }
        }


        public async Task<ServiceResponse> DeleteAsync(string id)
        {
            var entity = await _goodRequestRepository.GetByCondition(x => x.Id == id);
            if (entity == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Good request not found!",
                    Data = id
                };
            }

            try
            {
                _goodRequestRepository.Delete(entity);
                await _unitOfWork.SaveAsync();

                return new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Good request deleted successfully!",
                    Data = id
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = $"Error deleting good request: {ex.Message}"
                };
            }
        }
        public async Task<ServiceResponse> SearchGoodRequests(int? pageIndex = null, int? pageSize = null,
                                                                       string? keyword = null, GoodRequestEnum? requestType = null,
                                                                                               GoodRequestStatusEnum? status = null)
        {
            try
            {
                Expression<Func<GoodRequest, bool>> filter = g =>
                                                    (string.IsNullOrEmpty(keyword) || (
                                                    (g.Code != null && g.Code.Contains(keyword)) ||  // ✅ Tìm theo Code
                                                    (g.Note != null && g.Note.Contains(keyword)) ||
                                                    (g.Warehouse != null && g.Warehouse.Name.Contains(keyword)) ||
                                                    (g.RequestedWarehouse != null && g.RequestedWarehouse.Name.Contains(keyword)) ||
                                                    (g.Partner != null && g.Partner.Name.Contains(keyword)))) &&
                                                    (!requestType.HasValue || g.RequestType == requestType.Value) &&  // ✅ Lọc theo loại yêu cầu
                                                    (!status.HasValue || g.Status == status.Value);                   // ✅ Lọc theo trạng thái yêu cầu
                var totalRecords = await _goodRequestRepository.Count(filter);

                var results = await _goodRequestRepository.Search(
                    filter: filter,
                    orderBy: g => g.OrderByDescending(x => x.CreatedTime),
                    includeProperties: "GoodRequestDetails,Warehouse,RequestedWarehouse,Partner,GoodRequestDetails.Product," +
                                       "GoodRequestDetails.Product.Unit,GoodRequestDetails.Product.Brand",
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );

                var mappedResults = _mapper.Map<IEnumerable<GoodRequestDTO>>(results);

                foreach (var goodRequest in mappedResults)
                {
                    //var goodNote = await _unitOfWork.GoodNoteRepository.GetByCondition(g => g.GoodRequestId == goodRequest.Id);
                    //if (goodNote != null)
                    //{
                    //    //goodRequest.GoodNote = _mapper.Map<GoodNoteOfGoodRequestDTO>(goodNote);
                    //}
                    await AttachGoodNoteToGoodRequest(goodRequest);
                    goodRequest.GoodNoteCount = await _unitOfWork.GoodNoteRepository.Count(g => g.GoodRequestId == goodRequest.Id);

                }

                int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? 5));

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
                        Records = mappedResults
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Error,
                    Message = "An error occurred while searching GoodRequests. Please try again later." + ex,
                    Data = null
                };
            }
        }
        public async Task<ServiceResponse> UpdateStatusAsync(string id, GoodRequestStatusEnum newStatus)
        {
            var goodRequest = await _goodRequestRepository.GetByCondition(x => x.Id == id);
            if (goodRequest == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "GoodRequest không tồn tại.",
                    Data = id
                };
            }

            // Kiểm tra quy tắc cập nhật trạng thái
            if (!CanUpdateStatus(goodRequest.Status, newStatus))
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = $"Không thể chuyển từ trạng thái {goodRequest.Status.ToString()} sang {newStatus.ToString()}.",
                    Data = id
                };
            }
            // Kiểm tra quy tắc cập nhật trạng thái complete
            if(newStatus == GoodRequestStatusEnum.Completed)
            {
                var goodNote = await _unitOfWork.GoodNoteRepository.GetByCondition(g => g.GoodRequestId == id);
                if (goodNote == null)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Không thể hoàn thành yêu cầu này vì không có phiếu giao hàng nào được tạo cho yêu cầu này.",
                        Data = id
                    };
                }
            }

            goodRequest.Status = newStatus;
            _goodRequestRepository.Update(goodRequest);
            await _unitOfWork.SaveAsync();
            // Gửi thông báo đến người dùng
            //var userIds = await _unitOfWork.AccountRepository.GetUserIdsByRequestedWarehouseAndGroups(goodRequest.RequestedWarehouseId, new List<string> { "Nhân viên bán hàng" });
            switch (newStatus)
            {
                case GoodRequestStatusEnum.Approved:
                    await _firebaseService.SendNotificationToUsersAsync(new List<string> { goodRequest.CreatedBy }, "Yêu cầu kho đã được phê duyệt.", $"Yêu cầu kho với mã yêu cầu: {goodRequest.Code} đã được phê duyệt.", NotificationType.GOOD_REQUEST_APPROVED, goodRequest.RequestedWarehouseId);
                    break;
                case GoodRequestStatusEnum.Rejected:
                    await _firebaseService.SendNotificationToUsersAsync(new List<string> { goodRequest.CreatedBy }, "Yêu cầu kho đã bị từ chối.", $"Yêu cầu kho với mã yêu cầu: {goodRequest.Code} đã bị từ chối.", NotificationType.GOOD_REQUEST_REJECTED, goodRequest.RequestedWarehouseId);
                    break;
                case GoodRequestStatusEnum.Completed:
                    await _firebaseService.SendNotificationToUsersAsync(new List<string> { goodRequest.CreatedBy }, "Yêu cầu kho đã hoàn thành.", $"Yêu cầu kho với mã yêu cầu: {goodRequest.Code} đã hoàn thành.", NotificationType.GOOD_REQUEST_CONFIRMED, goodRequest.RequestedWarehouseId);
                    break;
                case GoodRequestStatusEnum.Pending:
                    break;
                default:
                    break;
            }
            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Cập nhật trạng thái thành công.",
                Data = new { goodRequestId = id, newStatus = goodRequest.Status }
            };
        }
        private bool CanUpdateStatus(GoodRequestStatusEnum currentStatus, GoodRequestStatusEnum newStatus)
        {
            var validTransitions = new Dictionary<GoodRequestStatusEnum, List<GoodRequestStatusEnum>>()
            {
                { GoodRequestStatusEnum.Pending, new List<GoodRequestStatusEnum> { GoodRequestStatusEnum.Approved,/* GoodRequestStatusEnum.Canceled,*/ GoodRequestStatusEnum.Rejected } },
                { GoodRequestStatusEnum.Approved, new List<GoodRequestStatusEnum> { GoodRequestStatusEnum.Completed, /*GoodRequestStatusEnum.Canceled*/ } },
                { GoodRequestStatusEnum.Issued, new List<GoodRequestStatusEnum> { GoodRequestStatusEnum.Completed } },
                //{ GoodRequestStatusEnum.Rejected, new List<GoodRequestStatusEnum> { GoodRequestStatusEnum.Pending } },
                //{ GoodRequestStatusEnum.Canceled, new List<GoodRequestStatusEnum>() }, // Không thể đổi từ Canceled                
                { GoodRequestStatusEnum.Completed, new List<GoodRequestStatusEnum>() }, // Không thể đổi từ Completed
                { GoodRequestStatusEnum.Rejected, new List<GoodRequestStatusEnum>() } // Không thể đổi từ Rejected
            };

            return validTransitions.ContainsKey(currentStatus) && validTransitions[currentStatus].Contains(newStatus);
        }

        private async Task AttachGoodNoteToGoodRequest(GoodRequestDTO goodRequest)
        {
            var goodNotes = await _unitOfWork.GoodNoteRepository.Search(g => g.GoodRequestId == goodRequest.Id);
            if (!goodNotes.Any()) return;

            var goodNoteDTOs = new List<GoodNoteDTOv2>();

            foreach (var goodNote in goodNotes)
            {
                var entities = await _unitOfWork.GoodNoteDetailRepository.Search(
                    g => g.GoodNoteId == goodNote.Id,
                    includeProperties: "GoodNote," +
                                       "GoodNote.GoodRequest," +
                                       "GoodNote.GoodRequest.Warehouse," +
                                       "GoodNote.GoodRequest.RequestedWarehouse," +
                                       "GoodNote.GoodRequest.Partner," +
                                       "Batch," +
                                       "Batch.Product," +
                                       "Batch.Product.Unit," +
                                       "Batch.Product.Brand");

                var goodNoteDTO = new GoodNoteDTOv2
                {
                    Id = goodNote.Id,
                    ReceiverName = goodNote.ReceiverName,
                    ShipperName = goodNote.ShipperName,
                    NoteType = goodNote.NoteType,
                    Status = goodNote.Status,
                    Code = goodNote.Code,
                    Date = goodNote.Date,
                    CreatedTime = goodNote.CreatedTime.ToString(),
                    CreatedBy = goodNote.CreatedBy,
                    GoodNoteDetails = _mapper.Map<List<GoodNoteDetailDTO>>(entities)
                };

                goodNoteDTOs.Add(goodNoteDTO);
            }

            goodRequest.GoodNotes = goodNoteDTOs;
        }
    }
}