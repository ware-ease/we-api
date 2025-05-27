using BusinessLogicLayer.IServices;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/inventories")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IWarehouseService _warehouseService;

        public InventoryController(IInventoryService inventoryService, IWarehouseService warehouseService)
        {
            _inventoryService = inventoryService;
            _warehouseService = warehouseService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SearchInventories([FromQuery] int pageIndex = 1,
                                                        [FromQuery] int pageSize = 5,
                                                        [FromQuery] string? keyword = null,
                                                        [FromQuery] string? warehouseId = null,
                                                        [FromQuery] string? productId = null)
        {
            var response = await _inventoryService.Search(
                pageIndex, pageSize, keyword, warehouseId, productId);

            return ControllerResponse.Response(response);
        }
    }
}
