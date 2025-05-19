using API.Utils;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Services;
using Data.Entity;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.Batch;
using Data.Model.Request.Product;
using Data.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/batches")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly IBatchService _batchService;

        public BatchController(IBatchService batchService)
        {
            _batchService = batchService;
        }

        [Authorize]
        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _batchService.CountBatch();
            var response = new ServiceResponse
            {
                Data = count,
                Status = SRStatus.Success,
                Message = "Total batch count retrieved successfully"
            };

            return ControllerResponse.Response(response);
        }
        [Authorize]
        [NonAction]
        public async Task<IActionResult> Get()
        {

            var batchs = await _batchService.Get<BatchDTO>();
            return ControllerResponse.Response(batchs);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] int pageIndex = 1,
                                                        [FromQuery] int pageSize = 5,
                                                        [FromQuery] string? keyword = null,
                                                        [FromQuery] string? productId = null)
        {
            var response = await _batchService.Search<BatchDTO>(
                pageIndex, pageSize, keyword, productId);

            return ControllerResponse.Response(response);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var batch = await _batchService.Get<BatchDTO>(id);
                return ControllerResponse.Response(batch);
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
        public async Task<IActionResult> Add([FromBody] BatchCreateDTO request)
        {
            try
            {
                var authUser = AuthHelper.GetCurrentUser(HttpContext.Request);

                if (authUser != null)
                {
                    request.CreatedBy = authUser.id;
                }

                var batch = await _batchService.AddBatch(request);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Batch created successfully",
                    Data = batch
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
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] BatchUpdateDTO request)
        {
            try
            {
                request.Id = id;
                var updatedBatch = await _batchService.UpdateBatch(request);
                return ControllerResponse.Response(new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Product updated successfully",
                    Data = updatedBatch
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _batchService.Delete(id);
            return ControllerResponse.Response(result);
        }
    }
}
