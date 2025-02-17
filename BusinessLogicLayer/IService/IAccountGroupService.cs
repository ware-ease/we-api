using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.AccountGroup;
using BusinessLogicLayer.Models.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IAccountGroupService
    {
        Task<IEnumerable<AccountGroupDTO>> GetAllAsync();
        Task<AccountGroupDTO> GetByIdAsync(string accountId, string groupId);
        Task<AccountGroupDTO> CreateAsync(CreateAccountGroupDTO model);
        Task<bool> DeleteAsync(string accountId, string groupId);
        Task<IEnumerable<GroupDTO>> GetGroupsByAccountIdAsync(string accountId);
        Task<IEnumerable<AccountDTO>> GetAccountsByGroupIdAsync(string groupId);
    }
}
