using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface IStockCardRepository
    {
        IQueryable<StockCard> GetAllQueryable();
        Task<List<StockCard>> GetAllAsync();
        Task<StockCard> GetByIdAsync(string id);
        Task AddAsync(StockCard stockCard);
        Task UpdateAsync(StockCard stockCard);
        Task DeleteAsync(StockCard stockCard);
        Task<List<StockCard>> GetByCellIdAsync(string cellId);
        IQueryable<StockCard> GetQueryableByCellId(string cellId);
    }
}
