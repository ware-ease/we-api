using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Profile;

namespace API.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var profiles = await _profileService.GetAllAsync(1,10);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy danh sách profile thành công",
                    Data = profiles,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi khi lấy danh sách profile",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpGet("{id}", Name ="GetProfileById")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(id);
                if (profile == null)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Profile không tồn tại",
                        IsSuccess = false
                    });
                }

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy dữ liệu thành công",
                    Data = profile,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi khi lấy profile",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ProfileCreateDTO model)
        {
            try
            {
                var profile = await _profileService.CreateProfileAsync(model);
                return Created("GetProfileById", new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Tạo profile thành công",
                    Data = profile,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi khi tạo profile",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ProfileUpdateDTO model)
        {
            try
            {
                var updatedProfile = await _profileService.UpdateProfileAsync(id, model);
                if (updatedProfile == null)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Profile không tồn tại",
                        IsSuccess = false
                    });
                }

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Cập nhật profile thành công",
                    Data = updatedProfile,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi khi cập nhật profile",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id, [FromBody] DeleteProfileDTO model)
        {
            try
            {
                var deleted = await _profileService.DeleteProfileAsync(id, model.DeletedBy!);
                return deleted ? Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Xóa profile thành công",
                    IsSuccess = true
                })
                : NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Profile không tồn tại",
                    IsSuccess = false
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi khi xóa profile",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpPut("account/{accountId}")]
        public async Task<IActionResult> UpdateByAccountIdAsync(string accountId, [FromBody] ProfileUpdateDTO model)
        {
            try
            {
                var updatedProfile = await _profileService.UpdateProfileByAccountIdAsync(accountId, model);
                if (updatedProfile == null)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Profile không tồn tại cho tài khoản này",
                        IsSuccess = false
                    });
                }

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Cập nhật profile thành công",
                    Data = updatedProfile,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi khi cập nhật profile",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProfilesAsync([FromQuery] string? searchKey,
                                                             [FromQuery] int pageNumber = 1,
                                                             [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _profileService.SearchAsync(searchKey, pageNumber, pageSize);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Tìm kiếm profile thành công",
                    Data = result,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi khi tìm kiếm profile",
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }

    }
}
