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
    public class PurchaseReceiptRepository : IPurchaseReceiptRepository
    {
        private readonly WaseEaseDbContext _context;

        public PurchaseReceiptRepository(WaseEaseDbContext context)
        {
            _context = context;
        }

        public IQueryable<PurchaseReceipt> GetAllQueryable()
        {
            return _context.PurchaseReceipts.Include(pr => pr.Supplier).Include(pr => pr.ReceivingNotes).AsQueryable();
        }

        public async Task<List<PurchaseReceipt>> GetAllAsync()
        {
            return await _context.PurchaseReceipts.Include(pr => pr.Supplier).Include(pr => pr.ReceivingNotes).ToListAsync();
        }

        public async Task<PurchaseReceipt> GetByIdAsync(string id)
        {
            return await _context.PurchaseReceipts.Include(pr => pr.Supplier)
                .Include(pr => pr.ReceivingNotes)
                .FirstOrDefaultAsync(pr => pr.Id == id);
        }

        public async Task AddAsync(PurchaseReceipt purchaseReceipt)
        {
            await _context.PurchaseReceipts.AddAsync(purchaseReceipt);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PurchaseReceipt purchaseReceipt)
        {
            _context.PurchaseReceipts.Update(purchaseReceipt);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PurchaseReceipt purchaseReceipt)
        {
            purchaseReceipt.IsDeleted = true;
            purchaseReceipt.DeletedTime = DateTime.Now;
            _context.PurchaseReceipts.Update(purchaseReceipt);
            await _context.SaveChangesAsync();
        }

        public Task UpdateAsync(ReceivingNote note)
        {
            throw new NotImplementedException();
        }
    }
}
