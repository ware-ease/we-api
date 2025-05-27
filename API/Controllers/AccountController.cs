using Microsoft.AspNetCore.Mvc;
using Data.Model.DTO;
using Data.Model.Response;
using API.Utils;
using Microsoft.AspNetCore.Authorization;
using Data.Model.Request.Account;
using BusinessLogicLayer.IServices;
using Data.Enum;

namespace API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IAccountService _accountService;

        public AccountController(IAccountService appUserService)
        {
            _accountService = appUserService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _accountService.Get<AccountDTO>(id);

            return ControllerResponse.Response(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AccountCreateDTO request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }

            var result = await _accountService.Add(request);

            return ControllerResponse.Response(result);
        }

        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            var result = await _accountService.Get<AccountDTO>();

            return ControllerResponse.Response(result);
        }

        [Authorize]
        
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var currentUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            var result = await _accountService.Get<AccountDTO>(currentUser!.id!);

            return ControllerResponse.Response(result);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] AccountUpdateDTO request)
        {
            var result = await _accountService.Update(id, request);

            return ControllerResponse.Response(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var result = await _accountService.Delete(id);

            return ControllerResponse.Response(result);
        }

        [Authorize]
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] AccountPasswordUpdateDTO request)
        {
            AuthHelper.AuthUser? authUser = AuthHelper.GetCurrentUser(Request);

            if (authUser != null && authUser.id != null)
            {
                var result = await _accountService.ChangePassword(authUser.id, request.OldPassword!, request.Password!);

                AuthHelper.DeleteFromCookies(Response, "accessToken");
                AuthHelper.DeleteFromCookies(Response, "refreshToken");

                return ControllerResponse.Response(result);
            }

            return BadRequest();
        }

        [Authorize]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromQuery] AccountStatus status)
        {
            var response = await _accountService.UpdateStatus(id, status);
            return ControllerResponse.Response(response);
        }
        //get all task of staff
        [Authorize]
        [HttpGet("tasks")]
        public async Task<IActionResult> GetTasks([FromQuery] string? warehouseId, [FromQuery] InventoryCountDetailStatus? status,[FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

            var result = await _accountService.GetTasks(authUser!.id!, warehouseId, status, pageIndex, pageSize);
            return ControllerResponse.Response(result);
        }

    }
}
