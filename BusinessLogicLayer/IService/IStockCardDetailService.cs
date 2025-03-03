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
        Task<PagedResult<InOutDetail>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<PagedResult<InOutDetail>> GetQueryableByStockCardId(string stockCardId, int? pageNumber, int? pageSize);
        Task<PagedResult<InOutDetail>> GetQueryableByProductTypeId(string productTypeId, int? pageNumber, int? pageSize);
        Task<InOutDetail> GetByIdAsync(string stockCardId, string productTypeId);
        Task<InOutDetail> AddAsync(string stockCardId, string productTypeId, 
            CreateStockCardDetailDTO createStockCardDetailDTO);
        Task<InOutDetail> UpdateAsync(string stockCardId, string productTypeId, 
            UpdateStockCardDetailDTO updateStockCardDetailDTO);
        Task DeleteAsync(string stockCardId, string productTypeId, string deletedBy);
    }
}
