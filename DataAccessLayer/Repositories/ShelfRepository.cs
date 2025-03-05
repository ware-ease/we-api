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
    public class ShelfRepository : GenericRepository<Shelf>, IShelfRepository
    {
        public ShelfRepository(WaseEaseDbContext context) : base(context)
        {
        }

        public IQueryable<Shelf> GetAllQueryable()
        {
            //return _context.Shelves
            //    .Include(s => s.Floors)
            //    .Include(s => s.Warehouse)
            //    .AsQueryable();

            return null;
        }

        public IQueryable<Shelf> GetShelvesByWarehouseIdQueryable(string warehouseId)
        {
            //return _context.Shelves
            //    .Include(s => s.Floors)
            //    .Include(s => s.Warehouse)
            //    .Where(s => s.WarehouseId == warehouseId)
            //    .AsQueryable();

            return null;
        }

        public async Task<List<Shelf>> GetAllAsync()
        {
            //return await _context.Shelves
            //    .Include(s => s.Floors)
            //    .Include(s => s.Warehouse)
            //    .ToListAsync();

            return null;
        }

        public async Task<Shelf> GetByIdAsync(string id)
        {
            //return await _context.Shelves
            //    .Include(s => s.Floors)
            //    .Include(s => s.Warehouse)
            //    .FirstOrDefaultAsync(s => s.Id == id);

            return null;
        }

        public async Task AddAsync(Shelf shelf)
        {
            await _context.Shelves.AddAsync(shelf);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Shelf shelf)
        {
            _context.Shelves.Update(shelf);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Shelf shelf)
        {
            shelf.IsDeleted = true;
            shelf.DeletedTime = DateTime.Now;
            _context.Shelves.Update(shelf);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetActiveShelfCountByWarehouseIdAsync(string warehouseId)
        {
            //return await _context.Shelves
            //    .Where(s => !s.IsDeleted && s.WarehouseId == warehouseId)
            //    .CountAsync();

            return 0;
        }

        public async Task<Shelf> GetShelfByWarehouseIdAndNumberAsync(string warehouseId, int number)
        {
            //return await _context.Shelves
            //    .Include(s => s.Floors)
            //    .Include(s => s.Warehouse)
            //    .FirstOrDefaultAsync(s => s.WarehouseId == warehouseId && s.Number == number && !s.IsDeleted);

            return null;
        }
    }
}
