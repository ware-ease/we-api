using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Floor;
using BusinessLogicLayer.Models.General;
using BusinessLogicLayer.Models.Shelf;
using BusinessLogicLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/floors")]
    [ApiController]
    public class FloorController : ControllerBase
    {
        private readonly IFloorService _floorService;
        private readonly IMapper _mapper;

        public FloorController(IFloorService floorService, IMapper mapper)
        {
            _floorService = floorService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _floorService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("GetAllByShelfId")]
        public async Task<IActionResult> GetAllByShelfId(string shelfId,[FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _floorService.GetAllByShelfIdAsync(shelfId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {

            try
            {
                var floor = await _floorService.GetByIdAsync(id);
                return Ok(new { Data = floor });
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
        public async Task<IActionResult> Create([Required] string shelfId, [FromBody] CreateFloorDTO createFloorDTO)
        {

            try
            {


                var floor = await _floorService.AddAsync(shelfId, createFloorDTO);
                var response = new
                {
                    Data = floor
                };
                return CreatedAtAction(nameof(GetById), new { id = floor.Id }, response);
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
        public async Task<IActionResult> Update(string floorId, [FromBody] UpdateFloorDTO updateFloorDTO)
        {
            try
            {
                var updateFloor = await _floorService.UpdateAsync(floorId, updateFloorDTO);
                return Ok(new
                {
                    Data = updateFloor
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
        public async Task<IActionResult> Delete(string id, [FromBody] DeleteDTO deleteDTO)
        {
            try
            {
                await _floorService.DeleteAsync(id, deleteDTO.DeletedBy);

                return Ok(new { StatusCode = StatusCodes.Status200OK, Message = "Xóa Floor thành công", Data = (object)null });
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
