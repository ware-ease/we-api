using BusinessLogicLayer.Generic;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.Product;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.Product;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IProductService : IGenericService
    {
        Task<int> Count();
        Task<ProductDTO> GetProductById(string id);
        Task<ServiceResponse> GetAllProducts();
        Task<ProductDTO> AddProduct(ProductCreateDTO request);
        Task<ProductDTO> UpdateProduct(ProductUpdateDTO request);
        Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null, bool? IsBatchManaged = true,
                                                                   string? keyword = null);

        /*Task<PagedResult<Product>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<PagedResult<Product>> GetAllByCategoryIdAsync(string categoryId, int? pageNumber, int? pageSize);
        Task<Product> GetByIdAsync(string id);
        Task<Product> AddAsync(string categoryId, CreateProductDTO createProductDTO);
        Task<Product> UpdateAsync(string id, UpdateProductDTO updateProductDTO);
        Task DeleteAsync(string Id, string deletedBy);*/
    }
}
