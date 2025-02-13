using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface IReceivingNoteRepository
    {
        IQueryable<ReceivingNote> GetAllQueryable();
        Task<ReceivingNote> GetByIdAsync(string id);
        Task AddAsync(ReceivingNote receivingNote);
        Task UpdateAsync(ReceivingNote receivingNote);
        Task DeleteAsync(ReceivingNote receivingNote);
    }
}
