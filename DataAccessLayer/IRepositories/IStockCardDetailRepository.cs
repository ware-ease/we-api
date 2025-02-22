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
        IQueryable<StockCardDetail> GetAllQueryable();
        IQueryable<StockCardDetail> GetQueryableByStockCardId(string stockCardId);
        IQueryable<StockCardDetail> GetQueryableByProductTypeId(string productTypeId);
        Task<List<StockCardDetail>> GetAllAsync();
        Task<StockCardDetail> GetByIdAsync(string stockCardId, string productTypeId);
        Task AddAsync(StockCardDetail stockCardDetail);
        Task UpdateAsync(StockCardDetail stockCardDetail);
        Task DeleteAsync(string stockCardId, string productTypeId);
    }
}
