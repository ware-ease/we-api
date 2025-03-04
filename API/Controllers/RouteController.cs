using BusinessLogicLayer.IServices;
using Data.Model.DTO;
using Data.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/routes")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RouteController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _routeService.Get<RouteDTO>();

            return ControllerResponse.Response(result);
        }
    }
}
