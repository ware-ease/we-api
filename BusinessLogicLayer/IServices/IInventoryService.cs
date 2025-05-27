using BusinessLogicLayer.Generic;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IInventoryService : IGenericService
    {
        Task<ServiceResponse> Search(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null, string? warehouseId = null, string? productId = null);
    }
}
