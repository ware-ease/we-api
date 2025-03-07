using API.Utils;
using BusinessLogicLayer.IServices;
using Data.Model.DTO;
using Data.Model.Request.Area;
using Data.Model.Request.Customer;
using Data.Model.Request.Warehouse;
using Data.Model.Response;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _warehouseService.Get<WarehouseDTO>(id);

            return ControllerResponse.Response(result);
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
        public async Task<IActionResult> Update(string id, [FromBody] UpdateWarehouseDTO request)
        {
            request.Id = id;
            var result = await _warehouseService.Update<WarehouseDTO, UpdateWarehouseDTO>(request);

            return ControllerResponse.Response(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _warehouseService.Delete(id);

            return ControllerResponse.Response(result);
        }

        [HttpGet("{id}/full-info")]
        public async Task<IActionResult> GetWarehouseShelves(string id)
        {
            var result = await _warehouseService.GetFullWarehouseInfo<WarehouseFullInfoDTO>(id);

            return ControllerResponse.Response(result);
        }

        //[Authorize]
        [HttpPost("{id}/full-info")]
        public async Task<IActionResult> AddWarehouseInfo([FromRoute] string id, [FromBody] CreateWarehouseStructureRequest request)
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

        [HttpGet("{id}/areas")]
        public async Task<IActionResult> GetWarehouseAreas(string id)
        {
            var result = await _warehouseService.GetWarehouseAreas(id);
            return ControllerResponse.Response(result);
        }

        [HttpPost("{id}/areas")]
        public async Task<IActionResult> AddWarehouseArea([FromRoute] string id, [FromBody] CreateAREADto request)
        {
            request.WarehouseId = id;
            var result = await _warehouseService.AddWarehouseArea(request);
            return ControllerResponse.Response(result);
        }

        [HttpGet("areas/{id}/shelves")]
        public async Task<IActionResult> GetShelvesByArea(string id)
        {
            var result = await _warehouseService.GetShelvesByArea(id);
            return ControllerResponse.Response(result);
        }

        [HttpPost("areas/{id}/shelves")]
        public async Task<IActionResult> AddShelfWithStructure([FromRoute] string id, [FromBody] CreateShelfDto request)
        {
            var result = await _warehouseService.AddShelfWithStructure(request,id);
            return ControllerResponse.Response(result);
        }

        [HttpGet("shelves/{id}")]
        public async Task<IActionResult> GetShelfDetails(string id)
        {
            var result = await _warehouseService.GetShelfDetails(id);
            return ControllerResponse.Response(result);
        }

    }
}