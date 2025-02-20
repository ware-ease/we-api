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
    public class CellRepository : ICellRepository
    {
        private readonly WaseEaseDbContext _context;

        public CellRepository(WaseEaseDbContext context)
        {
            _context = context;
        }

        public IQueryable<Cell> GetAllQueryable()
        {
            return _context.Cells.Include(c => c.Floor).AsQueryable();
        }

        public IQueryable<Cell> GetCellsByFloorId(string floorId)
        {
            return _context.Cells
                .Where(c => c.FloorId == floorId && !c.IsDeleted)
                .AsQueryable();
        }

        public async Task<List<Cell>> GetAllAsync()
        {
            return await _context.Cells.Include(c => c.Floor).ToListAsync();
        }

        public async Task<Cell> GetByIdAsync(string id)
        {
            return await _context.Cells
                .Include(c => c.Floor)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Cell cell)
        {
            await _context.Cells.AddAsync(cell);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cell cell)
        {
            _context.Cells.Update(cell);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Cell cell)
        {
            cell.IsDeleted = true;
            cell.DeletedTime = DateTime.Now;
            _context.Cells.Update(cell);
            await _context.SaveChangesAsync();
        }


        public async Task<Cell> GetCellByFloorIdAndNumberAsync(string floorId, int number)
        {
            return await _context.Cells
                .Where(c => c.FloorId == floorId && c.Number == number && !c.IsDeleted)
                .FirstOrDefaultAsync();
        }

    }
}
