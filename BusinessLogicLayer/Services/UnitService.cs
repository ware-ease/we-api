using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.ProductType;
using Data.Model.Request.Unit;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class UnitService : GenericService<Unit>, IUnitService
    {
        public UnitService(IGenericRepository<Unit> genericRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {

        }

        public override async Task<ServiceResponse> Get<TResult>()
        {
            var units = await _genericRepository.GetAllNoPaging();

            List<TResult> mappedResults = _mapper.Map<List<TResult>>(units);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = mappedResults
            };
        }

        public override async Task<ServiceResponse> Get<TResult>(string id)
        {
            var unit = await _genericRepository.GetByCondition(b => b.Id == id);

            if (unit == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Unit not found!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(unit);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = result
            };
        }

        public async Task<UnitDTO> UpdateUnit(UnitUpdateDTO request)
        {
            var existedUnit = await _genericRepository.GetByCondition(p => p.Id == request.Id);
            if (existedUnit == null)
                throw new Exception("Unit không tồn tại");

            if (!string.IsNullOrEmpty(request.Name))
            {
                existedUnit.Name = request.Name;
            }
            if (!string.IsNullOrEmpty(request.Note))
            {
                existedUnit.Note = request.Note;
            }

            _genericRepository.Update(existedUnit);
            await _unitOfWork.SaveAsync();


            var updatedUnit = await _genericRepository.GetByCondition(p => p.Id == existedUnit.Id);
            if (updatedUnit == null)
                throw new Exception("Update failed, Unit not found after update");

            return _mapper.Map<UnitDTO>(updatedUnit);
        }

        public async Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null)
        {

            Expression<Func<Unit, bool>> filter = p =>
                (string.IsNullOrEmpty(keyword) 
                || (p.Name + " " + p.Note).Contains(keyword));

            var totalRecords = await _genericRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize);

            var mappedResults = _mapper.Map<IEnumerable<TResult>>(results);

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
