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
    public class StockCardDetailRepository : IStockCardDetailRepository
    {
        private readonly WaseEaseDbContext _context;

        public StockCardDetailRepository(WaseEaseDbContext context)
        {
            _context = context;
        }

        public IQueryable<StockCardDetail> GetAllQueryable()
        {
            return _context.StockCardDetails.Include(scd => scd.StockCard).Include(scd => scd.ProductType).AsQueryable();
        }

        public IQueryable<StockCardDetail> GetQueryableByStockCardId(string stockCardId)
        {
            return _context.StockCardDetails
                .Where(scd => scd.StockCardId == stockCardId)
                .AsQueryable();
        }

        public IQueryable<StockCardDetail> GetQueryableByProductTypeId(string productTypeId)
        {
            return _context.StockCardDetails
                .Where(scd => scd.ProductTypeId == productTypeId)
                .AsQueryable();
        }

        public async Task<List<StockCardDetail>> GetAllAsync()
        {
            return await _context.StockCardDetails.Include(scd => scd.StockCard).Include(scd => scd.ProductType).ToListAsync();
        }

        public async Task<StockCardDetail> GetByIdAsync(string stockCardId, string productTypeId)
        {
            return await _context.StockCardDetails.Include(scd => scd.StockCard)
                .Include(scd => scd.ProductType)
                .FirstOrDefaultAsync(scd => scd.StockCardId == stockCardId && scd.ProductTypeId == productTypeId);
        }

        public async Task AddAsync(StockCardDetail stockCardDetail)
        {
            await _context.StockCardDetails.AddAsync(stockCardDetail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StockCardDetail stockCardDetail)
        {
            _context.StockCardDetails.Update(stockCardDetail);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string stockCardId, string productTypeId)
        {
            var stockCardDetail = await GetByIdAsync(stockCardId, productTypeId);
            if (stockCardDetail != null)
            {
                _context.StockCardDetails.Remove(stockCardDetail);
                await _context.SaveChangesAsync();
            }
        }
    }
}
