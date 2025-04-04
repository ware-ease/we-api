﻿using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Enum;
using Data.Model.Request.GoodRequest;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class GoodRequestService : GenericService<GoodRequest>, IGoodRequestService
    {
        private readonly IGoodRequestRepository _goodRequestRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GoodRequestService(IGenericRepository<GoodRequest> genericRepository, IUnitOfWork unitOfWork, IMapper mapper) : base(genericRepository, mapper, unitOfWork)
        {
            _goodRequestRepository = unitOfWork.GoodRequestRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ServiceResponse> GetById(string id)
        {
            var entity = await _goodRequestRepository.GetByCondition(g => g.Id == id, includeProperties: "GoodRequestDetails,Warehouse,Partner," +
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
            if (goodNote != null)
            {
                result.GoodNote = _mapper.Map<GoodNoteOfGoodRequestDTO>(goodNote);
            }
            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Get good request successfully!",
                Data = result
            };
        }

        public async Task<ServiceResponse> CreateAsync<TResult>(GoodRequestCreateDTO request)
        {
            // Kiểm tra Code có bị trùng không
            var existingCode = await _goodRequestRepository.GetByCondition(g => g.Code == request.Code);
            if (existingCode != null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = "Good request code already exists.",
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
                            Message = "ProductId cannot be null or empty in GoodRequestDetails."
                        };
                    }

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

            // 6️⃣ Mapping và thêm vào DB
            var entity = _mapper.Map<GoodRequest>(request);
            entity.CreatedTime = DateTime.Now;

            try
            {
                await _goodRequestRepository.Add(entity);
                await _unitOfWork.SaveAsync();

                var goodRequest = await _goodRequestRepository.GetByCondition(x => x.Id == entity.Id, includeProperties: "GoodRequestDetails,Warehouse,Partner," +
                                                                                                                         "GoodRequestDetails.Product," +
                                                                                                                         "GoodRequestDetails.Product.Unit," +
                                                                                                                         "GoodRequestDetails.Product.Brand");
                var result = _mapper.Map<TResult>(goodRequest);
                return new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Good request created successfully!",
                    Data = result
                };
            }
            catch (Exception ex)
            {
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
            var entity = await _goodRequestRepository.GetByCondition(x => x.Id == id, includeProperties: "GoodRequestDetails,Warehouse,Partner," +
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

            // 🔴 Kiểm tra nếu trạng thái không phải PENDING thì không cho chỉnh sửa
            if (entity.Status != GoodRequestStatusEnum.Pending)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = "Only requests with status 'Pending' can be updated.",
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
                        Message = "Good request code already exists.",
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
                if (goodnote != null)
                {
                    result.GoodNote = _mapper.Map<GoodNoteOfGoodRequestDTO>(goodnote);
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
                    includeProperties: "GoodRequestDetails,Warehouse,Partner,GoodRequestDetails.Product," +
                                       "GoodRequestDetails.Product.Unit,GoodRequestDetails.Product.Brand",
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );

                var mappedResults = _mapper.Map<IEnumerable<GoodRequestDTO>>(results);

                foreach (var goodRequest in mappedResults)
                {
                    var goodNote = await _unitOfWork.GoodNoteRepository.GetByCondition(g => g.GoodRequestId == goodRequest.Id);
                    if (goodNote != null)
                    {
                        goodRequest.GoodNote = _mapper.Map<GoodNoteOfGoodRequestDTO>(goodNote);
                    }
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
                    Message = "An error occurred while searching GoodRequests. Please try again later.",
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

            goodRequest.Status = newStatus;
            _goodRequestRepository.Update(goodRequest);
            await _unitOfWork.SaveAsync();

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
                { GoodRequestStatusEnum.Pending, new List<GoodRequestStatusEnum> { GoodRequestStatusEnum.Approved, GoodRequestStatusEnum.Canceled, GoodRequestStatusEnum.Rejected } },
                { GoodRequestStatusEnum.Approved, new List<GoodRequestStatusEnum> { GoodRequestStatusEnum.Completed, GoodRequestStatusEnum.Canceled } },
                { GoodRequestStatusEnum.Canceled, new List<GoodRequestStatusEnum>() }, // Không thể đổi từ Canceled                
                { GoodRequestStatusEnum.Completed, new List<GoodRequestStatusEnum>() }, // Không thể đổi từ Completed
                { GoodRequestStatusEnum.Rejected, new List<GoodRequestStatusEnum>() } // Không thể đổi từ Rejected
            };

            return validTransitions.ContainsKey(currentStatus) && validTransitions[currentStatus].Contains(newStatus);
        }
    }
}