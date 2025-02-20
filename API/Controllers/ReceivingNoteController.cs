using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.General;
using BusinessLogicLayer.Models.PurchaseReceipt;
using BusinessLogicLayer.Models.ReceivingNote;
using BusinessLogicLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceivingNoteController : ControllerBase
    {
        private readonly IReceivingNoteService _receivingNoteService;
        private readonly IMapper _mapper;

        public ReceivingNoteController(IReceivingNoteService receivingNoteService, IMapper mapper)
        {
            _receivingNoteService = receivingNoteService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _receivingNoteService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {

            try
            {
                var supplier = await _receivingNoteService.GetByIdAsync(id);
                return Ok(new { Data = supplier });
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
        public async Task<IActionResult> Create([Required] string supplierId, [Required] string purchaseId, [FromBody] CreateReceivingNoteDTO createReceivingNoteDTO)
        {

            try
            {
                var purchaseReceipt = await _receivingNoteService.AddAsync(supplierId, purchaseId, createReceivingNoteDTO);
                var response = new
                {
                    Data = purchaseReceipt
                };
                return CreatedAtAction(nameof(GetById), new { id = purchaseReceipt.Id }, response);
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
        public async Task<IActionResult> Update(string noteId, [FromBody] UpdateReceivingNoteDTO updateReceivingNoteDTO)
        {
            try
            {
                var updatePurchaseReceipt = await _receivingNoteService.UpdateAsync(noteId, updateReceivingNoteDTO);
                return Ok(new
                {
                    Data = updatePurchaseReceipt
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
        public async Task<IActionResult> Delete(string id, [FromBody] DeleteDTO deleteReceivingNoteDTO)
        {
            try
            {
                await _receivingNoteService.DeleteAsync(id, deleteReceivingNoteDTO.DeletedBy);

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
