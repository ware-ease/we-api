using BusinessLogicLayer.Models.AppAction;
using BusinessLogicLayer.Models.Group;
using BusinessLogicLayer.Models.Pagination;
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
        Task<PageEntity<GroupDTO>?> GetAllAsync(int? pageIndex, int? pageSize);
        Task<GroupDTO?> GetGroupByIdAsync(string id);
        Task<GroupDTO> CreateGroupAsync(CreateGroupDTO groupDto);
        Task<GroupDTO?> UpdateGroupAsync(string id, UpdateGroupDTO groupDto);
        Task<bool> DeleteGroupAsync(string id, string deleteBy);
        Task<PageEntity<GroupDTO>?> SearchAsync(string? searchKey, int? pageIndex, int? pageSize);

    }
}
