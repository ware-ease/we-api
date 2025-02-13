using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.ReceivingNote;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IReceivingNoteService
    {
        Task<PagedResult<ReceivingNote>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<ReceivingNote> GetByIdAsync(string id);
        Task<ReceivingNote> AddAsync(string supplierId, string purchaseId, CreateReceivingNoteDTO createReceivingNoteDTO);
        Task<ReceivingNote> UpdateAsync(string id, UpdateReceivingNoteDTO updateReceivingNoteDTO);
        Task DeleteAsync(string Id, string deletedBy);
    }
}
