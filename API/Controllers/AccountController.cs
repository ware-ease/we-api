using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Account;

namespace API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IAccountService _accountService;

        public AccountController(IAccountService appUserService)
        {
            _accountService = appUserService;
        }

        [HttpGet("accounts", Name = "GetUsersAsync")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var allAccount = await _accountService.GetAllAccountsAsync(null, null);

                return Ok(new 
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Tải dữ liệu thành công",
                    Data = allAccount,
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
                        //Data = null,
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
                    //Data = null,
                    IsSuccess = false
                });
            }
        }

        [HttpPost()]
        public async Task<IActionResult> CreateAsync([FromBody] AccountCreateDTO model)
        {
            try
            {
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
                    //Data = null,
                    IsSuccess = false
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] AccountUpdateDTO model)
        {
            try
            {
                var updatedAccount = await _accountService.UpdateAccountAsync(id, model);
                if (updatedAccount == null)
                {
                    return NotFound(new 
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Tài khoản không tồn tại",
                        //Data = null,
                        IsSuccess = false
                    });
                }

                return Ok(new 
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Cập nhật tài khoản thành công",
                    Data = updatedAccount,
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id, [FromBody] DeleteAccountDTO model)
        {
            try
            {
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


        [HttpPut("{id}/username")]
        public async Task<IActionResult> UpdateUsernameAsync(string id, [FromBody] UpdateUsernameDTO model)
        {
            try
            {
                var updatedAccount = await _accountService.UpdateUsernameAsync(id, model.NewUsername, model.LastUpdatedBy!);
                if (updatedAccount == null)
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
                    Message = "Cập nhật username thành công",
                    Data = updatedAccount,
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

        [HttpPut("{id}/password")]
        public async Task<IActionResult> UpdatePasswordAsync(string id, [FromBody] UpdatePasswordDTO model)
        {
            try
            {
                var isUpdated = await _accountService.UpdatePasswordAsync(id, model.CurrentPassword, model.NewPassword, model.LastUpdatedBy!);
                if (isUpdated == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Mật khẩu cũ không đúng hoặc có lỗi xảy ra",
                        IsSuccess = false
                    });
                }

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Cập nhật mật khẩu thành công",
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

        [HttpGet("accounts/search")]
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
    }
}
