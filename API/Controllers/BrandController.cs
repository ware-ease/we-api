using API.Utils;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Models.Category;
using Data.Model.DTO;
using Data.Model.Request.Brand;
using Data.Model.Request.Category;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/brands")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _brandService.Get<BrandDTO>();
            return ControllerResponse.Response(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _brandService.Get<BrandDTO>(id);
            return ControllerResponse.Response(result);
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] BrandCreateDTO request)
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

            var result = await _brandService.Add<BrandDTO, BrandCreateDTO>(request);
            return ControllerResponse.Response(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] BrandUpdateDTO request)
        {
            request.Id = id;
            var result = await _brandService.Update<BrandDTO, BrandUpdateDTO>(request);
            return ControllerResponse.Response(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _brandService.Delete(id);
            return ControllerResponse.Response(result);
        }
    }
}
