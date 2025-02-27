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
    public class PurchaseDetailRepository : IPurchaseDetailRepository
    {
        private readonly WaseEaseDbContext _context;

        public PurchaseDetailRepository(WaseEaseDbContext context)
        {
            _context = context;
        }

        public IQueryable<PurchaseDetail> GetAllQueryable()
        {
            //return _context.PurchaseDetails
            //               .Include(pd => pd.PurchaseReceipt)
            //               .Include(pd => pd.ProductType)
            //               .AsQueryable();

            return null;
        }

        public async Task<List<PurchaseDetail>> GetAllAsync()
        {
            //return await _context.PurchaseDetails
            //                     .Include(pd => pd.PurchaseReceipt)
            //                     .Include(pd => pd.ProductType)
            //                     .ToListAsync();

            return null;
        }

        public async Task<PurchaseDetail> GetByIdAsync(string id)
        {
            //return await _context.PurchaseDetails
            //                     .Include(pd => pd.PurchaseReceipt)
            //                     .Include(pd => pd.ProductType)
            //                     .FirstOrDefaultAsync(pd => pd.Id == id);

            return null;
        }

        public async Task AddAsync(PurchaseDetail purchaseDetail)
        {
            await _context.PurchaseDetails.AddAsync(purchaseDetail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PurchaseDetail purchaseDetail)
        {
            _context.PurchaseDetails.Update(purchaseDetail);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PurchaseDetail purchaseDetail)
        {
            purchaseDetail.IsDeleted = true;
            purchaseDetail.DeletedTime = DateTime.Now;
            _context.PurchaseDetails.Update(purchaseDetail);
            await _context.SaveChangesAsync();
        }
    }
}
