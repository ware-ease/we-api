using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Permission;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/permission")]
    [ApiController]
    public class PermissionController : ControllerBase
    {

        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var permissions = await _permissionService.GetAllAsync(pageIndex, pageSize);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Lấy danh sách thành công",
                    Data = permissions,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Lỗi khi lấy danh sách quyền",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpGet("{id}", Name ="GetPermissionById")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                var permission = await _permissionService.GetByIdAsync(id);
                if (permission == null)
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy quyền",
                        IsSuccess = false
                    });

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Lấy dữ liệu thành công",
                    Data = permission,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Lỗi khi lấy quyền theo ID",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreatePermissionDTO model)
        {
            try
            {
                var permission = await _permissionService.CreateAsync(model);
                return Created("GetPermissionById", new
                {
                    StatusCode = 201,
                    Message = "Tạo quyền thành công",
                    Data = permission,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Lỗi khi tạo quyền",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdatePermissionDTO model)
        {
            try
            {
                var updatedPermission = await _permissionService.UpdateAsync(id, model);
                if (updatedPermission == null)
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy quyền",
                        IsSuccess = false
                    });

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Cập nhật quyền thành công",
                    Data = updatedPermission,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Lỗi khi cập nhật quyền",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id, [FromBody] DeletePermissionDTO model)
        {
            try
            {
                var deleted = await _permissionService.DeleteAsync(id, model.DeletedBy!);
                if (!deleted)
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy quyền",
                        IsSuccess = false
                    });

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Xóa quyền thành công",
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Lỗi khi xóa quyền",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync([FromQuery] string? searchKey, [FromQuery] int? pageIndex, [FromQuery] int? pageSize)
        {
            try
            {
                var permissions = await _permissionService.SearchAsync(searchKey, pageIndex, pageSize);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Tìm kiếm thành công",
                    Data = permissions,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Lỗi khi tìm kiếm quyền",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }
    }
}