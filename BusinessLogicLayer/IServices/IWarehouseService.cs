using BusinessLogicLayer.Generic;
using Data.Model.Request.Warehouse;
using Data.Model.Response;

namespace BusinessLogicLayer.IServices
{
    public interface IWarehouseService : IGenericService
    {
        Task<ServiceResponse> GetFullWarehouseInfo<TResult>(string id);
        Task<ServiceResponse> SearchWarehouses( int? pageIndex = null,
                                                int? pageSize = null,
                                                string? keyword = null,
                                                float? minArea = null,
                                                float? maxArea = null);
        Task<ServiceResponse> GetInventoryByWarehouseId(string warehouseId);
        Task<ServiceResponse> GetWarehouseStatisticsAsync(string? warehouseId);
        //Task<ServiceResponse> GetGoodsFlowHistogramAsync(string? warehouseId);
        Task<ServiceResponse> GetStockCard(string productId, string warehouseId, DateTime? from = null, DateTime? to = null);
        Task<ServiceResponse> GetGoodsFlowHistogramAsync(int? month, int? year, string? warehouseId);
        Task<ServiceResponse> GetStockLineChartAsync(int? year, int? startMonth, int? endMonth, string? warehouseId);
        Task<ServiceResponse> GetStockPieChartAsync();
        Task<ServiceResponse> GetStockPieChartByWarehouseAsync(string warehouseId);
        Task<ServiceResponse> GetStockBookAsync(string warehouseId, int month, int year, string userId);
        Task<ServiceResponse> GetAvailableProductsInWarehouse(string warehouseId);
        Task<ServiceResponse> GetStaffsOfWarehouse(string warehouseId, 
                                                    int? pageIndex = null,
                                                    int? pageSize = null,
                                                    string? keyword = null);

    }
}
