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
        IQueryable<CellBatch> GetAllQueryable();
        Task<List<CellBatch>> GetAllAsync();
        Task<CellBatch> GetByIdAsync(string id);
        Task AddAsync(CellBatch stockCard);
        Task UpdateAsync(CellBatch stockCard);
        Task DeleteAsync(CellBatch stockCard);
        Task<List<CellBatch>> GetByCellIdAsync(string cellId);
        IQueryable<CellBatch> GetQueryableByCellId(string cellId);
    }
}
