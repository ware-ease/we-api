using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.General;
using BusinessLogicLayer.Models.StockCard;
using BusinessLogicLayer.Models.StockCardDetail;
using BusinessLogicLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockCardDetailController : ControllerBase
    {
        private readonly IStockCardDetailService _stockCardDetailService;
        private readonly IMapper _mapper;

        public StockCardDetailController(IStockCardDetailService stockCardDetailService, IMapper mapper)
        {
            _stockCardDetailService = stockCardDetailService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _stockCardDetailService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("GetAllByStockCardId")]
        public async Task<IActionResult> GetAllByStockCardId(string stockCardId, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _stockCardDetailService.GetQueryableByStockCardId(stockCardId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("GetAllByProductTypeId")]
        public async Task<IActionResult> GetAllByProductTypeId(string productTypeId, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _stockCardDetailService.GetQueryableByProductTypeId(productTypeId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{stockCardId}/{productTypeId}")]
        public async Task<IActionResult> GetById(string stockCardId, string productTypeId)
        {

            try
            {
                var stockCardDetail = await _stockCardDetailService.GetByIdAsync(stockCardId, productTypeId);
                return Ok(new { Data = stockCardDetail });
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
        public async Task<IActionResult> Create([Required] string stockCardId,[Required] string productTypeId, 
            [FromBody] CreateStockCardDetailDTO createStockCardDetailDTO)
        {

            try
            {
                var stockCardDetail = await _stockCardDetailService.AddAsync(stockCardId, productTypeId, createStockCardDetailDTO);
                var response = new
                {
                    Data = stockCardDetail
                };
                return CreatedAtAction(nameof(GetById), new { 
                    stockCardId = stockCardDetail.StockCardId, 
                    //productTypeId = stockCardDetail.ProductTypeId 
                }, response);

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

        [HttpPut("stockCardId/productTypeId")]
        public async Task<IActionResult> Update(string stockCardId, string productTypeId, [FromBody] UpdateStockCardDetailDTO updateStockCardDetailDTO)
        {
            try
            {
                var updateStockCardDetail = await _stockCardDetailService.UpdateAsync(stockCardId, productTypeId, updateStockCardDetailDTO);
                return Ok(new
                {
                    Data = updateStockCardDetail
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
        public async Task<IActionResult> Delete(string stockCardId, string productTypeId, [FromBody] DeleteDTO deleteDTO)
        {
            try
            {
                await _stockCardDetailService.DeleteAsync(stockCardId, productTypeId, deleteDTO.DeletedBy);

                return Ok(new { StatusCode = StatusCodes.Status200OK, Message = "Xóa StockCardDetail thành công", Data = (object)null });
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
