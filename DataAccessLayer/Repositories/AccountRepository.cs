using Data.Entity;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections;

namespace DataAccessLayer.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(WaseEaseDbContext context) : base(context)
        {
        }

        public async Task<Account> GetWithFullInfo(string id)
        {
            var query = _dbSet.AsQueryable().Where(e => e.IsDeleted == false)
                .Include(a => a.Profile)
                .Include(a => a.AccountWarehouses).ThenInclude(aw => aw.Warehouse)
                .Include(a => a.AccountPermissions).ThenInclude(ap => ap.Permission)
                .Include(a => a.AccountGroups).ThenInclude(ag => ag.Group).ThenInclude(g => g.GroupPermissions).ThenInclude(gp => gp.Permission).ThenInclude(p => p.Route)
                .Include(a => a.AccountNotifications);

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<Account> GetWithFullInfo()
        {
            var query = _dbSet.AsQueryable().Where(e => e.IsDeleted == false)
                .Include(a => a.Profile)
                .Include(a => a.AccountWarehouses).ThenInclude(aw => aw.Warehouse)
                .Include(a => a.AccountPermissions).ThenInclude(ap => ap.Permission)
                .Include(a => a.AccountGroups).ThenInclude(ag => ag.Group).ThenInclude(g => g.GroupPermissions).ThenInclude(gp => gp.Permission).ThenInclude(p => p.Route)
                .Include(a => a.AccountNotifications);

            return query;
        }
    }
}
