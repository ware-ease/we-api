using BusinessLogicLayer.IServices;
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
        public async Task<IActionResult> Get4CardsInfo()
        {
            var result = await _warehouseService.GetWarehouseStatisticsAsync(null);

            return ControllerResponse.Response(result);
        }
        [HttpGet("histogram")]
        public async Task<IActionResult> GetGoodsFlowHistogram()
        {
            var response = await _warehouseService.GetGoodsFlowHistogramAsync(null);
            return ControllerResponse.Response(response);
        }

    }
}
