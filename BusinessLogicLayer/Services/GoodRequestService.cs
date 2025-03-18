using AutoMapper;
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

        public async Task<ServiceResponse> GetAll<TResult>()
        {
            var entities = await _goodRequestRepository.GetAllNoPaging();
            var result = _mapper.Map<List<TResult>>(entities);

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Get all good requests successfully!",
                Data = result
            };
        }

        public async Task<ServiceResponse> GetById<TResult>(string id)
        {
            var entity = await _goodRequestRepository.GetByCondition(g => g.Id == id, includeProperties: "GoodRequestDetails");
            if (entity == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Good request not found!",
                    Data = id
                };
            }

            var result = _mapper.Map<TResult>(entity);
            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Get good request successfully!",
                Data = result
            };
        }

        public async Task<ServiceResponse> CreateAsync<TResult>(GoodRequestCreateDTO request)
        {
            // 2️⃣ Kiểm tra PartnerId có tồn tại trong DB không (nếu có giá trị)
            if (!string.IsNullOrEmpty(request.PartnerId))
            {
                var partnerExists = await _unitOfWork.PartnerRepository.Get(request.PartnerId);
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
                var requestedWarehouseExists = await _unitOfWork.WarehouseRepository.Get(request.RequestedWarehouseId);
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

                    var productExists = await _unitOfWork.ProductRepository.Get(detail.ProductId);
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

                var result = _mapper.Map<TResult>(entity);
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

        public async Task<ServiceResponse> UpdateAsync<TResult>(string id, GoodRequestUpdateDTO request)
        {
            // 1️⃣ Tìm `GoodRequest` trong DB
            var entity = await _goodRequestRepository.GetByCondition(x => x.Id == id, includeProperties: "GoodRequestDetails");

            if (entity == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Good request not found!",
                    Data = id
                };
            }

            // 2️⃣ Cập nhật thông tin chung
            if (!string.IsNullOrEmpty(request.PartnerId))
            {
                var partnerExists = await _unitOfWork.PartnerRepository.Get(request.PartnerId);
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
                var requestedWarehouseExists = await _unitOfWork.WarehouseRepository.Get(request.RequestedWarehouseId);
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
                await _unitOfWork.SaveAsync();
                var result = _mapper.Map<TResult>(entity);

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
    }
}