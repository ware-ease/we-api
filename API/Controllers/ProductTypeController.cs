using API.Middlewares;
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
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
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
        [Authorize]
        [NonAction]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _productTypeService.Get<ProductTypeDTO>();
                return ControllerResponse.Response(result);
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
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] int pageIndex = 1,
                                                        [FromQuery] int pageSize = 5,
                                                        [FromQuery] string? keyword = null)
        {
            var response = await _productTypeService.Search<ProductTypeDTO>(
                pageIndex, pageSize, keyword);

            return ControllerResponse.Response(response);
        }
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var result = await _productTypeService.Get<ProductTypeDTO>(id);
                return ControllerResponse.Response(result);
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
                    Message = "ProductType đã tạo thành công",
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
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
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
                    Message = "ProductType đã sửa thành công",
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
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _productTypeService.Delete(id);
            return ControllerResponse.Response(result);
        }
    }
}
