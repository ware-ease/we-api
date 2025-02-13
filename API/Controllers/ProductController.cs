using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Product;
using BusinessLogicLayer.Models.PurchaseReceipt;
using BusinessLogicLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _productService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("GetAllByCategoryId")]
        public async Task<IActionResult> GetAllByCategoryId(string categoryId, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _productService.GetAllByCategoryIdAsync(categoryId,pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {

            try
            {
                var supplier = await _productService.GetByIdAsync(id);
                return Ok(new {  Data = supplier });
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
        public async Task<IActionResult> Create([Required] string categoryId, [FromBody] CreateProductDTO createProductDTO)
        {

            try
            {
                var purchaseReceipt = await _productService.AddAsync(categoryId, createProductDTO);
                var response = new
                {
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
        public async Task<IActionResult> Update(int productId, [FromBody] UpdateProductDTO updateProductDTO)
        {
            try
            {
                var updatePurchaseReceipt = await _productService.UpdateAsync(productId, updateProductDTO);
                return Ok(new
                {
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
        public async Task<IActionResult> Delete(int id, [FromBody] DeleteProductDTO deleteProductDTO)
        {
            try
            {
                await _productService.DeleteAsync(id, deleteProductDTO.DeletedBy);

                return Ok(new {  Data = (object)null });
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
