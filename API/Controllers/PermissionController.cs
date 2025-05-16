using BusinessLogicLayer.IService;
using BusinessLogicLayer.IServices;
using Data.Model.DTO;
using Data.Model.Request.Customer;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/permissions")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _permissionService.Get<PermissionDTO>();

            return ControllerResponse.Response(result);
        }
    }
}