using Data.Entity;
using DataAccessLayer.Generic;

namespace DataAccessLayer.IRepositories
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<IEnumerable<Account>> GetAccountsAsync();
        Task<Account?> CheckLoginAsync(string userName, string password);
        Task<Token?> GenerateAccessTokenAsync(string id);
    }
}
