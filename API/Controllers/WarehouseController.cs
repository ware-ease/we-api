﻿using API.Utils;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Utils;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.Area;
using Data.Model.Request.Customer;
using Data.Model.Request.InventoryLocation;
using Data.Model.Request.Warehouse;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace API.Controllers
{
    [Route("api/warehouses")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }
        [HttpGet()]
        public async Task<IActionResult> SearchWarehouses(
                                                            [FromQuery] int pageIndex = 1,
                                                            [FromQuery] int pageSize = 5,
                                                            [FromQuery] string? keyword = null,
                                                            [FromQuery] float? minArea = null,
                                                            [FromQuery] float? maxArea = null)
        {
            var response = await _warehouseService.SearchWarehouses(
                pageIndex, pageSize, keyword, minArea, maxArea);

            return ControllerResponse.Response(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateWarehouseDTO request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }
          
            var result = await _warehouseService.Add<WarehouseDTO, CreateWarehouseDTO>(request);

            return ControllerResponse.Response(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateWarehouseDTO request)
        {
            request.Id = id;
            var result = await _warehouseService.Update<WarehouseDTO, UpdateWarehouseDTO>(request);
            return ControllerResponse.Response(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var result = await _warehouseService.Delete(id);

            return ControllerResponse.Response(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFullWarehouseInfo([FromRoute] string id)
        {
            var result = await _warehouseService.GetFullWarehouseInfo<WarehouseFullInfoDTO>(id);

            return ControllerResponse.Response(result);
        }

        //[Authorize]
        [HttpPost("{id}/location")]
        public async Task<IActionResult> AddWarehouseLocation([FromRoute] string id, [FromBody] CreateWarehouseStructureRequest request)
        {
            //var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

            //if (authUser != null)
            //{
            //    request.CreatedBy = authUser.id;
            //}
            //else
            //{
            //    return Unauthorized();
            //}
            request.Id = id;
            var result = await _warehouseService.CreateStructureAsync(request);

            return ControllerResponse.Response(result);
        }
        //[Authorize]
        [HttpGet("{id}/inventory")]
        public async Task<IActionResult> GetWarehouseInventory([FromRoute] string id)
        {
            var result = await _warehouseService.GetInventoryByWarehouseId(id);

            return ControllerResponse.Response(result);
        }

        ////[Authorize]
        //[HttpPost("{id}/inventory-location")]
        //public async Task<IActionResult> PutAwayInventory([FromRoute] string id, [FromBody] CreateInventoryLocationDTO request)
        //{
        //    var result = await _warehouseService.InventoryLocationInOutAsync(request);
        //    return ControllerResponse.Response(result);
        //}
        //[HttpGet("locations/{locationId}")]
        //public async Task<IActionResult> GetInventoriesInLocation([FromRoute] string locationId)
        //{
        //    var result = await _warehouseService.GetInventoriesInLocation(locationId);
        //    return ControllerResponse.Response(result);
        //}
        [HttpGet("{id}/location-logs")]
        public async Task<IActionResult> GetLocationLogs(
            [FromRoute] string id,
            [FromQuery] string? locationId = null,  // locationId là tùy chọn
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            // Gọi service để lấy log
            var result = await _warehouseService.GetLocationLogsAsync(id, locationId, pageIndex, pageSize);

            return ControllerResponse.Response(result);
        }
        /// <summary>
        /// Lấy dữ liệu thẻ kho theo sản phẩm và kho
        /// </summary>
        /// <param name="productId">ID sản phẩm</param>
        /// <param name="warehouseId">ID kho</param>
        /// <returns>Thông tin thẻ kho</returns>
        [HttpGet("stock-card")]
        public async Task<IActionResult> GetStockCard([FromQuery] string productId, [FromQuery] string warehouseId, 
                                                      [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var response = await _warehouseService.GetStockCard(productId, warehouseId, from, to);
            return ControllerResponse.Response(response);
        }

        [Authorize]
        [HttpGet("stock-book")]
        public async Task<IActionResult> GetStockBook([FromQuery] string warehouseId, [FromQuery] int month, [FromQuery] int year)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            if (authUser == null)
            {
                return Unauthorized();
            }
            var response = await _warehouseService.GetStockBookAsync(warehouseId, month, year, authUser.id);
            return ControllerResponse.Response(response);
        }
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetAvailableProductsInWarehouse(string id)
        {
            var result = await _warehouseService.GetAvailableProductsInWarehouse(id);
            return ControllerResponse.Response(result);
        }
    }
}