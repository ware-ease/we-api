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
        Task<IEnumerable<Group>> GetAllGroupsAsync();
        Task<Group> GetGroupByIdAsync(string id);
        Task<Group> CreateGroupAsync(Group group);
        Task<Group> UpdateGroupAsync(string id, Group group);
        Task<bool> DeleteGroupAsync(string id);
    }
}
