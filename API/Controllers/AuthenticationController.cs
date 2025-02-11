using API.Payloads.Responses;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthenticationController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login", Name = "LoginAsync")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest reqObj)
        {
            try
            {
                var userDto = await _accountService.CheckLoginAsync(reqObj.UserName, reqObj.Password);

                if (userDto == null)
                {
                    return Unauthorized(new BaseResponse
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = "Sai tài khoản hoặc mật khẩu",
                        Data = null,
                        IsSuccess = false
                    });
                }

                var token = await _accountService.GenerateAccessTokenAsync(userDto.Id);


                return Ok(new BaseResponse
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Đăng nhập thành công",
                    Data = new
                    {
                        token,
                        userDto.Id,
                        userDto.AccountWarehouses,
                        userDto.AccountGroups,
                        userDto.AccountPermissions
                    },
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    Data = null,
                    IsSuccess = false
                });
            }
        }
        ////[Authorize(Roles = UserRoles.BrandManager + "," + UserRoles.Admin+","+UserRoles.Store)]
        //[HttpPut(APIRoutes.Account.ChangePassword, Name = "ChangePasswordAsync")]
        //public async Task<IActionResult> ChangePasswordAsync(int id, [FromBody] ChangePasswordRequest reqObj)
        //{
        //    try
        //    {
        //        if (!reqObj.NewPassword.Equals(reqObj.Confirm))
        //        {
        //            return BadRequest(new BaseResponse
        //            {
        //                StatusCode = StatusCodes.Status400BadRequest,
        //                Message = "mật khẩu mới chưa được xác thực chính xác",
        //                Data = null,
        //                IsSuccess = false
        //            });
        //        }
        //        var checkPass = await _accountService.checkCorrectPassword(id, reqObj.OldPassword);
        //        if (checkPass == false)
        //        {
        //            return BadRequest(new BaseResponse
        //            {
        //                StatusCode = StatusCodes.Status400BadRequest,
        //                Message = "Mật khẩu cũ của bạn chưa chính xác",
        //                Data = null,
        //                IsSuccess = false
        //            });
        //        }

        //        var dto = new AppUserDTO();
        //        dto.Password = reqObj.NewPassword;
        //        dto.IsActive = true;
        //        var result = await _appUserService.Update(id, dto, null);
        //        if (!result)
        //        {
        //            return NotFound(new BaseResponse
        //            {
        //                StatusCode = StatusCodes.Status404NotFound,
        //                Message = "Không tìm thấy người dùng",
        //                Data = null,
        //                IsSuccess = false
        //            });
        //        }
        //        return Ok(new BaseResponse
        //        {
        //            StatusCode = StatusCodes.Status200OK,
        //            Message = "Đổi mật khẩu thành công",
        //            Data = result,
        //            IsSuccess = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BaseResponse
        //        {
        //            StatusCode = StatusCodes.Status400BadRequest,
        //            Message = ex.Message,
        //            Data = null,
        //            IsSuccess = false
        //        });
        //    }
        //}
    }
}

