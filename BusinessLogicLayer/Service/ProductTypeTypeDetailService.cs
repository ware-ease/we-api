//using AutoMapper;
//using BusinessLogicLayer.IService;
//using BusinessLogicLayer.Models.Pagination;
//using BusinessLogicLayer.Models.ProductTypeTypeDetail;
//using BusinessLogicLayer.Models.ReceivingDetail;
//using Data.Entity;
//using DataAccessLayer.IRepositories;
//using DataAccessLayer.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BusinessLogicLayer.Service
//{
//    public class ProductTypeTypeDetailService : IProductTypeTypeDetailService
//    {
//        private readonly IProductTypeTypeDetailRepository _productTypeTypeDetailRepository;
//        private readonly IProductTypeRepository _productTypeRepository;
//        private readonly ITypeDetailRepository _typeDetailRepository;
//        private readonly IGenericPaginationService _genericPaginationService;
//        private readonly IMapper _mapper;

//        public ProductTypeTypeDetailService(IProductTypeTypeDetailRepository productTypeTypeDetailRepository, 
//            IProductTypeRepository productTypeRepository, 
//            ITypeDetailRepository typeDetailRepository, 
//            IGenericPaginationService genericPaginationService, 
//            IMapper mapper)
//        {
//            _productTypeTypeDetailRepository = productTypeTypeDetailRepository;
//            _productTypeRepository = productTypeRepository;
//            _typeDetailRepository = typeDetailRepository;
//            _genericPaginationService = genericPaginationService;
//            _mapper = mapper;
//        }

//        public async Task<PagedResult<ProductTypeTypeDetail>> GetAllAsync(int? pageNumber, int? pageSize)
//        {
//            var query = _productTypeTypeDetailRepository.GetAllQueryable();
//            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
//        }

//        public async Task<ProductTypeTypeDetail> GetByIdAsync(string id)
//        {
//            var productTypeTypeDetail = await _productTypeTypeDetailRepository.GetByIdAsync(id);
//            if (productTypeTypeDetail == null)
//            {
//                throw new ArgumentException("Không thể tìm thấy Id");
//            }
//            return productTypeTypeDetail;
//        }

//        public async Task<ProductTypeTypeDetail> AddAsync(string typeDetailId, string productTypeId, 
//            CreateProductTypeTypeDetailDTO createProductTypeTypeDetailDTO)
//        {
//            var typeDetail = await _typeDetailRepository.GetByIdAsync(typeDetailId);
//            if (typeDetail == null)
//            {
//                throw new ArgumentException("Không tìm thấy TypeDetail Id");
//            }
//            var productType = await _productTypeRepository.GetByIdAsync(productTypeId);
//            if (productType == null)
//            {
//                throw new ArgumentException("Không tìm thấy ProductType Id");
//            }
//            if (string.IsNullOrWhiteSpace(createProductTypeTypeDetailDTO.CreatedBy))
//            {
//                throw new ArgumentException("Người tạo không được để trống");
//            }

//            var productTypeTypeDetail = _mapper.Map<ProductTypeTypeDetail>(createProductTypeTypeDetailDTO);
//            productTypeTypeDetail.CreatedTime = DateTime.Now;
//            productTypeTypeDetail.TypeDetailId = typeDetailId;
//            productTypeTypeDetail.ProductTypeId = productTypeId;
//            await _productTypeTypeDetailRepository.AddAsync(productTypeTypeDetail);
//            return productTypeTypeDetail;
//        }

//        public async Task<ProductTypeTypeDetail> UpdateAsync(string productTypeTypeDetailId, 
//            UpdateProductTypeTypeDetailDTO updateProductTypeTypeDetailDTO)
//        {
//            var productTypeTypeDetail = await _productTypeTypeDetailRepository.GetByIdAsync(productTypeTypeDetailId);
//            if (productTypeTypeDetail == null)
//            {
//                throw new ArgumentException("Không thể tìm thấy ProductTypeTypeDetail với ID này");
//            }
//            if (!string.IsNullOrWhiteSpace(updateProductTypeTypeDetailDTO.TypeDetailId))
//            {
//                var typeDetail = await _typeDetailRepository.GetByIdAsync(updateProductTypeTypeDetailDTO.TypeDetailId);
//                if (typeDetail == null)
//                {
//                    throw new ArgumentException("Không thể tìm thấy TypeDetail với ID này");
//                }
//                productTypeTypeDetail.TypeDetailId = updateProductTypeTypeDetailDTO.TypeDetailId;
//            }
//            if (!string.IsNullOrWhiteSpace(updateProductTypeTypeDetailDTO.ProductTypeId))
//            {
//                var productType = await _productTypeRepository.GetByIdAsync(updateProductTypeTypeDetailDTO.ProductTypeId);
//                if (productType == null)
//                {
//                    throw new ArgumentException("Không thể tìm thấy ProductType với ID này");
//                }
//                productTypeTypeDetail.ProductTypeId = updateProductTypeTypeDetailDTO.ProductTypeId;
//            }

            


//            if (!string.IsNullOrWhiteSpace(updateProductTypeTypeDetailDTO.TypeDetailId))
//            {
//                var updateTypeDetail = await _typeDetailRepository.GetByIdAsync(updateProductTypeTypeDetailDTO.TypeDetailId);
//                if (updateTypeDetail == null)
//                {
//                    throw new ArgumentException("Không thể tìm thấy TypeDetail với ID này");
//                }
//                productTypeTypeDetail.TypeDetailId = updateProductTypeTypeDetailDTO.TypeDetailId;
//            }
//            if (!string.IsNullOrWhiteSpace(updateProductTypeTypeDetailDTO.ProductTypeId))
//            {
//                var updateProductType = await _productTypeRepository.GetByIdAsync(updateProductTypeTypeDetailDTO.ProductTypeId);
//                if (updateProductType == null)
//                {
//                    throw new ArgumentException("Không thể tìm thấy ProductType với ID này");
//                }
//                productTypeTypeDetail.ProductTypeId = updateProductTypeTypeDetailDTO.ProductTypeId;
//            }

//            if (string.IsNullOrWhiteSpace(updateProductTypeTypeDetailDTO.LastUpdatedBy))
//            {
//                throw new ArgumentException("Người sửa không được để trống");
//            }

//            productTypeTypeDetail.LastUpdatedBy = updateProductTypeTypeDetailDTO.LastUpdatedBy;
//            productTypeTypeDetail.LastUpdatedTime = DateTime.Now;

//            await _productTypeTypeDetailRepository.UpdateAsync(productTypeTypeDetail);
//            return productTypeTypeDetail;
//        }

//        public async Task DeleteAsync(string Id, string deletedBy)
//        {
//            var productTypeTypeDetail = await _productTypeTypeDetailRepository.GetByIdAsync(Id);
//            if (productTypeTypeDetail == null)
//            {
//                throw new ArgumentException("Không thể tìm thấy Id để Delete");
//            }
//            if (string.IsNullOrWhiteSpace(deletedBy))
//            {
//                throw new ArgumentException("Người xóa không được để trống");
//            }
//            productTypeTypeDetail.DeletedBy = deletedBy;
//            productTypeTypeDetail.DeletedTime = DateTime.Now;
//            productTypeTypeDetail.IsDeleted = true;
//            await _productTypeTypeDetailRepository.UpdateAsync(productTypeTypeDetail);
//        }
//    }
//}
