using BusinessLogicLayer.Generic;
using Data.Enum;
using Data.Model.DTO;
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
                                                                   string? keyword = null, InventoryCountStatus? status = null);
        Task<InventoryCountDTO> AddInventoryCount(InventoryCountCreateDTO request);
        Task<InventoryCountDTO> UpdateInventoryCount(InventoryCountUpdateDTO request);
        Task<InventoryByLocationDTO> GetInventoriesByLocationLevel0Async(string locationLevel0Id);
    }
}
