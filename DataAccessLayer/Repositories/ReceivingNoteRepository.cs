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
    public class ReceivingNoteRepository : IReceivingNoteRepository
    {
        private readonly WaseEaseDbContext _context;

        public ReceivingNoteRepository(WaseEaseDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả ReceivingNotes dưới dạng IQueryable
        public IQueryable<ReceivingNote> GetAllQueryable()
        {
            return _context.ReceivingNotes
                .Include(r => r.ReceivingDetails)
                .Include(r => r.Supplier)
                .Include(r => r.PurchaseReceipt)
                .AsQueryable();
        }

        public async Task<ReceivingNote> GetByIdAsync(string id)
        {
            return await _context.ReceivingNotes
                .Include(r => r.ReceivingDetails)
                .Include(r => r.Supplier)
                .Include(r => r.PurchaseReceipt)
                .FirstOrDefaultAsync(r => r.Id == id);
        }


        public async Task AddAsync(ReceivingNote receivingNote)
        {
            await _context.ReceivingNotes.AddAsync(receivingNote);
            await _context.SaveChangesAsync();
        }

        // Cập nhật ReceivingNote
        public async Task UpdateAsync(ReceivingNote receivingNote)
        {
            _context.ReceivingNotes.Update(receivingNote);
            await _context.SaveChangesAsync();
        }

        // Xóa ReceivingNote (chuyển IsDeleted = true)
        public async Task DeleteAsync(ReceivingNote receivingNote)
        {
            receivingNote.IsDeleted = true;
            receivingNote.DeletedTime = DateTime.Now;
            _context.ReceivingNotes.Update(receivingNote);
            await _context.SaveChangesAsync();
        }

    }
}
