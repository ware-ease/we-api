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

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _categoryService.Count();
            var response = new ServiceResponse
            {
                Data = count,
                Status = SRStatus.Success,
                Message = "Total category count retrieved successfully"
            };

            return ControllerResponse.Response(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _categoryService.Get<CategoryDTO>();
            return ControllerResponse.Response(result);
        }

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
                    Message = "Category updated successfully",
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
            var result = await _categoryService.Delete(id);
            return ControllerResponse.Response(result);
        }

        /*//Get all
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _categoryService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        //Creat
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDTO categoryDto)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDto);
                await _categoryService.AddAsync(category);

                var response = new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Tạo Category thành công",
                    Data = category
                };
                return CreatedAtAction(nameof(GetById), new { id = category.Id }, response);
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
                var errorResponse = new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Đã xảy ra lỗi: " + ex.Message,
                    Data = (object)null
                };
                return StatusCode(500, errorResponse);
            }
        }

        //Update
        [HttpPut("Update")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCategoryDTO updateCategoryDTO)
        {
            try
            {
                var category = _mapper.Map<Category>(updateCategoryDTO);
                await _categoryService.UpdateAsync(id, category);

                var response = new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Cập nhật Category thành công",
                    Data = category
                };

                return CreatedAtAction(nameof(GetById), new { id = category.Id }, response);
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
        public async Task<IActionResult> Delete(string id, [FromBody] DeleteDTO deleteCategoryDTO)
        {
            try
            {
                var category = _mapper.Map<Category>(deleteCategoryDTO);
                await _categoryService.DeleteAsync(id, category);

                return Ok(new { StatusCode = StatusCodes.Status200OK, Message = "Xóa Category thành công", Data = (object)null });
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

        //Get by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                return Ok(new { StatusCode = StatusCodes.Status200OK, Message = "Thành công", Data = category });
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
