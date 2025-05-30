﻿using API.Middlewares;
using API.Utils;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Services;
using Data.Model.DTO;
using Data.Model.Request.Brand;
using Data.Model.Request.Product;
using Data.Model.Request.Unit;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [NonAction]
        public async Task<IActionResult> Get()
        {
            var result = await _unitService.Get<UnitDTO>();
            return ControllerResponse.Response(result);
        }
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] int pageIndex = 1,
                                                        [FromQuery] int pageSize = 5,
                                                        [FromQuery] string? keyword = null)
        {
            var response = await _unitService.Search<UnitDTO>(
                pageIndex, pageSize, keyword);

            return ControllerResponse.Response(response);
        }
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _unitService.Get<UnitDTO>(id);
            return ControllerResponse.Response(result);
        }

        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] UnitCreateDTO request)
        {
            var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

            if (authUser != null)
            {
                request.CreatedBy = authUser.id;
            }

            var result = await _unitService.AddUnit(request);
            return ControllerResponse.Response(new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Đơn vị đã tạo thành công",
                Data = result
            });
        }
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UnitUpdateDTO request)
        {
            try
            {
                request.Id = id;
                var updatedUnit = await _unitService.UpdateUnit(request);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Đơn vị đã sửa thành công",
                    Data = updatedUnit
                });
            }
            catch (Exception ex)
            {
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Error,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
        [Authorize]
        [AuthorizeGroup("Admin,Thủ kho")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _unitService.Delete(id);
            return ControllerResponse.Response(result);
        }
    }
}
