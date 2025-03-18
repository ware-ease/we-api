using API.Utils;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Services;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.Brand;
using Data.Model.Request.ProductType;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/product-types")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypesService _productTypeService;

        public ProductTypeController(IProductTypesService productTypesService)
        {
            _productTypeService = productTypesService;
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _productTypeService.Count();
            var response = new ServiceResponse
            {
                Data = count,
                Status = SRStatus.Success,
                Message = "Total product type count retrieved successfully"
            };

            return ControllerResponse.Response(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _productTypeService.GetAllProducts();
            return ControllerResponse.Response(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _productTypeService.Get<ProductTypeDTO>(id);
            return ControllerResponse.Response(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductTypeCreateDTO request)
        {
            try
            {
                var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

                if (authUser != null)
                {
                    request.CreatedBy = authUser.id;
                }

                var result = await _productTypeService.AddProductType(request);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "ProductType created successfully",
                    Data = result
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ProductTypeUpdateDTO request)
        {
            try
            {
                request.Id = id;
                var result = await _productTypeService.UpdateProductType(request);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "ProductType updated successfully",
                    Data = result
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
            var result = await _productTypeService.Delete(id);
            return ControllerResponse.Response(result);
        }
    }
}
