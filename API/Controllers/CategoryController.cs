using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.Models.Category;
using BusinessLogicLayer.Models.General;
using BusinessLogicLayer.Service;
using Data.Entity;
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
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService service, IMapper mapper)
        {
            _categoryService = service;
            _mapper = mapper;
        }

        //Get all
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
        }
    }
}
