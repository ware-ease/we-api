using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface IPurchaseDetailRepository
    {
        IQueryable<PurchaseDetail> GetAllQueryable();
        Task<List<PurchaseDetail>> GetAllAsync();
        Task<PurchaseDetail> GetByIdAsync(string id);
        Task AddAsync(PurchaseDetail purchaseDetail);
        Task UpdateAsync(PurchaseDetail purchaseDetail);
        Task DeleteAsync(PurchaseDetail purchaseDetail);
    }
}
