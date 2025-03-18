using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.ProductType;
using Data.Model.Request.Unit;
using DataAccessLayer.Generic;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<UnitDTO> UpdateUnit(UnitUpdateDTO request)
        {
            var existedUnit = await _genericRepository.Get(request.Id);
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


            var updatedUnit = await _genericRepository.Get(existedUnit.Id);
            if (updatedUnit == null)
                throw new Exception("Update failed, Unit not found after update");

            return _mapper.Map<UnitDTO>(updatedUnit);
        }

    }
}
