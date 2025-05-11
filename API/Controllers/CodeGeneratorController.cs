using BusinessLogicLayer.IServices;
using Data.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/code-generator")]
    [ApiController]
    public class CodeGeneratorController : ControllerBase
    {
        public readonly ICodeGeneratorService _codeGeneratorService;
        public CodeGeneratorController(ICodeGeneratorService codeGeneratorService)
        {
            _codeGeneratorService = codeGeneratorService;
        }

        [HttpGet("generate")]
        public async Task<IActionResult> GenerateCode([FromQuery] CodeType codeType)
        {
            try
            {
                var newCode = await _codeGeneratorService.GenerateCodeAsync(codeType);
                return Ok(new { codeType = codeType.ToString(), newCode });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("generate-batch-code")]
        public async Task<IActionResult> GenerateBatchCodeByProduct([FromQuery] string productId)
        {
            try
            {
                var newCode = await _codeGeneratorService.GenerateBatchCodeByProductAsync(productId);
                return Ok(new { productId, newCode });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}