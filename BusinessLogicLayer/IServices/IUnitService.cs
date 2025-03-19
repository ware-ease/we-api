using BusinessLogicLayer.Generic;
using Data.Model.DTO;
using Data.Model.Request.Unit;
using Data.Model.Response;
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
        Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null);
    }
}
