using API.Middlewares;
using API.Utils;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Utils;
using Data.Enum;
using Data.Model.DTO;
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
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho,Nhân viên bán hàng,Nhân viên kho")]
        [HttpGet]
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
        [AuthorizeGroup("Admin,Thủ kho")]
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
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateWarehouseDTO request)
        {
            request.Id = id;
            var result = await _warehouseService.Update<WarehouseDTO, UpdateWarehouseDTO>(request);
            return ControllerResponse.Response(result);
        }
        [Authorize]
        [AuthorizeGroup("Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var result = await _warehouseService.Delete(id);

            return ControllerResponse.Response(result);
        }
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho,Nhân viên bán hàng,Nhân viên kho")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFullWarehouseInfo([FromRoute] string id)
        {
            var result = await _warehouseService.GetFullWarehouseInfo<WarehouseFullInfoDTO>(id);

            return ControllerResponse.Response(result);
        }

        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho,Nhân viên bán hàng,Nhân viên kho")]
        [HttpGet("{id}/inventory")]
        public async Task<IActionResult> GetWarehouseInventory([FromRoute] string id)
        {
            var result = await _warehouseService.GetInventoryByWarehouseId(id);

            return ControllerResponse.Response(result);
        }
       
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpGet("stock-card")]
        public async Task<IActionResult> GetStockCard([FromQuery] string productId, [FromQuery] string warehouseId, 
                                                      [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var response = await _warehouseService.GetStockCard(productId, warehouseId, from, to);
            return ControllerResponse.Response(response);
        }

        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
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
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho,Nhân viên bán hàng")]
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetAvailableProductsInWarehouse(string id)
        {
            var result = await _warehouseService.GetAvailableProductsInWarehouse(id);
            return ControllerResponse.Response(result);
        }
        //Get all staff of warehouse
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpGet("{id}/staffs")]
        public async Task<IActionResult> GetStaffsInWarehouse(string id, [FromQuery] int pageIndex = 1,
                                                            [FromQuery] int pageSize = 5,
                                                            [FromQuery] string? keyword = null)
        {
            var result = await _warehouseService.GetStaffsOfWarehouse(id, pageIndex, pageSize, keyword);
            return ControllerResponse.Response(result);
        }
    }
}