using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.AccountWarehouse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/account-warehouse")]
    [ApiController]
    public class AccountWarehouseController : ControllerBase
    {
        private readonly IAccountWarehouseService _accountWarehouseService;

        public AccountWarehouseController(IAccountWarehouseService accountWarehouseService)
        {
            _accountWarehouseService = accountWarehouseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var data = await _accountWarehouseService.GetAllAsync();
                return Ok(new { StatusCode = 200, Message = "Lấy danh sách thành công", Data = data, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpGet("{accountId}/{warehouseId}")]
        public async Task<IActionResult> GetByIdAsync(string accountId, string warehouseId)
        {
            try
            {
                var data = await _accountWarehouseService.GetByIdAsync(accountId, warehouseId);
                if (data == null)
                    return NotFound(new { StatusCode = 404, Message = "Không tìm thấy dữ liệu", IsSuccess = false });

                return Ok(new { StatusCode = 200, Message = "Lấy dữ liệu thành công", Data = data, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateAccountWarehouseDTO model)
        {
            try
            {
                var data = await _accountWarehouseService.CreateAsync(model);
                return Ok(new { StatusCode = 201, Message = "Tạo thành công", Data = data, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteAccountWarehouseDTO model)
        {
            try
            {
                var deleted = await _accountWarehouseService.DeleteAsync(model);
                if (!deleted)
                    return NotFound(new { StatusCode = 404, Message = "Không tìm thấy dữ liệu", IsSuccess = false });

                return Ok(new { StatusCode = 200, Message = "Xóa thành công", IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpGet("warehouses/{accountId}")]
        public async Task<IActionResult> GetWarehousesByAccountIdAsync(string accountId)
        {
            try
            {
                var data = await _accountWarehouseService.GetWarehousesByAccountIdAsync(accountId);
                return Ok(new { StatusCode = 200, Message = "Lấy danh sách kho thành công", Data = data, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpGet("accounts/{warehouseId}")]
        public async Task<IActionResult> GetAccountsByWarehouseIdAsync(string warehouseId)
        {
            try
            {
                var data = await _accountWarehouseService.GetAccountsByWarehouseIdAsync(warehouseId);
                return Ok(new { StatusCode = 200, Message = "Lấy danh sách tài khoản thành công", Data = data, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpPut("{accountId}/{warehouseId}")]
        public async Task<IActionResult> UpdateAsync(string accountId, string warehouseId, [FromBody] UpdateAccountWarehouseDTO model)
        {
            try
            {
                var data = await _accountWarehouseService.UpdateAsync(accountId, warehouseId, model);
                return Ok(new { StatusCode = 200, Message = "Cập nhật thành công", Data = data, IsSuccess = true });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { StatusCode = 404, Message = ex.Message, IsSuccess = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message, IsSuccess = false });
            }
        }

    }
}
