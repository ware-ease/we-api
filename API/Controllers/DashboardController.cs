﻿using BusinessLogicLayer.IServices;
using Data.Model.Request.Warehouse;
using Data.Model.Response;
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

        [HttpGet("4cards")]
        public async Task<IActionResult> Get4CardsInfo(string? warehouseId)
        {
            var result = await _warehouseService.GetWarehouseStatisticsAsync(warehouseId);
            return ControllerResponse.Response(result);
        }
        [HttpGet("histogram")]
        public async Task<IActionResult> GetGoodsFlowHistogram(string? warehouseId, int? month, int? year)
        {
            //var response = await _warehouseService.GetGoodsFlowHistogramAsync(warehouseId);
            var response = await _warehouseService.GetGoodsFlowHistogramAsync(month,year,warehouseId);
            return ControllerResponse.Response(response);
        }
        [HttpGet("linechart")]
        public async Task<IActionResult> GetStockLineChart(int? year, int? startMonth, int? endMonth, string? warehouseId)
        {
            var response = await _warehouseService.GetStockLineChartAsync(year, startMonth, endMonth, warehouseId);
            return ControllerResponse.Response(response);
        }
        [HttpGet("piechart")]
        public async Task<IActionResult> GetStockPieChart()
        {
            var response = await _warehouseService.GetStockPieChartAsync();
            return ControllerResponse.Response(response);
        }
        [HttpGet("piechart/warehouse/{warehouseId}")]
        public async Task<IActionResult> GetStockPieChartOfAWarehouse(string warehouseId)
        {
            var response = await _warehouseService.GetStockPieChartByWarehouseAsync(warehouseId);
            return ControllerResponse.Response(response);
        }
    }
}
