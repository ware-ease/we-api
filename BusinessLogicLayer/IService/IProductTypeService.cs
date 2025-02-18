using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.ProductType;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IProductTypeService
    {
        Task<PagedResult<ProductType>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<ProductType> GetByIdAsync(string id);
        Task<ProductType> AddAsync(string productId, CreateProductTypeDTO createProductTypeDTO);
        Task<ProductType> UpdateAsync(string productTypeId, UpdateProductTypeDTO updateProductTypeDTO);
        Task DeleteAsync(string Id, string deletedBy);
    }
}
