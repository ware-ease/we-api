using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.IService;
using API.Common.Constants;
using API.Payloads.Responses;
using API.Payloads;

namespace API.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IAccountService _accountService;

        public AccountController(IAccountService appUserService)
        {
            _accountService = appUserService;
        }

        //[Authorize(Roles = UserRoles.Admin)]
        [HttpGet(APIRoutes.Account.GetAll, Name = "GetUsersAsync")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var allAccount = await _accountService.GetAccountsAsync();

                return Ok(new BaseResponse
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Tải dữ liệu thành công",
                    Data = allAccount,
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
    }
}
