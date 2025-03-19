using BusinessLogicLayer.IServices;
using Data.Model.Request.Partner;
using Data.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace API.Controllers
{
    [Route("api/partners")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService _partnerService;

        public PartnerController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _partnerService.GetAll<PartnerDTO>();
            return ControllerResponse.Response(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _partnerService.GetById<PartnerDTO>(id);
            return ControllerResponse.Response(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PartnerCreateDTO request)
        {
            var result = await _partnerService.CreateAsync<PartnerDTO>(request);
            return ControllerResponse.Response(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] PartnerUpdateDTO request)
        {
            var result = await _partnerService.UpdateAsync<PartnerDTO>(id, request);
            return ControllerResponse.Response(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _partnerService.DeleteAsync(id);
            return ControllerResponse.Response(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchPartners([FromQuery] int pageIndex = 1,
                                                        [FromQuery] int pageSize = 5,
                                                        [FromQuery] string? keyword = null,
                                                        [FromQuery] int? partnerType = null)
        {
            var response = await _partnerService.SearchPartners<PartnerDTO>(
                pageIndex, pageSize, keyword, partnerType);

            return ControllerResponse.Response(response);
        }
    }
}
