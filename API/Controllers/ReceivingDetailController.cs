using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.PurchaseDetail;
using BusinessLogicLayer.Models.ReceivingDetail;
using BusinessLogicLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceivingDetailController : ControllerBase
    {
        private readonly IReceivingDetailService _receivingDetailService;
        private readonly IMapper _mapper;

        public ReceivingDetailController(IReceivingDetailService receivingDetailService, IMapper mapper)
        {
            _receivingDetailService = receivingDetailService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _receivingDetailService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {

            try
            {
                var receivingDetail = await _receivingDetailService.GetByIdAsync(id);
                return Ok(new { Data = receivingDetail });
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
        public async Task<IActionResult> Create([Required] string noteId, [Required] string productTypeId, [FromBody] CreateReceivingDetailDTO createReceivingDetailDTO)
        {

            try
            {


                var receivingDetail = await _receivingDetailService.AddAsync(noteId, productTypeId, createReceivingDetailDTO);
                var response = new
                {
                    Data = receivingDetail
                };
                return CreatedAtAction(nameof(GetById), new { id = receivingDetail.Id }, response);
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
        public async Task<IActionResult> Update(string receivingId, [FromBody] UpdateReceivingDetailDTO updateReceivingDetailDTO)
        {
            try
            {
                var updateRereceivingDetail = await _receivingDetailService.UpdateAsync(receivingId, updateReceivingDetailDTO);
                return Ok(new
                {
                    Data = updateRereceivingDetail
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
        public async Task<IActionResult> Delete(string id, [FromBody] DeleteReceivingDetailDTO deleteReceivingDetailDTO)
        {
            try
            {
                await _receivingDetailService.DeleteAsync(id, deleteReceivingDetailDTO.DeletedBy);

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
