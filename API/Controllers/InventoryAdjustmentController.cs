using API.Utils;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Services;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.InventoryAdjustment;
using Data.Model.Request.Product;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/inventory-adjustments")]
    [ApiController]
    public class InventoryAdjustmentController : ControllerBase
    {
        private readonly IInventoryAdjustmentService _inventoryAdjustmentService;

        public InventoryAdjustmentController(IInventoryAdjustmentService inventoryAdjustmentService)
        {
            _inventoryAdjustmentService = inventoryAdjustmentService;
        }

        /*[HttpGet]
        public async Task<IActionResult> SearchPartners([FromQuery] int pageIndex = 1,
                                                        [FromQuery] int pageSize = 5,
                                                        [FromQuery] string? keyword = null, string? warehouseId = null)
        {
            var response = await _inventoryAdjustmentService.Search<InventoryAdjustmentDTO>(
                pageIndex, pageSize, keyword, warehouseId);

            return ControllerResponse.Response(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var response = await _inventoryAdjustmentService.Get<InventoryAdjustmentDTO>(id);
                return ControllerResponse.Response(response);
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
        public async Task<IActionResult> Add([FromBody] InventoryAdjustmentCreateDTO request)
        {
            try
            {
                var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

                if (authUser != null)
                {
                    request.CreatedBy = authUser.id;
                    request.InventoryAdjustmentDetails.ForEach(x => x.CreatedBy = authUser.id);
                }

                var respones = await _inventoryAdjustmentService.AddInventoryAdjustment(request);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "InventoryAdjustment created successfully",
                    Data = respones
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
        public async Task<IActionResult> Update(string id, [FromBody] InventoryAdjustmentUpdateDTO request)
        {
            try
            {
                request.Id = id;
                var updatedInventoryAdjustment = await _inventoryAdjustmentService.UpdateInventoryAdjustment(request);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "InventoryAdjustment updated successfully",
                    Data = updatedInventoryAdjustment
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
            var result = await _inventoryAdjustmentService.Delete(id);
            return ControllerResponse.Response(result);
        }*/
    }
}
