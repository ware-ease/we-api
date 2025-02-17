using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.AccountPermission;
using BusinessLogicLayer.Models.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IAccountPermissionService
    {
        Task<IEnumerable<AccountPermissionDTO>> GetAllAsync();
        Task<AccountPermissionDTO> GetByIdAsync(string accountId, string permissionId);
        Task<AccountPermissionDTO> CreateAsync(CreateAccountPermissionDTO model);
        Task<bool> DeleteAsync(string accountId, string permissionId);
        Task<IEnumerable<PermissionDTO>> GetPermissionsByAccountIdAsync(string accountId);
        Task<IEnumerable<AccountDTO>> GetAccountsByPermissionIdAsync(string permissionId);
    }
}
