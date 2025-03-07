using API.Utils;
using BusinessLogicLayer.IServices;
using Data.Model.DTO;
using Data.Model.Request.Auth;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            var result = await _authService.Login(request.Username, request.Password);

            if (result.Status == Data.Enum.SRStatus.Unauthorized || result.Data == null)
            {
                return ControllerResponse.Response(result);
            }

            var token = await _authService.GenerateTokens(result.Data.ToString()!);

            AuthHelper.AppendToCookies(Response, "accessToken", token.accessToken!);
            AuthHelper.AppendToCookies(Response, "refreshToken", token.refreshToken!);

            return ControllerResponse.Response(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            AuthHelper.AuthUser? authUser = AuthHelper.GetCurrentUser(Request);

            TokenDTO tokenDto = new TokenDTO
            {
                accessToken = authUser?.accessToken,
                refreshToken = authUser?.refreshToken,
            };

            var result = await _authService.Logout(tokenDto);

            AuthHelper.DeleteFromCookies(Response, "accessToken");
            AuthHelper.DeleteFromCookies(Response, "refreshToken");

            return ControllerResponse.Response(result);
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            AuthHelper.AuthUser? authUser = AuthHelper.GetCurrentUser(Request);
            AuthHelper.DeleteFromCookies(Response, "accessToken");
            AuthHelper.DeleteFromCookies(Response, "refreshToken");

            var result = await _authService.CheckRefreshTokenExpiry(authUser?.refreshToken);
            if (result == true)
            {
                var token = await _authService.GenerateTokens(authUser?.id!);

                AuthHelper.AppendToCookies(Response, "accessToken", token.accessToken!);
                AuthHelper.AppendToCookies(Response, "refreshToken", token.refreshToken!);
                
                return Ok();
            }

            return BadRequest();
        }
    }
}

