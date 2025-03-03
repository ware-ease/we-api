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

        public IQueryable<InOutDetail> GetAllQueryable()
        {
            //return _context.StockCardDetails.Include(scd => scd.StockCard).Include(scd => scd.ProductType).AsQueryable();

            return null;
        }

        public IQueryable<InOutDetail> GetQueryableByStockCardId(string stockCardId)
        {
            return _context.StockCardDetails
                .Where(scd => scd.CellBatchId == stockCardId)
                .AsQueryable();
        }

        public IQueryable<InOutDetail> GetQueryableByProductTypeId(string productTypeId)
        {
            //return _context.StockCardDetails
            //    .Where(scd => scd.ProductTypeId == productTypeId)
            //    .AsQueryable();

            return null;
        }

        public async Task<List<InOutDetail>> GetAllAsync()
        {
            //return await _context.StockCardDetails.Include(scd => scd.StockCard).Include(scd => scd.ProductType).ToListAsync();

            return null;
        }

        public async Task<InOutDetail> GetByIdAsync(string stockCardId, string productTypeId)
        {
            //return await _context.StockCardDetails.Include(scd => scd.StockCard)
            //    .Include(scd => scd.ProductType)
            //    .FirstOrDefaultAsync(scd => scd.StockCardId == stockCardId && scd.ProductTypeId == productTypeId);

            return null;
        }

        public async Task AddAsync(InOutDetail stockCardDetail)
        {
            await _context.StockCardDetails.AddAsync(stockCardDetail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(InOutDetail stockCardDetail)
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
