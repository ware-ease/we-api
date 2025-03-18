using API.Utils;
using AutoMapper;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Models.General;
using BusinessLogicLayer.Models.Product;
using BusinessLogicLayer.Models.PurchaseReceipt;
using BusinessLogicLayer.Service;
using BusinessLogicLayer.Services;
using Data.Entity;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.Product;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        //private readonly IMapper _mapper;

        public ProductController(IProductService productService)
        {
            _productService = productService;
            //_mapper = mapper;
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _productService.Count();
            var response = new ServiceResponse
            {
                Data = count,
                Status = SRStatus.Success,
                Message = "Total product count retrieved successfully"
            };

            return ControllerResponse.Response(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var products = await _productService.Get<ProductDTO>();
            return ControllerResponse.Response(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var productDTO = await _productService.GetProductById(id);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Product retrieved successfully",
                    Data = productDTO
                });
            }
            catch (Exception ex)
            {
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductCreateDTO request)
        {
            try
            {
                var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

                if (authUser != null)
                {
                    request.CreatedBy = authUser.id;
                }

                var productDTO = await _productService.AddProduct(request);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Product created successfully",
                    Data = productDTO
                });
            }
            catch (Exception ex)
            {
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ProductUpdateDTO request)
        {
            try
            {
                request.Id = id;
                var updatedProduct = await _productService.UpdateProduct(request);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Product updated successfully",
                    Data = updatedProduct
                });
            }
            catch (Exception ex)
            {
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Error,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _productService.Delete(id);
            return ControllerResponse.Response(result);
        }
        /*[HttpGet("GetAll")]
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
        public async Task<IActionResult> GetById(string id)
        {

            try
            {
                var product = await _productService.GetByIdAsync(id);
                return Ok(new {  Data = product });
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
                var product = await _productService.AddAsync(categoryId, createProductDTO);
                var response = new
                {
                    Data = product
                };
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
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
        public async Task<IActionResult> Update(string productId, [FromBody] UpdateProductDTO updateProductDTO)
        {
            try
            {
                var product = await _productService.UpdateAsync(productId, updateProductDTO);
                return Ok(new
                {
                    Data = product
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
        public async Task<IActionResult> Delete(string id, [FromBody] DeleteDTO deleteProductDTO)
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
        }*/
    }
}
