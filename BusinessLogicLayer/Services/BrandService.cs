using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.DTO.Base;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class BrandService : GenericService<Brand>, IBrandService
    {
        public BrandService(IGenericRepository<Brand> genericRepository, IMapper mapper, IUnitOfWork unitOfWork)
            : base(genericRepository, mapper, unitOfWork)
        {
        }

        public override async Task<ServiceResponse> Get<TResult>()
        {
            var brands = await _genericRepository.GetAllNoPaging();

            List<TResult> mappedResults = _mapper.Map<List<TResult>>(brands);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = mappedResults
            };
        }

        public override async Task<ServiceResponse> Get<TResult>(string id)
        {
            var brand = await _genericRepository.GetByCondition(b => b.Id == id);

            if (brand == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Brand not found!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(brand);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = result
            };
        }

    }
}
