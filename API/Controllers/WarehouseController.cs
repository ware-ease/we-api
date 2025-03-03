using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Warehouse;
using Microsoft.AspNetCore.Http;
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

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int pageIndex = 1,
                                                     [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _warehouseService.GetAllAsync(pageIndex, pageSize);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy danh sách thành công",
                    Data = result,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi server: {ex.Message}",
                    IsSuccess = false
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                var warehouse = await _warehouseService.GetByIdAsync(id);

                if (warehouse == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy kho",
                        IsSuccess = false
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Lấy dữ liệu thành công",
                    Data = warehouse,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = $"Lỗi server: {ex.Message}",
                    IsSuccess = false
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateWarehouseDTO model)
        {
            try
            {
                var warehouse = await _warehouseService.CreateAsync(model);

                return Created("",
                    new
                    {
                        StatusCode = 201,
                        Message = "Tạo kho thành công",
                        Data = warehouse,
                        IsSuccess = true
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = $"Lỗi server: {ex.Message}",
                    IsSuccess = false
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateWarehouseDTO model)
        {
            try
            {
                var updatedWarehouse = await _warehouseService.UpdateAsync(id, model);

                if (updatedWarehouse == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy kho",
                        IsSuccess = false
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Cập nhật kho thành công",
                    Data = updatedWarehouse,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = $"Lỗi server: {ex.Message}",
                    IsSuccess = false
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id, [FromBody] DeleteWarehouseDTO model)
        {
            try
            {
                var deleted = await _warehouseService.DeleteAsync(id, model.DeletedBy!);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy kho",
                        IsSuccess = false
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Xóa kho thành công",
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = $"Lỗi server: {ex.Message}",
                    IsSuccess = false
                });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync(
            [FromQuery] string? searchKey,
            [FromQuery] int? pageIndex,
            [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _warehouseService.SearchAsync(searchKey, pageIndex, pageSize);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Tìm kiếm thành công",
                    Data = result,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = $"Lỗi server: {ex.Message}",
                    IsSuccess = false
                });
            }
        }
    }
}