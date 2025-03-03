using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Service;
using System;
using BusinessLogicLayer.Models.Profile;
using BusinessLogicLayer.Models.AccountGroup;
using BusinessLogicLayer.Models.AccountAction;

namespace API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IAccountService _accountService;
        private readonly IProfileService _profileService;
        private readonly IAccountGroupService _accountGroupService;
        private readonly IJwtService _jwtService;

        public AccountController(IAccountService appUserService, IJwtService jwtService, IProfileService profileService, IAccountGroupService accountGroupService)
        {
            _accountService = appUserService;
            _jwtService = jwtService;
            _profileService = profileService;
            _accountGroupService = accountGroupService;
        }

        private string? GetUserIdFromToken()
        {
            HttpContext.Request.Cookies.TryGetValue("AuthToken", out var token);
            return _jwtService.ValidateToken(token);
        }

        [HttpGet("{id}", Name = "GetAccountById")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(id);
                if (account == null)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Tài khoản không tồn tại",
                        IsSuccess = false
                    });
                }

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Tải dữ liệu thành công",
                    Data = account,
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

        [HttpPost()]
        public async Task<IActionResult> CreateAsync([FromBody] AccountCreateDTO model)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Không tìm thấy UserId trong token", IsSuccess = false });

                model.CreatedBy = userId; // Gán UserId từ token vào DTO

                var account = await _accountService.CreateAccountAsync(model);
                return Created("GetAccountById", new 
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Tạo tài khoản thành công",
                    Data = account,
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

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateAsync(string id, [FromBody] AccountUpdateDTO model)
        //{
        //    try
        //    {
        //        var updatedAccount = await _accountService.UpdateAccountAsync(id, model);
        //        if (updatedAccount == null)
        //        {
        //            return NotFound(new 
        //            {
        //                StatusCode = StatusCodes.Status404NotFound,
        //                Message = "Tài khoản không tồn tại",
        //                //Data = null,
        //                IsSuccess = false
        //            });
        //        }

        //        return Ok(new 
        //        {
        //            StatusCode = StatusCodes.Status200OK,
        //            Message = "Cập nhật tài khoản thành công",
        //            Data = updatedAccount,
        //            IsSuccess = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new 
        //        {
        //            StatusCode = StatusCodes.Status400BadRequest,
        //            Message = ex.Message,
        //            //Data = null,
        //            IsSuccess = false
        //        });
        //    }
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id, [FromBody] DeleteAccountDTO model)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Không tìm thấy UserId trong token", IsSuccess = false });
                model.DeletedBy = userId;

                var deleted = await _accountService.DeleteAccountAsync(id, model.DeletedBy!);
                if (!deleted)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Tài khoản không tồn tại",
                        IsSuccess = false
                    });
                }

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Xóa tài khoản thành công",
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


        //[HttpPut("{id}/username")]
        //public async Task<IActionResult> UpdateUsernameAsync(string id, [FromBody] UpdateUsernameDTO model)
        //{
        //    try
        //    {
        //        var updatedAccount = await _accountService.UpdateUsernameAsync(id, model.NewUsername, model.LastUpdatedBy!);
        //        if (updatedAccount == null)
        //        {
        //            return NotFound(new
        //            {
        //                StatusCode = StatusCodes.Status404NotFound,
        //                Message = "Tài khoản không tồn tại",
        //                IsSuccess = false
        //            });
        //        }

        //        return Ok(new
        //        {
        //            StatusCode = StatusCodes.Status200OK,
        //            Message = "Cập nhật username thành công",
        //            Data = updatedAccount,
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

        //[HttpPut("{id}/password")]
        //public async Task<IActionResult> UpdatePasswordAsync(string id, [FromBody] UpdatePasswordDTO model)
        //{
        //    try
        //    {
        //        var isUpdated = await _accountService.UpdatePasswordAsync(id, model.CurrentPassword, model.NewPassword, model.LastUpdatedBy!);
        //        if (isUpdated == null)
        //        {
        //            return BadRequest(new
        //            {
        //                StatusCode = StatusCodes.Status400BadRequest,
        //                Message = "Mật khẩu cũ không đúng hoặc có lỗi xảy ra",
        //                IsSuccess = false
        //            });
        //        }

        //        return Ok(new
        //        {
        //            StatusCode = StatusCodes.Status200OK,
        //            Message = "Cập nhật mật khẩu thành công",
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

        //[HttpGet("search")]
        [HttpGet()]
        public async Task<IActionResult> SearchAccounts([FromQuery] string? keyword,
                                                        [FromQuery] int pageIndex = 1,
                                                        [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _accountService.SearchAccountAsync(keyword, pageIndex, pageSize);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Tìm kiếm tài khoản thành công",
                    Data = result,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi khi tìm kiếm tài khoản",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfileByIdAsync()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Không tìm thấy UserId trong token", IsSuccess = false });

