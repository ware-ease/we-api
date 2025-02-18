using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.ProductTypeTypeDetail;
using BusinessLogicLayer.Models.ReceivingNote;
using BusinessLogicLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeTypeDetailController : ControllerBase
    {
        private readonly IProductTypeTypeDetailService _productTypeTypeDetailService;
        private readonly IMapper _mapper;

        public ProductTypeTypeDetailController(IProductTypeTypeDetailService productTypeTypeDetailService, IMapper mapper)
        {
            _productTypeTypeDetailService = productTypeTypeDetailService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _productTypeTypeDetailService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {

            try
            {
                var productTypeTypeDetail = await _productTypeTypeDetailService.GetByIdAsync(id);
                return Ok(new { Data = productTypeTypeDetail });
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
        public async Task<IActionResult> Create([Required] string typeDetailId, [Required] string productTypeId, 
            [FromBody] CreateProductTypeTypeDetailDTO createProductTypeTypeDetailDTO)
        {

            try
            {
                var productTypeTypeDetail = await _productTypeTypeDetailService.AddAsync(typeDetailId, productTypeId, 
                    createProductTypeTypeDetailDTO);
                var response = new
                {
                    Data = productTypeTypeDetail
                };
                return CreatedAtAction(nameof(GetById), new { id = productTypeTypeDetail.Id }, response);
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
        public async Task<IActionResult> Update(string productTypeTypeDetailId, 
            [FromBody] UpdateProductTypeTypeDetailDTO updateProductTypeTypeDetailDTO)
        {
            try
            {
                var productTypeTypeDetail = await _productTypeTypeDetailService.UpdateAsync(productTypeTypeDetailId, updateProductTypeTypeDetailDTO);
                return Ok(new
                {
                    Data = productTypeTypeDetail
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
        public async Task<IActionResult> Delete(string id, [FromBody] DeleteProductTypeTypeDetailDTO deleteProductTypeTypeDetailDTO)
        {
            try
            {
                await _productTypeTypeDetailService.DeleteAsync(id, deleteProductTypeTypeDetailDTO.DeletedBy);

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
