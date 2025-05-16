using API.Utils;
using BusinessLogicLayer.IServices;
using Data.Enum;
using Data.Model.Request.GoodRequest;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/good-requests")]
    [ApiController]
    public class GoodRequestController : ControllerBase
    {
        private readonly IGoodRequestService _goodRequestService;

        public GoodRequestController(IGoodRequestService goodRequestService)
        {
            _goodRequestService = goodRequestService;
        }
        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> Search([FromQuery] int pageIndex = 1,
                                                [FromQuery] int pageSize = 5,
                                                [FromQuery] string? keyword = null,
                                                [FromQuery] GoodRequestEnum? requestType = null,
                                                [FromQuery] GoodRequestStatusEnum? status = null)
        {
            var response = await _goodRequestService.SearchGoodRequests(
                pageIndex, pageSize, keyword, requestType, status
            );
            return ControllerResponse.Response(response);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _goodRequestService.GetById(id);
            return ControllerResponse.Response(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateGoodRequestAsync([FromBody] GoodRequestCreateDTO request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }
            var result = await _goodRequestService.CreateAsync<GoodRequestDTO>(request);
            return ControllerResponse.Response(result);
        }
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] GoodRequestUpdateDTO request)
        {
            request.Id = id;
            var result = await _goodRequestService.UpdateAsync(id, request);
            return ControllerResponse.Response(result);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _goodRequestService.Delete(id);
            return ControllerResponse.Response(result);
        }
        [Authorize]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromQuery] GoodRequestStatusEnum requestStatus, string? statusNote)
        {
            var response = await _goodRequestService.UpdateStatusAsync(id, requestStatus, statusNote);
            return ControllerResponse.Response(response);
        }
    }
}