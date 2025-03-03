using API.Payloads.Responses;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers
{
    [Route("api/auth")]
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
                var userDto = await _accountService.CheckLoginAsync(reqObj.Username, reqObj.Password);

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

                // 🌟 Lưu token vào cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,  // Ngăn JavaScript truy cập (bảo mật hơn)
                    Secure = true,    // Chỉ gửi khi dùng HTTPS
                    SameSite = SameSiteMode.Strict, // Chặn gửi cookie từ site khác
                    Expires = DateTime.UtcNow.AddHours(1) // Thời gian hết hạn 1 giờ
                };

                Response.Cookies.Append("accessToken", token.AccessToken!, cookieOptions); // Thêm cookie vào response

                return Ok(new 
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Đăng nhập thành công",
                    //Data = new
                    //{
                    //    token,
                    //    userDto.Id,
                    //    //WarehouseIds = userDto.WarehouseIds
                    //},
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

