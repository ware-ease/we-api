using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.PermissionAction;
using BusinessLogicLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/permission-action")]
    [ApiController]
    public class PermissionActionController : ControllerBase
    {
        private readonly IPermissionActionService _permissionActionService;

        public PermissionActionController(IPermissionActionService permissionActionService)
        {
            _permissionActionService = permissionActionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var data = await _permissionActionService.GetAllAsync();
                return Ok(new { StatusCode = 200, Message = "Lấy danh sách thành công", Data = data, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpGet("{permissionId}/{actionId}")]
        public async Task<IActionResult> GetByIdAsync(string permissionId, string actionId)
        {
            try
            {
                var data = await _permissionActionService.GetByIdAsync(permissionId, actionId);
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
        public async Task<IActionResult> CreateAsync([FromBody] CreatePermissionActionDTO model)
        {
            try
            {
                var data = await _permissionActionService.CreateAsync(model);
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

        [HttpDelete("{permissionId}/{actionId}")]
        public async Task<IActionResult> DeleteAsync(string permissionId, string actionId)
        {
            try
            {
                var deleted = await _permissionActionService.DeleteAsync(permissionId, actionId);
                if (!deleted)
                    return NotFound(new { StatusCode = 404, Message = "Không tìm thấy dữ liệu", IsSuccess = false });

                return Ok(new { StatusCode = 200, Message = "Xóa thành công", IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpGet("actions/{permissionId}")]
        public async Task<IActionResult> GetActionsByPermissionIdAsync(string permissionId)
        {
            try
            {
                var data = await _permissionActionService.GetActionsByPermissionIdAsync(permissionId);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Lấy danh sách hành động thành công",
                    Data = data,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpGet("permissions/{actionId}")]
        public async Task<IActionResult> GetPermissionsByActionIdAsync(string actionId)
        {
            try
            {
                var data = await _permissionActionService.GetPermissionsByActionIdAsync(actionId);
                return Ok(new { StatusCode = 200, Message = "Lấy danh sách quyền theo Action thành công", Data = data, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

    }
}
