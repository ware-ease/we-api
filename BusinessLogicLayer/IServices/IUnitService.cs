using BusinessLogicLayer.Generic;
using Data.Model.DTO;
using Data.Model.Request.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IUnitService : IGenericService
    {
        Task<UnitDTO> UpdateUnit(UnitUpdateDTO request);
    }
}
