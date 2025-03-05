using BusinessLogicLayer.IServices;
using Data.Model.DTO;
using Data.Model.Request.Brand;
using Data.Model.Request.Unit;
using Data.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/units")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _unitService;

        public UnitController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _unitService.Get<UnitDTO>();
            return ControllerResponse.Response(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _unitService.Get<UnitDTO>(id);
            return ControllerResponse.Response(result);
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] UnitCreateDTO request)
        {
            /*var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }
            else
            {
                return Unauthorized();
            }*/

            var result = await _unitService.Add<UnitDTO, UnitCreateDTO>(request);
            return ControllerResponse.Response(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UnitUpdateDTO request)
        {
            request.Id = id;
            var result = await _unitService.Update<UnitDTO, UnitUpdateDTO>(request);
            return ControllerResponse.Response(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _unitService.Delete(id);
            return ControllerResponse.Response(result);
        }
    }
}
