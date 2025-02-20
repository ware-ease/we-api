using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.GroupPermission;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/group-permission")]
    [ApiController]
    public class GroupPermissionController : ControllerBase
    {
        private readonly IGroupPermissionService _groupPermissionService;

        public GroupPermissionController(IGroupPermissionService groupPermissionService)
        {
            _groupPermissionService = groupPermissionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var data = await _groupPermissionService.GetAllAsync();
                return Ok(new { StatusCode = 200, Message = "Lấy danh sách thành công", Data = data, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpGet("{groupId}/{permissionId}")]
        public async Task<IActionResult> GetByIdAsync(string groupId, string permissionId)
        {
            try
            {
                var data = await _groupPermissionService.GetByIdAsync(groupId, permissionId);
                if (data == null)
                    return NotFound(new { StatusCode = 404, Message = "Không tìm thấy dữ liệu", IsSuccess = false });

                return Ok(new { StatusCode = 200, Message = "Lấy dữ liệu thành công", Data = data, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateGroupPermissionDTO model)
        {
            try
            {
                var data = await _groupPermissionService.CreateAsync(model);
                return Ok(new { StatusCode = 201, Message = "Tạo thành công", Data = data, IsSuccess = true });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message, IsSuccess = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpDelete("{groupId}/{permissionId}")]
        public async Task<IActionResult> DeleteAsync(string groupId, string permissionId)
        {
            try
            {
                var deleted = await _groupPermissionService.DeleteAsync(groupId, permissionId);
                if (!deleted)
                    return NotFound(new { StatusCode = 404, Message = "Không tìm thấy dữ liệu", IsSuccess = false });

                return Ok(new { StatusCode = 200, Message = "Xóa thành công", IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpGet("permissions/{groupId}")]
        public async Task<IActionResult> GetPermissionsByGroupIdAsync(string groupId)
        {
            try
            {
                var data = await _groupPermissionService.GetPermissionsByGroupIdAsync(groupId);
                return Ok(new { StatusCode = 200, Message = "Lấy danh sách quyền thành công", Data = data, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpGet("groups/{permissionId}")]
        public async Task<IActionResult> GetGroupsByPermissionIdAsync(string permissionId)
        {
            try
            {
                var data = await _groupPermissionService.GetGroupsByPermissionIdAsync(permissionId);
                return Ok(new { StatusCode = 200, Message = "Lấy danh sách nhóm thành công", Data = data, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }
    }
}
