using Data.Entity;
using DataAccessLayer.Generic;

namespace DataAccessLayer.IRepositories
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account> GetWithFullInfo(string id);
        IQueryable<Account> GetWithFullInfo();
        Task<List<string>> GetUserIdsByWarehouseAndGroups(string requestedWarehouseId, List<string> groupNames);
        Task<List<string>> GetAdminUserIdsAsync();
    }
}
