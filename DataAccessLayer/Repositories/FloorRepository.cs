using Data.Entity;
using DataAccessLayer.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class FloorRepository : IFloorRepository
    {
        private readonly WaseEaseDbContext _context;

        public FloorRepository(WaseEaseDbContext context)
        {
            _context = context;
        }

        public IQueryable<Floor> GetAllQueryable()
        {
            return _context.Floors.Include(f => f.Shelf).AsQueryable();
        }

        public IQueryable<Floor> GetFloorsByShelfIdQueryable(string shelfId)
        {
            return _context.Floors
                .Include(f => f.Shelf)
                .Where(f => f.ShelfId == shelfId)
                .AsQueryable();
        }

        public async Task<List<Floor>> GetAllAsync()
        {
            return await _context.Floors.Include(f => f.Shelf).ToListAsync();
        }

        public async Task<Floor> GetByIdAsync(string id)
        {
            return await _context.Floors.Include(f => f.Shelf).FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddAsync(Floor floor)
        {
            await _context.Floors.AddAsync(floor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Floor floor)
        {
            _context.Floors.Update(floor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Floor floor)
        {
            floor.IsDeleted = true;
            floor.DeletedTime = DateTime.Now;
            _context.Floors.Update(floor);
            await _context.SaveChangesAsync();
        }

        public async Task<Floor> GetFloorByShelfIdAndNumberAsync(string shelfId, int number)
        {
            return await _context.Floors
                .Where(f => f.ShelfId == shelfId && f.Number == number && !f.IsDeleted)
                .FirstOrDefaultAsync();
        }
    }
}
