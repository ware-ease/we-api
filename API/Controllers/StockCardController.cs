using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Cell;
using BusinessLogicLayer.Models.General;
using BusinessLogicLayer.Models.StockCard;
using BusinessLogicLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockCardController : ControllerBase
    {
        private readonly IStockCardService _stockCardService;
        private readonly IMapper _mapper;

        public StockCardController(IStockCardService stockCardService, IMapper mapper)
        {
            _stockCardService = stockCardService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _stockCardService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("GetAllByCellId")]
        public async Task<IActionResult> GetAllByCellId(string cellId, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _stockCardService.GetAllStockCardByCellId(cellId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {

            try
            {
                var stockCard = await _stockCardService.GetByIdAsync(id);
                return Ok(new { Data = stockCard });
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
        public async Task<IActionResult> Create([Required] string cellId, [FromBody] CreateStockCardDTO createStockCardDTO)
        {

            try
            {
                var stockCard = await _stockCardService.AddAsync(cellId, createStockCardDTO);
                var response = new
                {
                    Data = stockCard
                };
                return CreatedAtAction(nameof(GetById), new { id = stockCard.Id }, response);
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
        public async Task<IActionResult> Update(string stockCardId, [FromBody] UpdateStockCardDTO updateStockCardDTO)
        {
            try
            {
                var updateStockCard = await _stockCardService.UpdateAsync(stockCardId, updateStockCardDTO);
                return Ok(new
                {
                    Data = updateStockCard
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
                await _stockCardService.DeleteAsync(id, deleteDTO.DeletedBy);

                return Ok(new { StatusCode = StatusCodes.Status200OK, Message = "Xóa StockCard thành công", Data = (object)null });
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
