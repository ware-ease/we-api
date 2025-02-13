using BusinessLogicLayer.Models.AppAction;
using BusinessLogicLayer.Models.Pagination;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IAppActionService
    {
        Task<PageEntity<AppActionDTO>?> GetAllAsync(int? pageIndex, int? pageSize);
        Task<AppActionDTO> GetByIdAsync(string id);
        Task<AppActionDTO> CreateAsync(CreateAppActionDTO action);
        Task<AppActionDTO> UpdateAsync(string id, UpdateAppActionDTO action);
        Task<bool> DeleteAsync(string id, string deleteBy);
        Task<PageEntity<AppActionDTO>?> SearchAsync(string? searchKey, int? pageIndex, int? pageSize);
    }
}
