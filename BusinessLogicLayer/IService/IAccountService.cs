using Data.Entity;

namespace BusinessLogicLayer.IService
{
    public interface IAccountService
    {
        public  Task<IEnumerable<Account>> GetAccountsAsync();

        //public  Task<AppUserDTO?> CheckLoginAsync(string userName, string password);
        //public Task<TokenDto> GenerateAccessTokenAsync(int id);
        //public Task<AppUserDTO> BanAccount(int id);
        //public Task<bool> checkCorrectPassword(int id, string password);
    }
}
