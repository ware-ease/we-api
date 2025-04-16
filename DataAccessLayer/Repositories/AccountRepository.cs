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

        public async Task<List<string>> GetUserIdsByWarehouseAndGroups(string warehouseId, List<string> groupNames)
        {
            var userIds = await _context.AccountWarehouses
                .Where(aw => aw.WarehouseId == warehouseId && aw.IsDeleted == false)
                .SelectMany(aw => _context.AccountGroups
                    .Where(ag => ag.AccountId == aw.AccountId && ag.IsDeleted == false)
                    .Join(_context.Groups,
                          ag => ag.GroupId,
                          g => g.Id,
                          (ag, g) => new { ag.AccountId, g.Name }))
                .Where(x => groupNames.Contains(x.Name))
                .Select(x => x.AccountId)
                .Distinct()
                .ToListAsync();

            return userIds;
        }
        public async Task<List<string>> GetAdminUserIdsAsync()
        {
            var userIds = await _context.AccountGroups
                .Where(ag => ag.IsDeleted == false)
                .Join(_context.Groups,
                      ag => ag.GroupId,
                      g => g.Id,
                      (ag, g) => new { ag.AccountId, g.Name })
                .Where(x => x.Name == "Admin")
                .Select(x => x.AccountId)
                .Distinct()
                .ToListAsync();

            return userIds;
        }

    }
}
