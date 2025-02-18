using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.Product;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IProductService
    {
        Task<PagedResult<Product>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<PagedResult<Product>> GetAllByCategoryIdAsync(string categoryId, int? pageNumber, int? pageSize);
        Task<Product> GetByIdAsync(string id);
        Task<Product> AddAsync(string categoryId, CreateProductDTO createProductDTO);
        Task<Product> UpdateAsync(string id, UpdateProductDTO updateProductDTO);
        Task DeleteAsync(string Id, string deletedBy);
    }
}
