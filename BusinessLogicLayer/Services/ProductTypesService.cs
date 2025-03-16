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

    }
}
