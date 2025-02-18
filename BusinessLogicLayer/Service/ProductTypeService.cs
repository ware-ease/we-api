using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.ProductType;
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
    public class ProductTypeService : IProductTypeService
    {
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGenericPaginationService _genericPaginationService;
        private readonly IMapper _mapper;

        public ProductTypeService(IProductTypeRepository productTypeRepository, IProductRepository productRepository, IGenericPaginationService genericPaginationService, IMapper mapper)
        {
            _productTypeRepository = productTypeRepository;
            _productRepository = productRepository;
            _genericPaginationService = genericPaginationService;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductType>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = _productTypeRepository.GetAllQueryable();
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<ProductType> GetByIdAsync(string id)
        {
            var supplier = await _productTypeRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id");
            }
            return supplier;
        }

        public async Task<ProductType> AddAsync(string productId, CreateProductTypeDTO createProductTypeDTO)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new ArgumentException("Không tìm thấy product Id");
            }
            if (string.IsNullOrWhiteSpace(createProductTypeDTO.Name))
            {
                throw new ArgumentException("Name không được để trống");
            }
            if (string.IsNullOrWhiteSpace(createProductTypeDTO.CreatedBy))
            {
                throw new ArgumentException("Người tạo không được để trống");
            }

            var productType = _mapper.Map<ProductType>(createProductTypeDTO);
            productType.CreatedTime = DateTime.Now;
            productType.ProductId = productId;
            await _productTypeRepository.AddAsync(productType);
            return productType;
        }

        public async Task<ProductType> UpdateAsync(string productTypeId, UpdateProductTypeDTO updateProductTypeDTO)
        {
            var productType = await _productTypeRepository.GetByIdAsync(productTypeId);
            if (productType == null)
            {
                throw new ArgumentException("Không thể tìm thấy productType với ID này");
            }

            if (!string.IsNullOrWhiteSpace(updateProductTypeDTO.ProductId))
            {
                var updateProduct = await _productRepository.GetByIdAsync(updateProductTypeDTO.ProductId);
                if (updateProduct == null)
                {
                    throw new ArgumentException("Không thể tìm thấy Product với ID này");
                }
                productType.ProductId = updateProductTypeDTO.ProductId;
            }
            if (!string.IsNullOrWhiteSpace(updateProductTypeDTO.Name))
            {
                productType.Name = updateProductTypeDTO.Name;
            }
            if (string.IsNullOrWhiteSpace(updateProductTypeDTO.LastUpdatedBy))
            {
                throw new ArgumentException("Người sửa không được để trống");
            }

            productType.LastUpdatedBy = updateProductTypeDTO.LastUpdatedBy;
            productType.LastUpdatedTime = DateTime.Now;

            await _productTypeRepository.UpdateAsync(productType);
            return productType;
        }

        public async Task DeleteAsync(string Id, string deletedBy)
        {
            var purchaseReceipt = await _productTypeRepository.GetByIdAsync(Id);
            if (purchaseReceipt == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id để Delete");
            }
            if (string.IsNullOrWhiteSpace(deletedBy))
            {
                throw new ArgumentException("Người xóa không được để trống");
            }
            purchaseReceipt.DeletedBy = deletedBy;
            purchaseReceipt.DeletedTime = DateTime.Now;
            purchaseReceipt.IsDeleted = true;
            await _productTypeRepository.UpdateAsync(purchaseReceipt);
        }
    }
}
