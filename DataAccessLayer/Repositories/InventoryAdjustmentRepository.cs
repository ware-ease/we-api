using Data.Entity;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class InventoryAdjustmentRepository : GenericRepository<InventoryAdjustment>, IInventoryAdjustmentRepository
    {
        public InventoryAdjustmentRepository(WaseEaseDbContext context) : base(context)
        {
        }
        public IQueryable<InventoryAdjustment> GetQueryable()
        {
            return _context.InventoryAdjustments.AsQueryable();
        }

    }
}
