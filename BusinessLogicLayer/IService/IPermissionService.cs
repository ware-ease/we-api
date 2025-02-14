using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.Permission;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IPermissionService
    {
        Task<PageEntity<PermissionDTO>?> GetAllAsync(int? pageIndex, int? pageSize);
        Task<PermissionDTO?> GetByIdAsync(string id);
        Task<PermissionDTO> CreateAsync(CreatePermissionDTO model);
        Task<PermissionDTO?> UpdateAsync(string id, UpdatePermissionDTO model);
        Task<bool> DeleteAsync(string id, string deleteBy);
        Task<PageEntity<PermissionDTO>?> SearchAsync(string? searchKey, int? pageIndex, int? pageSize);

    }
}
