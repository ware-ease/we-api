using API.Middlewares;
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
using Data.Model.Request.Partner;
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
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho,Nhân viên bán hàng")]
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
        [Authorize]
        [NonAction]
        public async Task<IActionResult> Get()
        {
            var products = await _productService.Get<ProductDTO>();
            return ControllerResponse.Response(products);
        }
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho,Nhân viên bán hàng")]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] int pageIndex = 1,
                                                        [FromQuery] int pageSize = 5,
                                                        [FromQuery] bool? IsBatchManaged = null,
                                                        [FromQuery] string? keyword = null)
        {
            var response = await _productService.Search<ProductDTO>(
                pageIndex, pageSize, IsBatchManaged, keyword);

            return ControllerResponse.Response(response);
        }
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho,Nhân viên bán hàng")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var productDTO = await _productService.Get<ProductDTO>(id);
                return ControllerResponse.Response(productDTO);
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
        [AuthorizeGroup("Admin,Thủ kho")]
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
                    Message = "Product đã tạo thành công",
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
        [AuthorizeGroup("Admin,Thủ kho")]
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
                    Message = "Product đã sửa thành công",
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
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _productService.Delete(id);
            return ControllerResponse.Response(result);
        }

    }
}
