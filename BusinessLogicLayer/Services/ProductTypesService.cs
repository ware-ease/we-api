using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.ProductType;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<ProductTypeDTO> AddProductType(ProductTypeCreateDTO request)
        {
            var category = await _categoryRepository.Get(request.CategoryId);
            if (category == null)
                throw new Exception("Category không tồn tại");

            var productType = _mapper.Map<ProductType>(request);
            productType.Category = category;
            await _productTypeRepository.Insert(productType);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<ProductTypeDTO>(productType);
        }

        public async Task<ProductTypeDTO> UpdateProductType(ProductTypeCreateDTO request)
        {
            var existedProductType = await _productTypeRepository.Get(request.CategoryId);
            if (existedProductType == null)
                throw new Exception("ProductType không tồn tại");

            if (!string.IsNullOrEmpty(request.CategoryId))
            {
                var category = await _categoryRepository.Get(request.CategoryId);
                if (category == null)
                    throw new Exception("Category not found");
                existedProductType.CategoryId = request.CategoryId;
            }

            _productTypeRepository.Update(existedProductType);
            await _unitOfWork.SaveAsync();


            var updatedProductType = await _productTypeRepository.Get(existedProductType.Id);
            if (updatedProductType == null)
                throw new Exception("Update failed, ProductType not found after update");

            return _mapper.Map<ProductTypeDTO>(updatedProductType);
        }

    }
}
