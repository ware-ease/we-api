using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.General;
using BusinessLogicLayer.Models.PurchaseReceipt;
using BusinessLogicLayer.Models.Supplier;
using BusinessLogicLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseReceiptController : ControllerBase
    {
        private readonly IPurchaseReceiptService _purchaseReceiptService;
        private readonly IMapper _mapper;

        public PurchaseReceiptController(IPurchaseReceiptService purchaseReceiptService, IMapper mapper)
        {
            _purchaseReceiptService = purchaseReceiptService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _purchaseReceiptService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {

            try
            {
                var purchaseReiceipt = await _purchaseReceiptService.GetByIdAsync(id);
                return Ok(new { StatusCode = StatusCodes.Status200OK, Message = "Thành công", Data = purchaseReiceipt });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([Required] string supplierId, [FromBody] CreatePurchaseReceiptDTO createPurchaseReceiptDTO)
        {

            try
            {


                var purchaseReceipt = await _purchaseReceiptService.AddAsync(supplierId, createPurchaseReceiptDTO);
                var response = new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Tạo PurchaseReceipt thành công",
                    Data = purchaseReceipt
                };
                return CreatedAtAction(nameof(GetById), new { id = purchaseReceipt.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                });
            }
        }

        [HttpPut("id")]
        public async Task<IActionResult> Update(string purchaseReceiptId, [FromBody] UpdatePurchaseReceiptDTO updatePurchaseReceiptDTO)
        {
            try
            {
                var updatePurchaseReceipt = await _purchaseReceiptService.UpdateAsync(purchaseReceiptId, updatePurchaseReceiptDTO);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Cập nhật Supplier thành công",
                    Data = updatePurchaseReceipt
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                });
            }
        }

        [HttpPut("Delete")]
        public async Task<IActionResult> Delete(string id, [FromBody] DeleteDTO deletePurchaseReceiptDTO)
        {
            try
            {
                await _purchaseReceiptService.DeleteAsync(id, deletePurchaseReceiptDTO.DeletedBy);

                return Ok(new { StatusCode = StatusCodes.Status200OK, Message = "Xóa PurchaseReceipt thành công", Data = (object)null });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                });
            }
        }
    }
}
