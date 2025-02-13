using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface IPurchaseReceiptRepository
    {
        IQueryable<PurchaseReceipt> GetAllQueryable();
        Task<List<PurchaseReceipt>> GetAllAsync();
        Task<PurchaseReceipt> GetByIdAsync(string id);
        Task AddAsync(PurchaseReceipt purchaseReceipt);
        Task UpdateAsync(PurchaseReceipt purchaseReceipt);
        Task DeleteAsync(PurchaseReceipt purchaseReceipt);
        Task UpdateAsync(ReceivingNote note);
    }
}
