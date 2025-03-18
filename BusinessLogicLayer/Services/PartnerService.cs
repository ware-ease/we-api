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
using System;
using System.Collections.Generic;
using System.Linq;
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
            var entity = await _partnerRepository.Get(id);
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
    }
}
