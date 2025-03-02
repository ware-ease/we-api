//using BusinessLogicLayer.IService;
//using BusinessLogicLayer.Models.AccountPermission;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace API.Controllers
//{
//    [Route("api/account-permission")]
//    [ApiController]
//    public class AccountPermissionController : ControllerBase
//    {
//        private readonly IAccountPermissionService _accountPermissionService;

//        public AccountPermissionController(IAccountPermissionService accountPermissionService)
//        {
//            _accountPermissionService = accountPermissionService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAllAsync()
//        {
//            try
//            {
//                var data = await _accountPermissionService.GetAllAsync();
//                return Ok(new { StatusCode = 200, Message = "Lấy danh sách thành công", Data = data, IsSuccess = true });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
//            }
//        }

//        [HttpGet("{accountId}/{permissionId}")]
//        public async Task<IActionResult> GetByIdAsync(string accountId, string permissionId)
//        {
//            try
//            {
//                var data = await _accountPermissionService.GetByIdAsync(accountId, permissionId);
//                if (data == null)
//                    return NotFound(new { StatusCode = 404, Message = "Không tìm thấy dữ liệu", IsSuccess = false });

//                return Ok(new { StatusCode = 200, Message = "Lấy dữ liệu thành công", Data = data, IsSuccess = true });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> CreateAsync([FromBody] CreateAccountPermissionDTO model)
//        {
//            try
//            {
//                var data = await _accountPermissionService.CreateAsync(model);
//                return Ok(new { StatusCode = 201, Message = "Tạo thành công", Data = data, IsSuccess = true });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
//            }
//        }

//        [HttpDelete("{accountId}/{permissionId}")]
//        public async Task<IActionResult> DeleteAsync(string accountId, string permissionId)
//        {
//            try
//            {
//                var deleted = await _accountPermissionService.DeleteAsync(accountId, permissionId);
//                if (!deleted)
//                    return NotFound(new { StatusCode = 404, Message = "Không tìm thấy dữ liệu", IsSuccess = false });

//                return Ok(new { StatusCode = 200, Message = "Xóa thành công", IsSuccess = true });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
//            }
//        }

//        [HttpGet("permissions/{accountId}")]
//        public async Task<IActionResult> GetPermissionsByAccountIdAsync(string accountId)
//        {
//            try
//            {
//                var data = await _accountPermissionService.GetPermissionsByAccountIdAsync(accountId);
//                return Ok(new { StatusCode = 200, Message = "Lấy danh sách quyền thành công", Data = data, IsSuccess = true });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
//            }
//        }

//        [HttpGet("accounts/{permissionId}")]
//        public async Task<IActionResult> GetAccountsByPermissionIdAsync(string permissionId)
//        {
//            try
//            {
//                var data = await _accountPermissionService.GetAccountsByPermissionIdAsync(permissionId);
//                return Ok(new { StatusCode = 200, Message = "Lấy danh sách tài khoản thành công", Data = data, IsSuccess = true });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
//            }
//        }
//    }
//}