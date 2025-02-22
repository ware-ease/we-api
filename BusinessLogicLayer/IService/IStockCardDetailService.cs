using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.StockCardDetail;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IStockCardDetailService
    {
        Task<PagedResult<StockCardDetail>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<PagedResult<StockCardDetail>> GetQueryableByStockCardId(string stockCardId, int? pageNumber, int? pageSize);
        Task<PagedResult<StockCardDetail>> GetQueryableByProductTypeId(string productTypeId, int? pageNumber, int? pageSize);
        Task<StockCardDetail> GetByIdAsync(string stockCardId, string productTypeId);
        Task<StockCardDetail> AddAsync(string stockCardId, string productTypeId, 
            CreateStockCardDetailDTO createStockCardDetailDTO);
        Task<StockCardDetail> UpdateAsync(string stockCardId, string productTypeId, 
            UpdateStockCardDetailDTO updateStockCardDetailDTO);
        Task DeleteAsync(string stockCardId, string productTypeId, string deletedBy);
    }
}
