using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.AccountAction;
using BusinessLogicLayer.Models.AccountGroup;
using BusinessLogicLayer.Models.Authentication;
using BusinessLogicLayer.Models.Pagination;
using Data.Entity;
using Data.Model.DTO;

namespace BusinessLogicLayer.IService
{
    public interface IAccountService
    {
        public  Task<AccountDTO?> CheckLoginAsync(string userName, string password);
        public Task<TokenDTO> GenerateAccessTokenAsync(string id);
        Task<PageEntity<AccountDTO>?> GetAllAccountsAsync(int? pageIndex, int? pageSize);
        Task<AccountDTO?> GetAccountByIdAsync(string id);
        Task<AccountDTO> CreateAccountAsync(AccountCreateDTO model);
        Task<AccountDTO?> UpdateAccountAsync(string id, AccountUpdateDTO model);
        Task<bool> DeleteAccountAsync(string id, string deleteBy);
        Task<AccountDTO> UpdateUsernameAsync(string accountId, string newUsername, string updateId);
        Task<AccountDTO> UpdatePasswordAsync(string accountId, string currentPassword, string newPassword, string updateId);
        Task<PageEntity<AccountDTO>?> SearchAccountAsync(string? searchKey, int? pageIndex, int? pageSize);
        Task<bool> CreateAsync(CreateAccountGroupDTO model);
        Task<bool> CreateAccountActionAsync(CreateAccountActionDTO model);
        Task<bool> DeleteMultipleAccountGroupAsync(List<string> accountIds, List<string> groupIds);
        Task<bool> DeleteMultipleAccountActionsAsync(List<string> accountIds, List<string> actionIds);
    }
}
