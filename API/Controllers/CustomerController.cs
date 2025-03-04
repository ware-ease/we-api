using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.Customer;
using Data.Model.Response;
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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _customerService.Get<CustomerDTO>();
            _customerService.Test();

            return ControllerResponse.Response(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _customerService.Get<CustomerDTO>(id);

            return ControllerResponse.Response(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CustomerCreateDTO request)
        {
            var result = await _customerService.Add<CustomerDTO, CustomerCreateDTO>(request);

            return ControllerResponse.Response(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CustomerUpdateDTO request)
        {
            request.Id = id;
            var result = await _customerService.Update<CustomerDTO, CustomerUpdateDTO>(request);

            return ControllerResponse.Response(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _customerService.Delete(id);

            return ControllerResponse.Response(result);
        }
    }
}
