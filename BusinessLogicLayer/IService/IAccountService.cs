using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.Authentication;
using Data.Entity;

namespace BusinessLogicLayer.IService
{
    public interface IAccountService
    {
        public  Task<AccountDTO?> CheckLoginAsync(string userName, string password);
        public Task<TokenDTO> GenerateAccessTokenAsync(string id);
        public Task<IEnumerable<AccountDTO?>> GetAccountsAsync();
        Task<AccountDTO?> GetAccountByIdAsync(string id);
        Task<AccountDTO> CreateAccountAsync(AccountCreateDTO model);
        Task<AccountDTO?> UpdateAccountAsync(string id, AccountUpdateDTO model);
        Task<bool> DeleteAccountAsync(string id);
    }
}
