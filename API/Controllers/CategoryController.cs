using API.Utils;
using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.Models.Category;
using BusinessLogicLayer.Models.General;
using BusinessLogicLayer.Service;
using Data.Entity;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.Category;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        //private readonly IMapper _mapper;

        public CategoryController(ICategoryService service)
        {
            _categoryService = service;
            //_mapper = mapper;
        }
        [Authorize]
        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _categoryService.Count();
            var response = new ServiceResponse
            {
                Data = count,
                Status = SRStatus.Success,
                Message = "Đã đếm xong số lượng category"
            };

            return ControllerResponse.Response(response);
        }
        [Authorize]
        [NonAction]
        public async Task<IActionResult> Get()
        {
            var result = await _categoryService.Get<CategoryDTO>();
            return ControllerResponse.Response(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] int pageIndex = 1,
                                                        [FromQuery] int pageSize = 5,
                                                        [FromQuery] string? keyword = null)
        {
            var response = await _categoryService.Search<CategoryDTO>(
                pageIndex, pageSize, keyword);

            return ControllerResponse.Response(response);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _categoryService.Get<CategoryDTO>(id);
            return ControllerResponse.Response(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CategoryCreateDTO request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }
            else
            {
                return Unauthorized();
            }

            var result = await _categoryService.Add<CategoryDTO, CategoryCreateDTO>(request);
            return ControllerResponse.Response(result);
        }
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CategoryUpdateDTO request)
        {
            try
            {
                request.Id = id;
                var result = await _categoryService.UpdateCategory(request);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Category đã sửa thành công",
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _categoryService.Delete(id);
            return ControllerResponse.Response(result);
        }
    }
}
