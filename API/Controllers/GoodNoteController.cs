using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Services;
using Data.Enum;
using Data.Model.Request.GoodNote;
using Data.Model.Request.GoodRequest;
using Data.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/good-notes")]
    [ApiController]
    public class GoodNoteController : ControllerBase
    {
        private readonly IGoodNoteService _goodNoteService;

        public GoodNoteController(IGoodNoteService goodNoteService)
        {
            _goodNoteService = goodNoteService;
        }

        [HttpGet()]
        public async Task<IActionResult> Search([FromQuery] int pageIndex = 1,
                                                [FromQuery] int pageSize = 5,
                                                [FromQuery] string? keyword = null,
                                                [FromQuery] GoodNoteEnum? noteType = null)
        {
            var response = await _goodNoteService.SearchGoodNotes<GoodNoteDTO>(
                pageIndex, pageSize, keyword, noteType
            );
            return ControllerResponse.Response(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _goodNoteService.GetById<GoodNoteDTO>(id);
            return ControllerResponse.Response(result);
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] GoodNoteCreateDTO request)
        {
            //var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            //if (authUser != null)
            //{
            //    request.CreatedBy = authUser.id;
            //}

            var result = await _goodNoteService.CreateAsync<GoodNoteCreateDTO>(request);
            return ControllerResponse.Response(result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromQuery] GoodNoteStatusEnum noteStatus)
        {
            var response = await _goodNoteService.UpdateStatusAsync(id, noteStatus);
            return ControllerResponse.Response(response);
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] GoodNoteUpdateDTO request)
        {
            request.Id = id;
            var result = await _goodNoteService.UpdateAsync(id, request);
            return ControllerResponse.Response(result);
        }
    }
}

