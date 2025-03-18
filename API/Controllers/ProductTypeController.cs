using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Services;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.Brand;
using Data.Model.Request.ProductType;
using Data.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/productTypes")]
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
            var result = await _productTypeService.Get<ProductTypeDTO>();
            return ControllerResponse.Response(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _productTypeService.Get<ProductTypeDTO>(id);
            return ControllerResponse.Response(result);
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductTypeCreateDTO request)
        {
            /*var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }
            else
            {
                return Unauthorized();
            }*/

            var result = await _productTypeService.Add<ProductTypeDTO, ProductTypeCreateDTO>(request);
            return ControllerResponse.Response(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] BrandUpdateDTO request)
        {
            request.Id = id;
            var result = await _productTypeService.Update<BrandDTO, BrandUpdateDTO>(request);
            return ControllerResponse.Response(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _productTypeService.Delete(id);
            return ControllerResponse.Response(result);
        }
    }
}
