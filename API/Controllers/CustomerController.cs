using API.Utils;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Services;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.Customer;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace API.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [Authorize]
        [NonAction]
        public async Task<IActionResult> Get()
        {
            var result = await _customerService.Get<CustomerDTO>();
            //_customerService.Test();

            return ControllerResponse.Response(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SearchPartners([FromQuery] int pageIndex = 1,
                                                        [FromQuery] int pageSize = 5,
                                                        [FromQuery] string? keyword = null)
        {
            var response = await _customerService.Search<CustomerDTO>(
                pageIndex, pageSize, keyword);

            return ControllerResponse.Response(response);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _customerService.Get<CustomerDTO>(id);

            return ControllerResponse.Response(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CustomerCreateDTO request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }

            var result = await _customerService.Add<CustomerDTO, CustomerCreateDTO>(request);

            return ControllerResponse.Response(result);
        }
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] CustomerUpdateDTO request)
        {
            try
            {
                request.Id = id;
                var result = await _customerService.UpdateCustomer(request);

                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Customer update thành công",
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
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _customerService.Delete(id);

            return ControllerResponse.Response(result);
        }
    }
}