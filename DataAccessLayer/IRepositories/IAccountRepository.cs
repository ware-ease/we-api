using Data.Entity;
using DataAccessLayer.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.IRepositories
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account> GetWithFullInfo(string id);
        // Phương thức truy vấn đồng bộ
        Account GetWithFullInfoNotAsync(string createdById);
        IQueryable<Account> GetWithFullInfo();
        Task<List<string>> GetUserIdsByWarehouseAndGroups(string requestedWarehouseId, List<string> groupNames);
        Task<List<string>> GetAdminUserIdsAsync();
    }
}
