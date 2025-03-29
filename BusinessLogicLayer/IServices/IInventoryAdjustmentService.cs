using BusinessLogicLayer.Generic;
using Data.Model.DTO;
using Data.Model.Request.InventoryAdjustment;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IInventoryAdjustmentService : IGenericService
    {
        Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null);
        Task<InventoryAdjustmentDTO> AddInventoryAdjustment(InventoryAdjustmentCreateDTO request);
        Task<InventoryAdjustmentDTO> UpdateInventoryAdjustment(InventoryAdjustmentUpdateDTO request);
    }
}
