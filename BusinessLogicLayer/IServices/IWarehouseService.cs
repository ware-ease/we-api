﻿using BusinessLogicLayer.Generic;
using Data.Model.Request.InventoryLocation;
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
        Task<ServiceResponse> GetInventoryByWarehouseId(string warehouseId);
        Task<ServiceResponse> InventoryLocationInOutAsync(CreateInventoryLocationDTO request);
        Task<ServiceResponse> GetInventoriesInLocation(string locationId);
        Task<ServiceResponse> GetLocationLogsAsync(string warehouseId, string? locationId, int pageIndex, int pageSize);
    }
}
