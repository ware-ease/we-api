//using BusinessLogicLayer.IService;
//using BusinessLogicLayer.Models.AccountGroup;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace API.Controllers
//{
//    [Route("api/account-group")]
//    [ApiController]
//    public class AccountGroupController : ControllerBase
//    {
//        private readonly IAccountGroupService _accountGroupService;

//        public AccountGroupController(IAccountGroupService accountGroupService)
//        {
//            _accountGroupService = accountGroupService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAllAsync()
//        {
//            try
//            {
//                var data = await _accountGroupService.GetAllAsync();
//                return Ok(new { StatusCode = 200, Message = "Lấy danh sách thành công", Data = data, IsSuccess = true });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
//            }
//        }

//        [HttpGet("{accountId}/{groupId}")]
//        public async Task<IActionResult> GetByIdAsync(string accountId, string groupId)
//        {
//            try
//            {
//                var data = await _accountGroupService.GetByIdAsync(accountId, groupId);
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
//        public async Task<IActionResult> CreateAsync([FromBody] CreateAccountGroupDTO model)
//        {
//            try
//            {
//                var data = await _accountGroupService.CreateAsync(model);
//                return Ok(new { StatusCode = 201, Message = "Tạo thành công", Data = data, IsSuccess = true });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
//            }
//        }

//        [HttpDelete("{accountId}/{groupId}")]
//        public async Task<IActionResult> DeleteAsync(string accountId, string groupId)
//        {
//            try
//            {
//                var deleted = await _accountGroupService.DeleteAsync(accountId, groupId);
//                if (!deleted)
//                    return NotFound(new { StatusCode = 404, Message = "Không tìm thấy dữ liệu", IsSuccess = false });

//                return Ok(new { StatusCode = 200, Message = "Xóa thành công", IsSuccess = true });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
//            }
//        }

//        [HttpGet("groups/{accountId}")]
//        public async Task<IActionResult> GetGroupsByAccountIdAsync(string accountId)
//        {
//            try
//            {
//                var data = await _accountGroupService.GetGroupsByAccountIdAsync(accountId);
//                return Ok(new { StatusCode = 200, Message = "Lấy danh sách nhóm thành công", Data = data, IsSuccess = true });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
//            }
//        }

//        [HttpGet("accounts/{groupId}")]
//        public async Task<IActionResult> GetAccountsByGroupIdAsync(string groupId)
//        {
//            try
//            {
//                var data = await _accountGroupService.GetAccountsByGroupIdAsync(groupId);
//                return Ok(new { StatusCode = 200, Message = "Lấy danh sách tài khoản thành công", Data = data, IsSuccess = true });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
//            }
//        }
//    }
//}