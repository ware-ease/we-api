using API.Utils;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Services;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.Brand;
using Data.Model.Request.ErrorTicket;
using Data.Model.Request.Product;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/error-ticket")]
    [ApiController]
    public class ErrorTicketController : ControllerBase
    {
        private readonly IErrorTicketService _errorTicketService;
        public ErrorTicketController(IErrorTicketService errorTicketService)
        {
            _errorTicketService = errorTicketService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchPartners([FromQuery] int pageIndex = 1,
                                                        [FromQuery] int pageSize = 5,
                                                        [FromQuery] string? keyword = null)
        {
            var response = await _errorTicketService.Search<ErrorTicketDTO>(
                pageIndex, pageSize, keyword);

            return ControllerResponse.Response(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _errorTicketService.Get<ErrorTicketDTO>(id);
            return ControllerResponse.Response(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ErrorTicketCreateDTO request)
        {
            try
            {
                var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

                if (authUser != null)
                {
                    request.CreatedBy = authUser.id;
                }

                var errorTicket = await _errorTicketService.AddErrorTicket(request);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "ErrorTicket created successfully",
                    Data = errorTicket
                });
            }
            catch (Exception ex)
            {
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ErrorTicketUpdateDTO request)
        {
            try
            {
                request.Id = id;
                var updatedErrorTicket = await _errorTicketService.UpdateErrorTicket(request);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "ErrorTicket updated successfully",
                    Data = updatedErrorTicket
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
            var result = await _errorTicketService.Delete(id);
            return ControllerResponse.Response(result);
        }
    }
}
