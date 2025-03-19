using API.Utils;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Utils;
using Data.Model.DTO;
using Data.Model.Request.Area;
using Data.Model.Request.Customer;
using Data.Model.Request.Warehouse;
using Data.Model.Response;
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

        //[HttpGet()]
        //public async Task<IActionResult> SearchAsync(
        //    [FromQuery] string? searchKey,
        //    [FromQuery] int? pageIndex,
        //    [FromQuery] int? pageSize)
        //{
        //    try
        //    {
        //        var result = await _warehouseService.SearchAsync(searchKey, pageIndex, pageSize);

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = "Tìm kiếm thành công",
        //            Data = result,
        //            IsSuccess = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = $"Lỗi server: {ex.Message}",
        //            IsSuccess = false
        //        });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> CreateAsync([FromBody] CreateWarehouseDTO model)
        //{
        //    try
        //    {
        //        var warehouse = await _warehouseService.CreateAsync(model);

        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = $"Lỗi server: {ex.Message}",
        //            IsSuccess = false
        //        });
        //    }
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetByIdAsync(string id)
        //{
        //    try
        //    {
        //        var warehouse = await _warehouseService.GetByIdAsync(id);

        //        if (warehouse == null)
        //        {
        //            return NotFound(new
        //            {
        //                StatusCode = 404,
        //                Message = "Không tìm thấy kho",
        //                IsSuccess = false
        //            });
        //        }

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = "Lấy dữ liệu thành công",
        //            Data = warehouse,
        //            IsSuccess = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = $"Lỗi server: {ex.Message}",
        //            IsSuccess = false
        //        });
        //    }
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateWarehouseDTO model)
        //{
        //    try
        //    {
        //        var updatedWarehouse = await _warehouseService.UpdateAsync(id, model);

        //        if (updatedWarehouse == null)
        //        {
        //            return NotFound(new
        //            {
        //                StatusCode = 404,
        //                Message = "Không tìm thấy kho",
        //                IsSuccess = false
        //            });
        //        }

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = "Cập nhật kho thành công",
        //            Data = updatedWarehouse,
        //            IsSuccess = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = $"Lỗi server: {ex.Message}",
        //            IsSuccess = false
        //        });
        //    }
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteAsync(string id)
        //{
        //    try
        //    {
        //        var deleted = await _warehouseService.DeleteAsync(id);

        //        if (!deleted)
        //        {
        //            return NotFound(new
        //            {
        //                StatusCode = 404,
        //                Message = "Không tìm thấy kho",
        //                IsSuccess = false
        //            });
        //        }

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = "Xóa kho thành công",
        //            IsSuccess = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = $"Lỗi server: {ex.Message}",
        //            IsSuccess = false
        //        });
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _warehouseService.Get<WarehouseDTO>();

            return ControllerResponse.Response(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchWarehouses(
                                                            [FromQuery] int pageIndex = 1,
                                                            [FromQuery] int pageSize = 5,
                                                            [FromQuery] string? keyword = null,
                                                            [FromQuery] float? minArea = null,
                                                            [FromQuery] float? maxArea = null)
        {
            var response = await _warehouseService.SearchWarehouses<WarehouseDTO>(
                pageIndex, pageSize, keyword, minArea, maxArea);

            return ControllerResponse.Response(response);
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateWarehouseDTO request)
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
    }
}