using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.StockCard;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IStockCardService
    {
        Task<PagedResult<CellBatch>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<PagedResult<CellBatch>> GetAllStockCardByCellId(string cellId, int? pageNumber, int? pageSize);
        Task<CellBatch> GetByIdAsync(string id);
        Task<CellBatch> AddAsync(string cellId, CreateStockCardDTO createStockCardDTO);
        Task<CellBatch> UpdateAsync(string stockCardId, UpdateStockCardDTO updateStockCardDTO);
        Task DeleteAsync(string Id, string deletedBy);
    }
}
