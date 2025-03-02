using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.AppAction;
using BusinessLogicLayer.Models.Group;
using BusinessLogicLayer.Service;
using Data.Entity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/app-action")]
    public class AppActionController : ControllerBase
    {
        private readonly IAppActionService _appActionService;
        private readonly IJwtService _jwtService;

        public AppActionController(IAppActionService appActionService, IJwtService jwtService)
        {
            _appActionService = appActionService;
            _jwtService = jwtService;
        }
        private string? GetUserIdFromToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            return _jwtService.ValidateToken(token);
        }

        //[HttpGet("app-actions")]
        //public async Task<IActionResult> GetAllAsync()
        //{
        //    try
        //    {
        //        var actions = await _appActionService.GetAllAsync(null, null);
        //        return Ok(new
        //        {
        //            StatusCode = StatusCodes.Status200OK,
        //            Message = "Lấy dữ liệu thành công",
        //            Data = actions,
        //            IsSuccess = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            StatusCode = StatusCodes.Status400BadRequest,
        //            Message = ex.Message,
        //            IsSuccess = false
        //        });
        //    }
        //}

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
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Không tìm thấy UserId trong token", IsSuccess = false });

                action.CreatedBy = userId; // Gán UserId từ token vào DTO

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
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateAppActionDTO action)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Không tìm thấy UserId trong token", IsSuccess = false });

                action.LastUpdatedBy = userId; // Gán UserId từ token vào DTO

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
        public async Task<IActionResult> DeleteAsync(string id, [FromBody] DeleteAppActionDTO action)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Không tìm thấy UserId trong token", IsSuccess = false });

                action.DeletedBy = userId; // Gán UserId từ token vào DTO

                var isDeleted = await _appActionService.DeleteAsync(id, action.DeletedBy!);
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

        [HttpGet()]
        public async Task<IActionResult> SearchAsync([FromQuery] string? searchKey, [FromQuery] int? pageIndex, [FromQuery] int? pageSize)
        {
            try
            {
                var actions = await _appActionService.SearchAsync(searchKey, pageIndex, pageSize);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Tìm kiếm thành công",
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

    }
}