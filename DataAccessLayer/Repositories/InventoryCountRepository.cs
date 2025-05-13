using Data.Entity;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class InventoryCountRepository : GenericRepository<InventoryCount>, IInventoryCountRepository
    {
        public InventoryCountRepository(WaseEaseDbContext context) : base(context)
        {
        }
        public IQueryable<InventoryCount> GetQueryable()
        {
            return _context.InventoryChecks.AsQueryable();
        }
    }
}
