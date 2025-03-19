using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.ProductType;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
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
    public class ProductTypesService : GenericService<ProductType>, IProductTypesService
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<ProductType> _productTypeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ProductTypesService(IGenericRepository<ProductType> genericService,
            IGenericRepository<Category> categoryRepository,
            IGenericRepository<ProductType> genericRepository,
            IMapper mapper, 
            IUnitOfWork unitOfWork) : base(genericService, mapper, unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
            _productTypeRepository = genericRepository;
        }

        public async Task<int> Count()
        {
            var batches = await _productTypeRepository.GetAllNoPaging();
            return batches.Count(b => !b.IsDeleted);
        }

        public override async Task<ServiceResponse> Get<TResult>()
        {
            var productTypes = await _genericRepository.GetAllNoPaging(
                includeProperties: "Category"
            );

            List<TResult> mappedResults = _mapper.Map<List<TResult>>(productTypes);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = mappedResults
            };
        }

        // Lấy ProductType theo ID
        public override async Task<ServiceResponse> Get<TResult>(string id)
        {
            var productType = await _genericRepository.GetByCondition(
                pt => pt.Id == id,
                includeProperties: "Category"
            );

            if (productType == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "ProductType not found!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(productType);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = result
            };
        }

        public async Task<ServiceResponse> GetAllProducts()
        {
            var query = _genericRepository.Get();

            var products = await query
                .ProjectTo<ProductTypeDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = products
            };
        }

        public async Task<ProductTypeDTO> AddProductType(ProductTypeCreateDTO request)
        {
            var category = await _categoryRepository.GetByCondition(p => p.Id == request.CategoryId);
            if (category == null)
                throw new Exception("Category không tồn tại");

            var productType = _mapper.Map<ProductType>(request);
            productType.Category = category;
            await _productTypeRepository.Insert(productType);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<ProductTypeDTO>(productType);
        }

        public async Task<ProductTypeDTO> UpdateProductType(ProductTypeUpdateDTO request)
        {
            var existedProductType = await _productTypeRepository.GetByCondition(p => p.Id == request.Id);
            if (existedProductType == null)
                throw new Exception("ProductType không tồn tại");

            if (!string.IsNullOrEmpty(request.Name))
            {
                existedProductType.Name = request.Name;
            }
            if (!string.IsNullOrEmpty(request.Note))
            {
                existedProductType.Note = request.Note;
            }

            if (!string.IsNullOrEmpty(request.CategoryId))
            {
                var category = await _categoryRepository.GetByCondition(p => p.Id == request.CategoryId);
                if (category == null)
                    throw new Exception("Category not found");
                existedProductType.CategoryId = request.CategoryId;
            }

            _productTypeRepository.Update(existedProductType);
            await _unitOfWork.SaveAsync();


            var updatedProductType = await _productTypeRepository.GetByCondition(p => p.Id == existedProductType.Id);
            if (updatedProductType == null)
                throw new Exception("Update failed, ProductType not found after update");

            return _mapper.Map<ProductTypeDTO>(updatedProductType);
        }

        public async Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null)
        {

            Expression<Func<ProductType, bool>> filter = p =>
                (string.IsNullOrEmpty(keyword) 
                || (p.Name + " " + p.Note).Contains(keyword)
                || (p.Category.Name + " " + p.Category.Note).Contains(keyword));

            var totalRecords = await _genericRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize,
                includeProperties: "Category");

            var mappedResults = _mapper.Map<IEnumerable<TResult>>(results);

            int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? totalRecords));

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Search successful!",
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
