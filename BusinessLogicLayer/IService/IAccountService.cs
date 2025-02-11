using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.Authentication;
using Data.Entity;

namespace BusinessLogicLayer.IService
{
    public interface IAccountService
    {
        public  Task<IEnumerable<Account>> GetAccountsAsync();

        public  Task<AccountDTO?> CheckLoginAsync(string userName, string password);
        public Task<TokenDTO> GenerateAccessTokenAsync(string id);
        //public Task<AppUserDTO> BanAccount(int id);
        //public Task<bool> checkCorrectPassword(int id, string password);
    }
}
