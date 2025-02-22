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
    public class StockCardRepository : IStockCardRepository
    {
        private readonly WaseEaseDbContext _context;

        public StockCardRepository(WaseEaseDbContext context)
        {
            _context = context;
        }

        public IQueryable<StockCard> GetAllQueryable()
        {
            return _context.StockCards.Include(sc => sc.Cell).Include(sc => sc.StockCardDetails).AsQueryable();
        }

        public async Task<List<StockCard>> GetAllAsync()
        {
            return await _context.StockCards.Include(sc => sc.Cell).Include(sc => sc.StockCardDetails).ToListAsync();
        }

        public async Task<StockCard> GetByIdAsync(string id)
        {
            return await _context.StockCards.Include(sc => sc.Cell)
                .Include(sc => sc.StockCardDetails)
                .FirstOrDefaultAsync(sc => sc.Id == id);
        }

        public async Task AddAsync(StockCard stockCard)
        {
            await _context.StockCards.AddAsync(stockCard);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StockCard stockCard)
        {
            _context.StockCards.Update(stockCard);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(StockCard stockCard)
        {
            stockCard.IsDeleted = true;
            stockCard.DeletedTime = DateTime.Now;
            _context.StockCards.Update(stockCard);
            await _context.SaveChangesAsync();
        }

        public async Task<List<StockCard>> GetByCellIdAsync(string cellId)
        {
            return await _context.StockCards
                .Where(sc => sc.CellId == cellId && !sc.IsDeleted)
                .ToListAsync();
        }

        public IQueryable<StockCard> GetQueryableByCellId(string cellId)
        {
            return _context.StockCards
                .Where(sc => sc.CellId == cellId)
                .AsQueryable();
        }
    }
}
