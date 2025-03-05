using AutoMapper;
using Data.Entity.Base;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Generic
{
    public abstract class GenericService<TEntity> : IGenericService where TEntity : BaseEntity
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _genericRepository;
        private readonly IMapper _mapper;

        public GenericService(IGenericRepository<TEntity> genericRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = _unitOfWork.GetRepository<TEntity>();
            _mapper = mapper;
        }

        public virtual async Task<ServiceResponse> Add<TResult, TRequest>(TRequest request)
        {
            TEntity entity = _mapper.Map<TEntity>(request);

            TResult result = _mapper.Map<TResult>(entity);

            try
            {
                entity.Id = Guid.NewGuid().ToString();
                await _genericRepository.Add(entity);
                await _unitOfWork.SaveAsync();

                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Add successfully!",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Error,
                    Message = ex.Message,
                    Data = request
                };
            }
        }

        public virtual async Task<ServiceResponse> Delete(string id)
        {
            TEntity? entity = await _genericRepository.Get(id);

            if (entity != null)
            {
                entity.IsDeleted = true;
                _genericRepository.Update(entity);
                await _unitOfWork.SaveAsync();

                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Delete successfully!",
                    Data = { }
                };
            }

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.NotFound,
                Message = "Not found!",
                Data = { }
            };
        }

        public virtual async Task<ServiceResponse> Get<TResult>()
        {
            var results = _genericRepository.Get();

            List<TResult> mappedResults = _mapper.Map<List<TResult>>(results);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = mappedResults
            };
        }

        public virtual async Task<ServiceResponse> Get<TResult>(string id)
        {
            TEntity? entity = await _genericRepository.Get(id);

            TResult result = _mapper.Map<TResult>(entity);

            if (entity != null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Get successfully!",
                    Data = result
                };
            }

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.NotFound,
                Message = "Not found!",
                Data = id
            };
        }

        public virtual async Task<ServiceResponse> Update<TResult, TRequest>(TRequest request)
        {
            TEntity entity = _mapper.Map<TEntity>(request);

            TResult result = _mapper.Map<TResult>(entity);

            try
            {
                _genericRepository.Update(entity);
                await _unitOfWork.SaveAsync();

                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Update successfully!",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Error,
                    Message = ex.Message,
                    Data = request
                };
            }
        }
    }
}
