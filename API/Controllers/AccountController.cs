using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Service;
using System;
using BusinessLogicLayer.Models.Profile;
using BusinessLogicLayer.Models.AccountGroup;
using BusinessLogicLayer.Models.AccountAction;
using Data.Model.DTO;
using Data.Model.Response;
using API.Utils;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly BusinessLogicLayer.IServices.IAccountService _accountService;

        public AccountController(BusinessLogicLayer.IServices.IAccountService appUserService)
        {
            _accountService = appUserService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _accountService.Get<AccountDTO>(id);

            return ControllerResponse.Response(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AccountCreateDTO request)
        {
            var result = await _accountService.Add(request);

            return ControllerResponse.Response(result);
        }

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
            var result = await _accountService.Get<AccountDTO>(currentUser.id);

            return ControllerResponse.Response(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Add([FromRoute] string id, [FromBody] AccountUpdateDTO request)
        {
            var result = await _accountService.Update<AccountDTO, AccountUpdateDTO>(request);

            return ControllerResponse.Response(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var result = await _accountService.Delete(id);

            return ControllerResponse.Response(result);
        }
    }
}
