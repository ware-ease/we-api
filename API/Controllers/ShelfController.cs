using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.PurchaseDetail;
using BusinessLogicLayer.Models.Shelf;
using BusinessLogicLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShelfController : ControllerBase
    {
        private readonly IShelfService _shelfService;
        private readonly IMapper _mapper;

        public ShelfController(IShelfService shelfService, IMapper mapper)
        {
            _shelfService = shelfService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _shelfService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("GetAllByWarehouseId")]
        public async Task<IActionResult> GetAllByWarehouseId([Required]string warehouseId,[FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _shelfService.GetAllByWarehouseIdAsync(warehouseId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {

            try
            {
                var shelf = await _shelfService.GetByIdAsync(id);
                return Ok(new { Data = shelf });
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
        public async Task<IActionResult> Create([Required] string warehouseId, [FromBody] CreateShelfDTO createShelfDTO)
        {

            try
            {


                var shelf = await _shelfService.AddAsync(warehouseId, createShelfDTO);
                var response = new
                {
                    Data = shelf
                };
                return CreatedAtAction(nameof(GetById), new { id = shelf.Id }, response);
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
        public async Task<IActionResult> Update(string shelfId, [FromBody] UpdateShelfDTO updateShelfDTO)
        {
            try
            {
                var updateShelf = await _shelfService.UpdateAsync(shelfId, updateShelfDTO);
                return Ok(new
                {
                    Data = updateShelf
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
        public async Task<IActionResult> Delete(string id, [FromBody] DeleteShelfDTO deleteShelfDTO)
        {
            try
            {
                await _shelfService.DeleteAsync(id, deleteShelfDTO.DeletedBy);

                return Ok(new { StatusCode = StatusCodes.Status200OK, Message = "Xóa Shelf thành công", Data = (object)null });
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
