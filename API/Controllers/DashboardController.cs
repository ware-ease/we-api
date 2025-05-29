using API.Middlewares;
using BusinessLogicLayer.IServices;
using Data.Model.Request.Warehouse;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace API.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public DashboardController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpGet("4cards")]
        public async Task<IActionResult> Get4CardsInfo(string? warehouseId)
        {
            var result = await _warehouseService.GetWarehouseStatisticsAsync(warehouseId);
            return ControllerResponse.Response(result);
        }
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpGet("histogram")]
        public async Task<IActionResult> GetGoodsFlowHistogram(string? warehouseId, int? month, int? year)
        {
            //var response = await _warehouseService.GetGoodsFlowHistogramAsync(warehouseId);
            var response = await _warehouseService.GetGoodsFlowHistogramAsync(month,year,warehouseId);
            return ControllerResponse.Response(response);
        }
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpGet("linechart")]
        public async Task<IActionResult> GetStockLineChart(int? year, int? startMonth, int? endMonth, string? warehouseId)
        {
            var response = await _warehouseService.GetStockLineChartAsync(year, startMonth, endMonth, warehouseId);
            return ControllerResponse.Response(response);
        }
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpGet("piechart")]
        public async Task<IActionResult> GetStockPieChart(string? half = "first", int year = 2025)
        {
           // var response = await _warehouseService.GetStockPieChartAsync();
            var response = await _warehouseService.GetStockDistributionByWarehouseAsync(year,half);
            return ControllerResponse.Response(response);
        }
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpGet("piechart/warehouse/{warehouseId}")]
        public async Task<IActionResult> GetStockPieChartOfAWarehouse(string warehouseId, string? half = "first", int year = 2025)
        {
            var response = await _warehouseService.GetStockPieChartByWarehouseAsync(warehouseId, year, half);
            return ControllerResponse.Response(response);
        }
    }
}
