using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface IStockCardDetailRepository
    {
        IQueryable<InOutDetail> GetAllQueryable();
        IQueryable<InOutDetail> GetQueryableByStockCardId(string stockCardId);
        IQueryable<InOutDetail> GetQueryableByProductTypeId(string productTypeId);
        Task<List<InOutDetail>> GetAllAsync();
        Task<InOutDetail> GetByIdAsync(string stockCardId, string productTypeId);
        Task AddAsync(InOutDetail stockCardDetail);
        Task UpdateAsync(InOutDetail stockCardDetail);
        Task DeleteAsync(string stockCardId, string productTypeId);
    }
}
