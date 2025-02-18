using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.Product;
using BusinessLogicLayer.Models.PurchaseReceipt;
using Data.Entity;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGenericPaginationService _genericPaginationService;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, 
            ICategoryRepository categoryRepository, 
            IMapper mapper, 
            IGenericPaginationService genericPaginationService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _genericPaginationService = genericPaginationService;
        }

        public async Task<PagedResult<Product>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = _productRepository.GetAllQueryable();
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<PagedResult<Product>> GetAllByCategoryIdAsync(string categoryId, int? pageNumber, int? pageSize)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new ArgumentException("Không tìm thấy Category Id");
            }
            var query = _productRepository.GetByCategoryIdQueryable(categoryId);
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id");
            }
            return product;
        }

        public async Task<Product> AddAsync(string categoryId, CreateProductDTO createProductDTO)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new ArgumentException("Không tìm thấy Category Id");
            }
            if (category.IsDeleted == true)
            {
                throw new ArgumentException("Category này đã bị xóa");
            }
            if (string.IsNullOrWhiteSpace(createProductDTO.Name))
            {
                throw new ArgumentException("Tên sản phẩm không được để trống");
            }
            if (string.IsNullOrWhiteSpace(createProductDTO.CreatedBy))
            {
                throw new ArgumentException("Người tạo không được để trống");
            }

            var product = _mapper.Map<Product>(createProductDTO);
            product.CreatedTime = DateTime.Now;
            product.CategoryId = categoryId;
            await _productRepository.AddAsync(product);
            return product;
        }

        public async Task<Product> UpdateAsync(int id, UpdateProductDTO updateProductDTO)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new ArgumentException("Không thể tìm thấy Product với ID này");
            }

            if (!string.IsNullOrWhiteSpace(updateProductDTO.Name))
            {
                product.Name = updateProductDTO.Name;
            }
            if (!string.IsNullOrWhiteSpace(updateProductDTO.CategoryId))
            {
                var updateCategory = await _categoryRepository.GetByIdAsync(updateProductDTO.CategoryId);
                if (updateCategory == null)
                {
                    throw new ArgumentException("Không thể tìm thấy Category với ID này");
                }
                if (updateCategory.IsDeleted == true)
                {
                    throw new ArgumentException("Category này đã bị xóa");
                }
                product.CategoryId = updateProductDTO.CategoryId;
            }
            if (string.IsNullOrWhiteSpace(updateProductDTO.LastUpdatedBy))
            {
                throw new ArgumentException("Người sửa không được để trống");
            }

            product.LastUpdatedBy = updateProductDTO.LastUpdatedBy;
            product.LastUpdatedTime = DateTime.Now;

            await _productRepository.UpdateAsync(product);
            return product;
        }

        public async Task DeleteAsync(int Id, string deletedBy)
        {
            var product = await _productRepository.GetByIdAsync(Id);
            if (product == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id để Delete");
            }
            if (string.IsNullOrWhiteSpace(deletedBy))
            {
                throw new ArgumentException("Người xóa không được để trống");
            }
            product.DeletedBy = deletedBy;
            product.DeletedTime = DateTime.Now;
            product.IsDeleted = true;
            await _productRepository.UpdateAsync(product);
        }
    }
}
