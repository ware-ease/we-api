using API.Utils;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Services;
using Data.Enum;
using Data.Model.Request.GoodNote;
using Data.Model.Request.GoodRequest;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
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
                                                [FromQuery] GoodNoteEnum? noteType = null,
                                                [FromQuery] GoodNoteStatusEnum? status = null,
                                                [FromQuery] string? requestedWarehouseId = null)
        {
            var response = await _goodNoteService.SearchGoodNotes(
                pageIndex, pageSize, keyword, noteType, status, requestedWarehouseId
            );
            return ControllerResponse.Response(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _goodNoteService.GetById(id);
            return ControllerResponse.Response(result);
        }

        [Authorize]
        [HttpPost("receive-note")]
        public async Task<IActionResult> CreateReceiveNote([FromBody] GoodNoteCreateDTO request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
                //request.GoodNoteDetails.ForEach(x =>
                //{
                //    x.CreatedBy = authUser.id;
                //    x.NewBatch!.CreatedBy = authUser.id;
                //});
            }

            var result = await _goodNoteService.CreateReceiveNoteAsync(request);
            return ControllerResponse.Response(result);
        }

        [Authorize]
        [HttpPost("issue-note")]
        public async Task<IActionResult> CreateIssueNote([FromBody] GoodNoteIssueCreateDTO request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }

            var result = await _goodNoteService.CreateIssueNoteAsync(request, CodeType.PX); //PX là mã code cho phiếu xuất hàng Quan trọng
            return ControllerResponse.Response(result);
        }  
        
        [Authorize]
        [HttpPost("internal-issue-note")]
        public async Task<IActionResult> CreateInternalIssueNote([FromBody] GoodNoteIssueCreateDTO request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }

            var result = await _goodNoteService.CreateIssueNoteAsync(request, CodeType.PXNB); //PXNB là mã code cho phiếu xuất nội bộ Quan trọng
            return ControllerResponse.Response(result);
        }

        [Authorize]
        [HttpPost("internal-receive-note")]
        public async Task<IActionResult> CreateInternalReceiveNote([FromBody] GoodNoteCreateDTOv2 request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
                //request.GoodNoteDetails.ForEach(x =>
                //{
                //    x.CreatedBy = authUser.id;
                //    x.NewBatch!.CreatedBy = authUser.id;
                //});
            }

            var result = await _goodNoteService.CreateReceiveNoteWithExistingBatchAsync(request, CodeType.PNNB);
            return ControllerResponse.Response(result);
        }    
        
        [Authorize]
        [HttpPost("return-receive-note")]
        public async Task<IActionResult> CreateReturnReceiveNote([FromBody] GoodNoteCreateDTOv2 request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
                //request.GoodNoteDetails.ForEach(x =>
                //{
                //    x.CreatedBy = authUser.id;
                //    x.NewBatch!.CreatedBy = authUser.id;
                //});
            }

            var result = await _goodNoteService.CreateReceiveNoteWithExistingBatchAsync(request, CodeType.PN);
            return ControllerResponse.Response(result);
        }
        //[HttpPut("{id}/status")]
        //public async Task<IActionResult> UpdateStatus(string id, [FromQuery] GoodNoteStatusEnum noteStatus)
        //{
        //    var response = await _goodNoteService.UpdateStatusAsync(id, noteStatus);
        //    return ControllerResponse.Response(response);
        //}
        //[HttpPatch("{id}")]
        //public async Task<IActionResult> Update([FromRoute] string id, [FromBody] GoodNoteUpdateDTO request)
        //{
        //    request.Id = id;
        //    var result = await _goodNoteService.UpdateAsync(id, request);
        //    return ControllerResponse.Response(result);
        //}
    }
}

