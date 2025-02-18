using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.PurchaseDetail;
using BusinessLogicLayer.Models.PurchaseReceipt;
using BusinessLogicLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseDetailController : ControllerBase
    {
        private readonly IPurchaseDetailService _purchaseDetailService;
        private readonly IMapper _mapper;

        public PurchaseDetailController(IPurchaseDetailService purchaseDetailService, IMapper mapper)
        {
            _purchaseDetailService = purchaseDetailService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _purchaseDetailService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {

            try
            {
                var purchaseDetail = await _purchaseDetailService.GetByIdAsync(id);
                return Ok(new { StatusCode = StatusCodes.Status200OK, Message = "Thành công", Data = purchaseDetail });
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
        public async Task<IActionResult> Create([Required] string reiceiptId, [Required] string productTypeId, [FromBody] CreatePurchaseDetailDTO createPurchaseDetailDTO)
        {

            try
            {


                var purchaseDetail = await _purchaseDetailService.AddAsync(reiceiptId, productTypeId, createPurchaseDetailDTO);
                var response = new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Tạo PurchaseReceipt thành công",
                    Data = purchaseDetail
                };
                return CreatedAtAction(nameof(GetById), new { id = purchaseDetail.Id }, response);
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
        public async Task<IActionResult> Update(string purchaseDetailId, [FromBody] UpdatePurchaseDetailDTO updatePurchaseDetailDTO)
        {
            try
            {
                var updatePurchaseDetail = await _purchaseDetailService.UpdateAsync(purchaseDetailId, updatePurchaseDetailDTO);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Cập nhật Supplier thành công",
                    Data = updatePurchaseDetail
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
        public async Task<IActionResult> Delete(string id, [FromBody] DeletePurchaseDetailDTO deletePurchaseDetailDTO)
        {
            try
            {
                await _purchaseDetailService.DeleteAsync(id, deletePurchaseDetailDTO.DeletedBy);

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
