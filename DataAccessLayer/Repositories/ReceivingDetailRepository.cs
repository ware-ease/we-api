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
    public class ReceivingDetailRepository : IReceivingDetailRepository
    {
        private readonly WaseEaseDbContext _context;

        public ReceivingDetailRepository(WaseEaseDbContext context)
        {
            _context = context;
        }

        // Get all ReceivingDetails as IQueryable for flexibility
        public IQueryable<ReceivingDetail> GetAllQueryable()
        {
            return _context.ReceiptDetails
                           .Include(rd => rd.receivingNote)
                           .Include(rd => rd.ProductType)
                           .AsQueryable();
        }

        public async Task<List<ReceivingDetail>> GetAllAsync()
        {
            return await _context.ReceiptDetails
                                  .Include(rd => rd.receivingNote)
                                  .Include(rd => rd.ProductType)
                                  .ToListAsync();
        }

        public async Task<ReceivingDetail> GetByIdAsync(string id)
        {
            return await _context.ReceiptDetails
                                  .Include(rd => rd.receivingNote)
                                  .Include(rd => rd.ProductType)
                                  .FirstOrDefaultAsync(rd => rd.Id == id);
        }

        public async Task AddAsync(ReceivingDetail receivingDetail)
        {
            await _context.ReceiptDetails.AddAsync(receivingDetail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ReceivingDetail receivingDetail)
        {
            _context.ReceiptDetails.Update(receivingDetail);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ReceivingDetail receivingDetail)
        {
            receivingDetail.IsDeleted = true;
            receivingDetail.DeletedTime = DateTime.Now;
            _context.ReceiptDetails.Update(receivingDetail);
            await _context.SaveChangesAsync();
        }
    }
}
