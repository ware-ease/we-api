using BusinessLogicLayer.Generic;
using Data.Entity;
using Data.Model.Request.InventoryCount;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IInventoryCountService : IGenericService
    {
        Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null, bool? status = null);
        Task<InventoryCountDTO> AddInventoryCount(InventoryCountCreateDTO request);
        Task<InventoryCountDTO> UpdateInventoryCount(InventoryCountUpdateDTO request);
    }
}
