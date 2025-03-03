using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.IService;
using API.Payloads.Responses;
using API.Payloads;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.Models.Group;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("api/groups")]
    //[Authorize] // Yêu cầu xác thực token
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IJwtService _jwtService;

        public GroupController(IGroupService groupService, IJwtService jwtService)
        {
            _groupService = groupService;
            _jwtService = jwtService;
        }

        private string? GetUserIdFromToken()
        {
            HttpContext.Request.Cookies.TryGetValue("AuthToken", out var token);
            return _jwtService.ValidateToken(token);
        }


        //[HttpGet()]
        //public async Task<IActionResult> GetAllAsync()
        //{
        //    try
        //    {
        //        var groups = await _groupService.GetAllAsync(1,10);
        //        return Ok(new
        //        {
        //            StatusCode = StatusCodes.Status200OK,
        //            Message = "Tải dữ liệu thành công",
        //            Data = groups,
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
                var group = await _groupService.GetGroupByIdAsync(id);
                if (group == null)
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Nhóm không tồn tại",
                        IsSuccess = false
                    });

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy dữ liệu thành công",
                    Data = group,
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
        public async Task<IActionResult> CreateAsync([FromBody] CreateGroupDTO groupDto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Không tìm thấy UserId trong token", IsSuccess = false });

                groupDto.CreatedBy = userId; // Gán UserId từ token vào DTO

                var createdGroup = await _groupService.CreateGroupAsync(groupDto);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Tạo nhóm thành công",
                    Data = createdGroup,
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
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateGroupDTO groupDto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Không tìm thấy UserId trong token", IsSuccess = false });
                groupDto.LastUpdatedBy = userId;

                var updatedGroup = await _groupService.UpdateGroupAsync(id, groupDto);
                if (updatedGroup == null)
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Nhóm không tồn tại",
                        IsSuccess = false
                    });

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Cập nhật nhóm thành công",
                    Data = updatedGroup,
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
        public async Task<IActionResult> DeleteAsync(string id, [FromBody] DeleteGroupDTO groupDto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Không tìm thấy UserId trong token", IsSuccess = false });
                groupDto.DeletedBy = userId;
                var isDeleted = await _groupService.DeleteGroupAsync(id, groupDto.DeletedBy!);
                if (!isDeleted)
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Nhóm không tồn tại",
                        IsSuccess = false
                    });

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Xóa nhóm thành công",
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
                var groups = await _groupService.SearchAsync(searchKey, pageIndex, pageSize);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Tìm kiếm thành công",
                    Data = groups,
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