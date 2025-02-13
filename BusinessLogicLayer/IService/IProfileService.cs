using BusinessLogicLayer.Models.Group;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IProfileService
    {
        Task<PageEntity<ProfileDTO>?> GetAllAsync(int? pageIndex, int? pageSize);
        Task<ProfileDTO?> GetProfileByIdAsync(string id);
        Task<ProfileDTO> CreateProfileAsync(ProfileCreateDTO model);
        Task<ProfileDTO?> UpdateProfileAsync(string id, ProfileUpdateDTO model);
        Task<bool> DeleteProfileAsync(string id, string deleteBy);
        Task<ProfileDTO?> UpdateProfileByAccountIdAsync(string accountId, ProfileUpdateDTO model);
        Task<PageEntity<ProfileDTO>?> SearchAsync(string? searchKey, int? pageIndex, int? pageSize);

    }
}
