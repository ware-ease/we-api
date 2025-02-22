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
        Task<PagedResult<StockCard>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<PagedResult<StockCard>> GetAllStockCardByCellId(string cellId, int? pageNumber, int? pageSize);
        Task<StockCard> GetByIdAsync(string id);
        Task<StockCard> AddAsync(string cellId, CreateStockCardDTO createStockCardDTO);
        Task<StockCard> UpdateAsync(string stockCardId, UpdateStockCardDTO updateStockCardDTO);
        Task DeleteAsync(string Id, string deletedBy);
    }
}
