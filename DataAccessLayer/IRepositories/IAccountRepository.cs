using Data.Entity;

namespace DataAccessLayer.IRepositories
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAccountsAsync();
        //Task<AppUser?> CheckLoginAsync(String userName, String password);
        //Task<Token?> GenerateAccessTokenAsync(int id);
    }
}
