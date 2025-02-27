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
    public class ProductTypeRepository : IProductTypeRepository
    {
        private readonly WaseEaseDbContext _context;

        public ProductTypeRepository(WaseEaseDbContext context)
        {
            _context = context;
        }

        public IQueryable<ProductType> GetAllQueryable()
        {
            //return _context.ProductTypes
            //               .Include(pt => pt.Product)
            //               /*                           .Include(pt => pt.PurchaseDetails)
            //                                          .Include(pt => pt.ReceivingDetails)
            //                                          .Include(pt => pt.SaleDetails)
            //                                          .Include(pt => pt.StockCardDetails)
            //                                          .Include(pt => pt.IssueDetails)*/
            //               .AsQueryable();

            return null;
        }

        public async Task<ProductType> GetByIdAsync(string id)
        {
            //return await _context.ProductTypes
            //                     .Include(pt => pt.Product)
            //                     .FirstOrDefaultAsync(pt => pt.Id == id);

            return null;
        }

        public async Task<List<ProductType>> GetAllAsync()
        {
            //return await _context.ProductTypes
            //                     .Include(pt => pt.Product) // Bao gồm Product nếu cần
            //                     .ToListAsync();

            return null;
        }


        public async Task AddAsync(ProductType productType)
        {
            //await _context.ProductTypes.AddAsync(productType);
            //await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(ProductType productType)
        {
            //_context.ProductTypes.Update(productType);
            //await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(string id)
        {
            //var productType = await _context.ProductTypes
            //                                .FirstOrDefaultAsync(pt => pt.Id == id);

            //_context.ProductTypes.Remove(productType);
            //await _context.SaveChangesAsync();
        }
    }
}
