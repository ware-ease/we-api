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
    public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(WaseEaseDbContext context) : base(context)
        {
        }
        //public async Task<List<Inventory>> GetInventoriesByProductIdAsync(string productId)
        //{
        //    return await _context.Inventories
        //        .Where(x => x.Batch.ProductId == productId && x.CurrentQuantity > 0)
        //        .ToListAsync();
        //}

        public async Task<List<Inventory>> GetAvailableInventoriesAsync(string productId)
        {
            return await _context.Inventories
                .Where(x => x.Batch.ProductId == productId && x.CurrentQuantity > 0)
                .OrderBy(x => x.Batch.InboundDate)  // Sắp xếp theo ngày nhập kho
                .ToListAsync();
        }
    }
}