using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.PurchaseReceipt;
using BusinessLogicLayer.Models.TypeDetail;
using BusinessLogicLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeDetailController : ControllerBase
    {
        private readonly ITypeDetailService _typeDetailService;
        private readonly IMapper _mapper;

        public TypeDetailController(ITypeDetailService typeDetailService, IMapper mapper)
        {
            _typeDetailService = typeDetailService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _typeDetailService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {

            try
            {
                var typeDetail = await _typeDetailService.GetByIdAsync(id);
                return Ok(new { StatusCode = StatusCodes.Status200OK, Message = "Thành công", Data = typeDetail });
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
        public async Task<IActionResult> Create([FromBody] CreateTypeDetailDTO createTypeDetailDTO)
        {

            try
            {


                var typeDetail = await _typeDetailService.AddAsync(createTypeDetailDTO);
                var response = new
                {
                    Data = typeDetail
                };
                return CreatedAtAction(nameof(GetById), new { id = typeDetail.Id }, response);
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
        public async Task<IActionResult> Update(string typeDetailId, [FromBody] UpdateTypeDetailDTO updateTypeDetailDTO)
        {
            try
            {
                var updateTypeDetail = await _typeDetailService.UpdateAsync(typeDetailId, updateTypeDetailDTO);
                return Ok(new
                {
                    Data = updateTypeDetailDTO
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
        public async Task<IActionResult> Delete(string id, [FromBody] DeleteTypeDetailDTO deleteTypeDetailDTO)
        {
            try
            {
                await _typeDetailService.DeleteAsync(id, deleteTypeDetailDTO.DeletedBy);

                return Ok(new { StatusCode = StatusCodes.Status200OK, Message = "Xóa PurchaseReceipt thành công", Data = (object)null });
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
