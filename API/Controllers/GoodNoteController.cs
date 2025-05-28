using API.Middlewares;
using API.Utils;
using BusinessLogicLayer.IServices;
using Data.Enum;
using Data.Model.Request.GoodNote;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
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
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _goodNoteService.GetById(id);
            return ControllerResponse.Response(result);
        }

        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpPost("receive-note")]
        public async Task<IActionResult> CreateReceiveNote([FromBody] GoodNoteCreateDTO request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }

            var result = await _goodNoteService.CreateReceiveNoteAsync(request);
            return ControllerResponse.Response(result);
        }

        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpPost("issue-note")]
        public async Task<IActionResult> CreateIssueNote([FromBody] GoodNoteIssueCreateDTO request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }

            var result = await _goodNoteService.CreateIssueNoteAsync(request, CodeType.PX); //PX is the code for issue note
            return ControllerResponse.Response(result);
        }  
        
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpPost("internal-issue-note")]
        public async Task<IActionResult> CreateInternalIssueNote([FromBody] GoodNoteIssueCreateDTO request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }

            var result = await _goodNoteService.CreateIssueNoteAsync(request, CodeType.PXNB); //PXNB is the code for internal issue note
            return ControllerResponse.Response(result);
        }

        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpPost("internal-receive-note")]
        public async Task<IActionResult> CreateInternalReceiveNote([FromBody] GoodNoteCreateDTOv2 request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }

            var result = await _goodNoteService.CreateReceiveNoteWithExistingBatchAsync(request, CodeType.PNNB);
            return ControllerResponse.Response(result);
        }    
        
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpPost("return-receive-note")]
        public async Task<IActionResult> CreateReturnReceiveNote([FromBody] GoodNoteCreateDTOv2 request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);
            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }

            var result = await _goodNoteService.CreateReceiveNoteWithExistingBatchAsync(request, CodeType.PN);
            return ControllerResponse.Response(result);
        }
    }
}

