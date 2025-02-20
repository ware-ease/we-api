using BusinessLogicLayer.Models.Group;
using BusinessLogicLayer.Models.GroupPermission;
using BusinessLogicLayer.Models.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IGroupPermissionService
    {
        Task<IEnumerable<GroupPermissionDTO>> GetAllAsync();
        Task<GroupPermissionDTO> GetByIdAsync(string groupId, string permissionId);
        Task<GroupPermissionDTO> CreateAsync(CreateGroupPermissionDTO model);
        Task<bool> DeleteAsync(string groupId, string permissionId);
        Task<IEnumerable<PermissionDTO>> GetPermissionsByGroupIdAsync(string groupId);
        Task<IEnumerable<GroupDTO>> GetGroupsByPermissionIdAsync(string permissionId);
    }
}
