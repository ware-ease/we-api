using API.Utils;
using BusinessLogicLayer.IServices;
using Data.Model.Request.GoodRequest;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _goodRequestService.GetAll<GoodRequestDTO>();
            return ControllerResponse.Response(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _goodRequestService.GetById<GoodRequestDTO>(id);
            return ControllerResponse.Response(result);
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] GoodRequestCreateDTO request)
        {
            //var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            //if (authUser != null)
            //{
            //    request.CreatedBy = authUser.id;
            //}

            var result = await _goodRequestService.CreateAsync<GoodRequestDTO>(request);
            return ControllerResponse.Response(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] GoodRequestUpdateDTO request)
        {
            request.Id = id;
            var result = await _goodRequestService.UpdateAsync<GoodRequestDTO>(id, request);
            return ControllerResponse.Response(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _goodRequestService.Delete(id);
            return ControllerResponse.Response(result);
        }
    }
}