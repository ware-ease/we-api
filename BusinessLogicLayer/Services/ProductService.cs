﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.Product;
using BusinessLogicLayer.Models.PurchaseReceipt;
using Data.Entity;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.Product;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Migrations;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ProductService : GenericService<Product>, IProductService
    {

        private readonly IProductRepository _productRepository;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<ProductType> _productTypeRepository;
        private readonly IGenericRepository<Brand> _brandRepository;
        private readonly IGenericRepository<Unit> _unitRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IGenericRepository<Product> genericRepository,
                              IProductRepository productRepository,
                              IGenericRepository<Category> categoryRepository,
                              IGenericRepository<Brand> brandRepository,
                              IGenericRepository<Unit> unitRepository,
                              IGenericRepository<ProductType> productTypeRepository,
                              IUnitOfWork unitOfWork,
                              IMapper mapper)
            : base(genericRepository, mapper, unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productTypeRepository = productTypeRepository;
            _brandRepository = brandRepository;
            _unitRepository = unitRepository;
        }

        public async Task<int> Count()
        {
            var batches = await _productRepository.GetAllNoPaging();
            return batches.Count(b => !b.IsDeleted);
        }

        public override async Task<ServiceResponse> Get<TResult>()
        {
            var products = await _genericRepository.GetAllNoPaging(
                includeProperties: "ProductType,ProductType.Category,Brand,Unit"
            );

            List<TResult> mappedResults = _mapper.Map<List<TResult>>(products);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Lấy thành công!",
                Data = mappedResults
            };
        }

        public override async Task<ServiceResponse> Get<TResult>(string id)
        {
            var product = await _genericRepository.GetByCondition(
                p => p.Id == id,
                includeProperties: "ProductType,ProductType.Category,Brand,Unit"
            );

            if (product == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Sản phẩm không tìm thấy!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(product);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Lấy thành công!",
                Data = result
            };
        }


        public async Task<ServiceResponse> GetAllProducts()
        {
            var query = _genericRepository.Get();

            var products = await query
                .ProjectTo<ProductDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Lấy thành công!",
                Data = products
            };
        }

        public async Task<ProductDTO> AddProduct(ProductCreateDTO request)
        {
            var existingProduct = await _productRepository.GetByCondition(p => p.Sku == request.Sku);
            if (existingProduct != null)
                throw new Exception("SKU đã tồn tại trong hệ thống!");

            var productType = await _productTypeRepository.GetByCondition(p => p.Id == request.ProductTypeId);
            if (productType == null)
                throw new Exception("Loại sản phẩm không tồn tại");

            var brand = await _brandRepository.GetByCondition(p => p.Id == request.BrandId);
            if (brand == null)
                throw new Exception("Hãng không tồn tại");

            var unit = await _unitRepository.GetByCondition(p => p.Id == request.UnitId);
            if (unit == null)
                throw new Exception("Đơn vị không tồn tại");


            var product = _mapper.Map<Product>(request);
            product.ProductType = productType;
            product.Brand = brand;
            product.Unit = unit;
            product.ProductTypeId = productType.Id;
            product.BrandId = brand.Id;
            product.UnitId = unit.Id;

            // Lưu vào database
            await _productRepository.Insert(product);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> GetProductById(string id)
        {
            var product = await _productRepository.GetFullProductById(id);
            if (product == null)
                throw new Exception("Sản phẩm không tìm thấy");
            return _mapper.Map<ProductDTO>(product);
        }


        public async Task<ProductDTO> UpdateProduct(ProductUpdateDTO request)
        {
            var existingProduct = await _productRepository.GetByCondition(p => p.Id == request.Id);
            if (existingProduct == null)
                throw new Exception("Sản phẩm không tìm thấy");

            if (!string.IsNullOrEmpty(request.Name))
            {
                existingProduct.Name = request.Name;
            }
            if (!string.IsNullOrEmpty(request.Sku))
            {
                var existingSku = await _productRepository.GetByCondition(p => p.Sku == request.Sku);
                if (existingSku != null)
                    throw new Exception("SKU đã tồn tại trong hệ thống!");

                existingProduct.Sku = request.Sku;
            }

            if (request.IsBatchManaged.HasValue)
                existingProduct.IsBatchManaged = request.IsBatchManaged.Value;

            if (!string.IsNullOrEmpty(request.imageUrl))
            {
                existingProduct.imageUrl = request.imageUrl;
            }


            if (!string.IsNullOrEmpty(request.ProductTypeId))
            {
                var productType = await _productTypeRepository.GetByCondition(p => p.Id == request.ProductTypeId);
                if (productType == null)
                    throw new Exception("Loại sản phẩm không tìm thấy");
                existingProduct.ProductTypeId = request.ProductTypeId;
            }

            if (!string.IsNullOrEmpty(request.BrandId))
            {
                var brand = await _brandRepository.GetByCondition(p => p.Id == request.BrandId);
                if (brand == null)
                    throw new Exception("Hãng không tìm thấy");
                existingProduct.BrandId = request.BrandId;
            }

            if (!string.IsNullOrEmpty(request.UnitId))
            {
                var unit = await _unitRepository.GetByCondition(p => p.Id == request.UnitId);
                if (unit == null)
                    throw new Exception("Đơn vị không tìm thấy");
                existingProduct.UnitId = request.UnitId;
            }

            //_mapper.Map(request, existingProduct);

            _productRepository.Update(existingProduct);
            await _unitOfWork.SaveAsync();


            var updatedProduct = await _productRepository.GetByCondition(p => p.Id == existingProduct.Id,
                includeProperties: "ProductType,Brand,Unit");
            if (updatedProduct == null)
                throw new Exception("Cập nhật lỗi, sản phẩm không được tìm thấy sau khi cập nhật");

            return _mapper.Map<ProductDTO>(updatedProduct);
        }

        public async Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
            bool? IsBatchManaged = null,
            string? keyword = null)
        {

            Expression<Func<Product, bool>> filter = p =>
                (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)
                || p.Sku.Contains(keyword)
                || p.ProductType.Name.Contains(keyword)
                || p.Brand.Name.Contains(keyword)
                || (p.ProductType.Category.Name + " " + p.ProductType.Category.Note).Contains(keyword))
                && (!IsBatchManaged.HasValue || p.IsBatchManaged == IsBatchManaged.Value);

            var totalRecords = await _genericRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize,
                includeProperties: "ProductType,ProductType.Category,Brand,Unit");

            var mappedResults = _mapper.Map<IEnumerable<TResult>>(results);

            int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? totalRecords));

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Tìm kiếm thành công!",
                Data = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageIndex = pageIndex ?? 1,
                    PageSize = pageSize ?? totalRecords,
                    Records = mappedResults
                }
            };
        }
    }
}
