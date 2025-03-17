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
        //Task<ServiceResponse> GetWarehouseAreas(string warehouseId);
        //Task<ServiceResponse> AddWarehouseArea(Data.Model.Request.Area.CreateAREADto request);
        //Task<ServiceResponse> GetShelvesByArea(string areaId);
        //Task<ServiceResponse> AddShelfWithStructure(CreateShelfDto request, string id);
        //Task<ServiceResponse> GetShelfDetails(string shelfId);

    }
}