//                model.CreatedBy = userId; // Gán UserId từ token vào DTO
                var profile = await _profileService.GetProfileByUserIdAsync(userId);
                if (profile == null)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Profile không tồn tại",
                        IsSuccess = false
                    });
                }

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy dữ liệu thành công",
                    Data = profile,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi khi lấy profile",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpPatch("me")]
        public async Task<IActionResult> UpdateByAccountIdAsync([FromBody] ProfileUpdateDTO model)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Không tìm thấy UserId trong token", IsSuccess = false });

                var updatedProfile = await _profileService.UpdateProfileByAccountIdAsync(userId, model);
                if (updatedProfile == null)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Profile không tồn tại cho tài khoản này",
                        IsSuccess = false
                    });
                }

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Cập nhật profile thành công",
                    Data = updatedProfile,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi khi cập nhật profile",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpGet("me/groups")]
        public async Task<IActionResult> GetGroupsByAccountIdAsync()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Không tìm thấy UserId trong token", IsSuccess = false });
                var data = await _accountGroupService.GetGroupsByAccountIdAsync(userId);
                return Ok(new { StatusCode = 200, Message = "Lấy danh sách nhóm thành công", Data = data, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpPost("groups")]
        public async Task<IActionResult> CreateAccountGroupAsync([FromBody] CreateAccountGroupDTO model)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Không tìm thấy UserId trong token", IsSuccess = false });
                model.CreatedBy = userId;

                var data = await _accountService.CreateAsync(model);
                return Ok(new { StatusCode = 201, Message = "Tạo thành công", Data = data, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpDelete("account-group")]
        public async Task<IActionResult> DeleteMultipleAccountGroupAsync([FromBody] DeleteAccountGroupDTO model)
        {
            try
            {
                if (model.AccountIds == null || model.GroupIds == null || !model.AccountIds.Any() || !model.GroupIds.Any())
                {
                    return BadRequest(new { StatusCode = 400, Message = "Danh sách accountId hoặc groupId không hợp lệ", IsSuccess = false });
                }

                var deleted = await _accountService.DeleteMultipleAccountGroupAsync(model.AccountIds, model.GroupIds);
                if (!deleted)
                    return NotFound(new { StatusCode = 404, Message = "Không tìm thấy dữ liệu phù hợp để xóa", IsSuccess = false });

                return Ok(new { StatusCode = 200, Message = "Xóa thành công", IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpPost("actions")]
        public async Task<IActionResult> CreateAccountActionAsync([FromBody] CreateAccountActionDTO model)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Không tìm thấy UserId trong token", IsSuccess = false });
                model.CreatedBy = userId;

                var data = await _accountService.CreateAccountActionAsync(model);
                return Ok(new { StatusCode = 201, Message = "Tạo thành công", Data = data, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpDelete("account-action")]
        public async Task<IActionResult> DeleteMultipleAccountActionsAsync([FromBody] DeleteAccountActionDTO model)
        {
            try
            {
                if (model.AccountIds == null || model.ActionIds == null || !model.AccountIds.Any() || !model.ActionIds.Any())
                {
                    return BadRequest(new { StatusCode = 400, Message = "Danh sách accountId hoặc actionId không hợp lệ", IsSuccess = false });
                }

                var deleted = await _accountService.DeleteMultipleAccountActionsAsync(model.AccountIds, model.ActionIds);
                if (!deleted)
                    return NotFound(new { StatusCode = 404, Message = "Không tìm thấy dữ liệu phù hợp để xóa", IsSuccess = false });

                return Ok(new { StatusCode = 200, Message = "Xóa thành công", IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

    }
}
