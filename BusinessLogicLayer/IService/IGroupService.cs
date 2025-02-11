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
        Task<Group> GetGroupByIdAsync(int id);
        Task<Group> CreateGroupAsync(Group group);
        Task<Group> UpdateGroupAsync(int id, Group group);
        Task<bool> DeleteGroupAsync(int id);
    }
}
