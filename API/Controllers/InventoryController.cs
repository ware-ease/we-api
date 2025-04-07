using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Services;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.InventoryLocation;
using Data.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;

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

        [HttpGet]
        public async Task<IActionResult> SearchPartners([FromQuery] int pageIndex = 1,
                                                        [FromQuery] int pageSize = 5,
                                                        [FromQuery] string? keyword = null)
        {
            var response = await _inventoryService.Search<Data.Model.DTO.InventoryDTO>(
                pageIndex, pageSize, keyword);

            return ControllerResponse.Response(response);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> Get(string id)
        //{
        //    try
        //    {
        //        var inventory = await _inventoryService.Get<Data.Model.DTO.InventoryDTO>(id);
        //        return ControllerResponse.Response(inventory);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ControllerResponse.Response(new ServiceResponse
        //        {
        //            Status = SRStatus.Error,
        //            Message = ex.Message,
        //            Data = null
        //        });
        //    }
        //}
        //[Authorize]
        [HttpPost("/inventory-location")]
        public async Task<IActionResult> PutAwayInvento([FromBody] CreateInventoryLocationDTO request)
        {
            var result = await _warehouseService.InventoryLocationInOutAsync(request);
            return ControllerResponse.Response(result);
        }
        [HttpGet("locations/{locationId}")]
        public async Task<IActionResult> GetInventoriesInLocation([FromRoute] string locationId)
        {
            var result = await _warehouseService.GetInventoriesInLocation(locationId);
            return ControllerResponse.Response(result);
        }
        [HttpGet("{inventoryId}")]
        public async Task<IActionResult> GetLocationsByInventoryId(string inventoryId)
        {
            var response = await _inventoryService.GetLocationsByInventoryId(inventoryId);

            return ControllerResponse.Response(response);
        }
        [HttpGet("batch/{batchId}/locations")]
        public async Task<IActionResult> GetLocationsByBatchId(string batchId)
        {
            var response = await _inventoryService.GetLocationsByBatchId(batchId);

            return ControllerResponse.Response(response);
        }

    }
}
