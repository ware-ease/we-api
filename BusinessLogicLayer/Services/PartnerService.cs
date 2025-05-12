using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Enum;
using Data.Model.Request.Partner;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.UnitOfWork;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class PartnerService : GenericService<Partner>, IPartnerService
    {
        private readonly IPartnerRepository _partnerRepository;
        //private readonly IMapper _mapper;
        //private readonly IUnitOfWork _unitOfWork;

        public PartnerService(IGenericRepository<Partner> genericRepository, IUnitOfWork unitOfWork, IMapper mapper) : base(genericRepository, mapper, unitOfWork)
        {
            _partnerRepository = unitOfWork.PartnerRepository;
            //_mapper = mapper;
            //_unitOfWork = unitOfWork;
        }
        public override async Task<ServiceResponse> Get<TResult>()
        {
            var results = await _genericRepository.Search();

            IEnumerable<TResult> mappedResults = _mapper.Map<IEnumerable<TResult>>(results);

            foreach (var mappedResult in mappedResults)
            {
                if (mappedResult.CreatedBy != null)
                {
                    var createdBy = await GetCreatedBy(mappedResult.CreatedBy);

                    if (createdBy != null)
                    {
                        mappedResult.CreatedBy = createdBy!.Username;
                    }
                    else
                    {
                        mappedResult.CreatedBy = "Deleted user";
                    }
                }
            }

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = mappedResults
            };
        }

        public async Task<ServiceResponse> GetAll<TResult>()
        {
            var entities = await _partnerRepository.GetAllNoPaging();
            var result = _mapper.Map<List<TResult>>(entities);
            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Get all partners successfully!",
                Data = result
            };
        }

        public async Task<ServiceResponse> GetById<TResult>(string id)
        {
            var entity = await _partnerRepository.GetByCondition(x => x.Id == id);
            if (entity == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Partner not found!",
                    Data = id
                };
            }

            var result = _mapper.Map<TResult>(entity);
            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Get partner successfully!",
                Data = result
            };
        }

        public async Task<ServiceResponse> CreateAsync<TResult>(PartnerCreateDTO request)
        {
            var entity = _mapper.Map<Partner>(request);
            entity.CreatedTime = DateTime.Now;

            try
            {
                await _partnerRepository.Add(entity);
                await _unitOfWork.SaveAsync();

                var result = _mapper.Map<TResult>(entity);
                return new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Partner created successfully!",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = $"Error creating partner: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse> UpdateAsync<TResult>(string id, PartnerUpdateDTO request)
        {
            var entity = await _partnerRepository.GetByCondition(x => x.Id == id);
            if (entity == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Partner not found!",
                    Data = id
                };
            }

            // Nếu request null, giữ nguyên dữ liệu cũ
            request.Name = request.Name ?? entity.Name;
            request.Phone = request.Phone ?? entity.Phone;
            request.Status ??= entity.Status;
            request.PartnerType ??= entity.PartnerType;

            try
            {
                _mapper.Map(request, entity);
                _partnerRepository.Update(entity);
                await _unitOfWork.SaveAsync();

                var result = _mapper.Map<TResult>(entity);
                return new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Partner updated successfully!",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = $"Error updating partner: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse> DeleteAsync(string id)
        {
            var entity = await _partnerRepository.GetByCondition(p => p.Id == id);
            if (entity == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Partner not found!",
                    Data = id
                };
            }

            try
            {
                _partnerRepository.Delete(entity);
                await _unitOfWork.SaveAsync();

                return new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Partner deleted successfully!",
                    Data = id
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = $"Error deleting partner: {ex.Message}"
                };
            }
        }
        public async Task<ServiceResponse> SearchPartners(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null, int? partnerType = null)
        {
            PartnerEnum? partnerTypeEnum = partnerType.HasValue ? (PartnerEnum?)partnerType.Value : null;

            Expression<Func<Partner, bool>> filter = p =>
                (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)) &&
                (!partnerTypeEnum.HasValue || p.PartnerType == partnerTypeEnum.Value);

            var totalRecords = await _partnerRepository.Count(filter);

            var results = await _partnerRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize);

            var mappedResults = _mapper.Map<IEnumerable<PartnerDTO>>(results);

            foreach (var mappedResult in mappedResults)
            {
                var createdByAccount = await _unitOfWork.AccountRepository.GetByCondition(a => a.Id == mappedResult.CreatedBy, "Profile,AccountGroups,AccountGroups.Group");
                if (createdByAccount != null)
                {
                    mappedResult.CreatedByAvatarUrl = createdByAccount.Profile!.AvatarUrl;
                    mappedResult.CreatedByFullName = $"{createdByAccount.Profile.FirstName} {createdByAccount.Profile.LastName}";
                    mappedResult.CreatedByGroup = createdByAccount.AccountGroups.FirstOrDefault()?.Group?.Name;
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
    }
}
