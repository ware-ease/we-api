using BusinessLogicLayer.Models.AppAction;
using BusinessLogicLayer.Models.Permission;
using BusinessLogicLayer.Models.PermissionAction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IPermissionActionService
    {
        Task<IEnumerable<PermissionActionDTO>> GetAllAsync();
        Task<PermissionActionDTO> GetByIdAsync(string permissionId, string actionId);
        Task<PermissionActionDTO> CreateAsync(CreatePermissionActionDTO model);
        Task<bool> DeleteAsync(string permissionId, string actionId);
        Task<IEnumerable<AppActionDTO>> GetActionsByPermissionIdAsync(string permissionId);
        Task<IEnumerable<PermissionDTO>> GetPermissionsByActionIdAsync(string actionId);

    }
}
