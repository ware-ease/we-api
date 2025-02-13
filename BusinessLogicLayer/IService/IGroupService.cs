using BusinessLogicLayer.Models.Group;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IGroupService
    {
        Task<IEnumerable<GroupDTO>> GetAllGroupsAsync();
        Task<GroupDTO?> GetGroupByIdAsync(string id);
        Task<GroupDTO> CreateGroupAsync(CreateGroupDTO groupDto);
        Task<GroupDTO?> UpdateGroupAsync(string id, CreateGroupDTO groupDto);
        Task<bool> DeleteGroupAsync(string id);
    }
}
