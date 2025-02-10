using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.PurchaseReceipt;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IPurchaseReceiptService
    {
        Task<PagedResult<PurchaseReceipt>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<PurchaseReceipt> GetByIdAsync(string id);
        Task<PurchaseReceipt> AddAsync(string supplierId, CreatePurchaseReceiptDTO createPurchaseReceiptDTO);
        Task<PurchaseReceipt> UpdateAsync(string purchaseReceiptId, UpdatePurchaseReceiptDTO updatePurchaseReceiptDTO);
        Task DeleteAsync(string Id, string deletedBy);
    }
}
