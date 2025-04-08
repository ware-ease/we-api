using API.Utils;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Services;
using Data.Entity;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.InventoryCount;
using Data.Model.Request.Product;
using Data.Model.Request.Schedule;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/inventory-counts")]
    [ApiController]
    public class InventoryCountController : ControllerBase
    {
        private readonly IInventoryCountService _inventoryCountService;

        public InventoryCountController(IInventoryCountService inventoryCountService)
        {
            _inventoryCountService = inventoryCountService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchPartners([FromQuery] int pageIndex = 1,
                                                        [FromQuery] int pageSize = 5,
                                                        [FromQuery] string? keyword = null,
                                                        [FromQuery] InventoryCountStatus? status = 0,
                                                        [FromQuery] string? warehouseId = null)
        {
            var response = await _inventoryCountService.Search<InventoryCountDTO>(
                pageIndex, pageSize, keyword, status, warehouseId);

            return ControllerResponse.Response(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var inventoryCount = await _inventoryCountService.Get<InventoryCountDTO>(id);
                return ControllerResponse.Response(inventoryCount);
            }
            catch (Exception ex)
            {
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("location-level0")]
        public async Task<IActionResult> GetByLocation(string id)
        {
            try
            {
                var response = await _inventoryCountService.GetInventoriesByLocationLevel0Async(id);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = " successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] InventoryCountCreateDTO request)
        {
            try
            {
                var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

                if (authUser != null)
                {
                    request.CreatedBy = authUser.id;
                }

                var inventoryCount = await _inventoryCountService.AddInventoryCount(request);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Product created successfully",
                    Data = inventoryCount
                });
            }
            catch (Exception ex)
            {
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] InventoryCountUpdateDTO request)
        {
            try
            {
                request.Id = id;
                var updated = await _inventoryCountService.UpdateInventoryCount(request);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "InventoryCount updated successfully",
                    Data = updated
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _inventoryCountService.Delete(id);
            return ControllerResponse.Response(result);
        }
    }
}
