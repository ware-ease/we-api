using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.AppAction;
using Data.Entity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/app-action")]
    public class AppActionController : ControllerBase
    {
        private readonly IAppActionService _appActionService;

        public AppActionController(IAppActionService appActionService)
        {
            _appActionService = appActionService;
        }

        [HttpGet("app-actions")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var actions = await _appActionService.GetAllAsync();
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy dữ liệu thành công",
                    Data = actions,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                var action = await _appActionService.GetByIdAsync(id);
                if (action == null)
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Hành động không tồn tại",
                        IsSuccess = false
                    });

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy dữ liệu thành công",
                    Data = action,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateAppActionDTO action)
        {
            try
            {
                var createdAction = await _appActionService.CreateAsync(action);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Tạo hành động thành công",
                    Data = createdAction,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] CreateAppActionDTO action)
        {
            try
            {
                var updatedAction = await _appActionService.UpdateAsync(id, action);
                if (updatedAction == null)
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Hành động không tồn tại",
                        IsSuccess = false
                    });

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Cập nhật hành động thành công",
                    Data = updatedAction,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                var isDeleted = await _appActionService.DeleteAsync(id);
                if (!isDeleted)
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Hành động không tồn tại",
                        IsSuccess = false
                    });

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Xóa hành động thành công",
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }
    }
}