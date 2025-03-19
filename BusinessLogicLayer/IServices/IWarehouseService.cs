using BusinessLogicLayer.Generic;
using Data.Model.Request.Warehouse;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IWarehouseService : IGenericService
    {
        Task<ServiceResponse> GetFullWarehouseInfo<TResult>(string id);
        Task<ServiceResponse> CreateStructureAsync(CreateWarehouseStructureRequest request);
        Task<ServiceResponse> SearchWarehouses<TResult>(
                                                        int? pageIndex = null,
                                                        int? pageSize = null,
                                                        string? keyword = null,
                                                        float? minArea = null,
                                                        float? maxArea = null);

    }
}
