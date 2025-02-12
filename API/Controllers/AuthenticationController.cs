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
                    return Unauthorized(new 
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = "Sai tài khoản hoặc mật khẩu",
                        //Data = null,
                        IsSuccess = false
                    });
                }

                var token = await _accountService.GenerateAccessTokenAsync(userDto.Id);


                return Ok(new 
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Đăng nhập thành công",
                    Data = new
                    {
                        token,
                        userDto.Id,
                        GroupIds = userDto.GroupIds,          
                        PermissionIds = userDto.PermissionIds,
                        WarehouseIds = userDto.WarehouseIds
                    },
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    //Data = null,
                    IsSuccess = false
                });
            }
        }
    }
}

