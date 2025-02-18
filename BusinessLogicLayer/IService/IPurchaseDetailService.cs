using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.PurchaseDetail;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IPurchaseDetailService
    {
        Task<PagedResult<PurchaseDetail>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<PurchaseDetail> GetByIdAsync(string id);
        Task<PurchaseDetail> AddAsync(string receiptId, string productTypeId, CreatePurchaseDetailDTO createPurchaseDetailDTO);
        Task<PurchaseDetail> UpdateAsync(string purchaseDetailId, UpdatePurchaseDetailDTO updatePurchaseDetailDTO);
        Task DeleteAsync(string Id, string deletedBy);
    }
}
