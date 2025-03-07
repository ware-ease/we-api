using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.IService;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BusinessLogicLayer.IServices;
using Data.Model.Response;
using Data.Model.DTO;

namespace API.Controllers
{
    [ApiController]
    [Route("api/groups")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _groupService.Get<GroupDTO>();

            return ControllerResponse.Response(result);
        }
    }
}