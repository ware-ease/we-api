using BusinessLogicLayer.Models.AppAction;
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
        Task<IEnumerable<AppActionDTO>> GetAllAsync();
        Task<AppActionDTO> GetByIdAsync(string id);
        Task<AppActionDTO> CreateAsync(CreateAppActionDTO action);
        Task<AppActionDTO> UpdateAsync(string id, CreateAppActionDTO action);
        Task<bool> DeleteAsync(string id);
    }
}
