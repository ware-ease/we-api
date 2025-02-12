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
        Task<IEnumerable<ProfileDTO>> GetProfilesAsync();
        Task<ProfileDTO?> GetProfileByIdAsync(string id);
        Task<ProfileDTO> CreateProfileAsync(ProfileCreateDTO model);
        Task<ProfileDTO?> UpdateProfileAsync(string id, ProfileUpdateDTO model);
        Task<bool> DeleteProfileAsync(string id);
    }
}
