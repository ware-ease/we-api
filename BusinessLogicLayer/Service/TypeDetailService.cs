//using AutoMapper;
//using BusinessLogicLayer.IService;
//using BusinessLogicLayer.Models.Pagination;
//using BusinessLogicLayer.Models.PurchaseReceipt;
//using BusinessLogicLayer.Models.TypeDetail;
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
//    public class TypeDetailService : ITypeDetailService
//    {
//        private readonly ITypeDetailRepository _repository;
//        private readonly IGenericPaginationService _genericPaginationService;
//        private readonly IMapper _mapper;

//        public TypeDetailService(ITypeDetailRepository repository, IGenericPaginationService genericPaginationService, IMapper mapper)
//        {
//            _repository = repository;
//            _genericPaginationService = genericPaginationService;
//            _mapper = mapper;
//        }

//        public async Task<PagedResult<TypeDetail>> GetAllAsync(int? pageNumber, int? pageSize)
//        {
//            var query = _repository.GetAllQueryable();
//            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
//        }

//        public async Task<TypeDetail> GetByIdAsync(string id)
//        {
//            var typeDetail = await _repository.GetByIdAsync(id);
//            if (typeDetail == null)
//            {
//                throw new ArgumentException("Không thể tìm thấy Id");
//            }
//            return typeDetail;
//        }

//        public async Task<TypeDetail> AddAsync(CreateTypeDetailDTO createTypeDetailDTO)
//        {
//            if (string.IsNullOrWhiteSpace(createTypeDetailDTO.Name))
//            {
//                throw new ArgumentException("Name không được để trống");
//            }
//            if (string.IsNullOrWhiteSpace(createTypeDetailDTO.CreatedBy))
//            {
//                throw new ArgumentException("Người tạo không được để trống");
//            }

//            var typeDetail = _mapper.Map<TypeDetail>(createTypeDetailDTO);
//            typeDetail.CreatedTime = DateTime.Now;
//            await _repository.AddAsync(typeDetail);
//            return typeDetail;
//        }

//        public async Task<TypeDetail> UpdateAsync(string typeDetaiId, UpdateTypeDetailDTO updateTypeDetailDTO)
//        {
//            var typeDetail = await _repository.GetByIdAsync(typeDetaiId);
//            if (typeDetail == null)
//            {
//                throw new ArgumentException("Không thể tìm thấy TypeDetail với ID này");
//            }
//            if (!string.IsNullOrWhiteSpace(updateTypeDetailDTO.Name))
//            {
//                typeDetail.Name = updateTypeDetailDTO.Name;
//            }
//            if (!string.IsNullOrWhiteSpace(updateTypeDetailDTO.Description))
//            {
//                typeDetail.Description = updateTypeDetailDTO.Description;
//            }
//            if (string.IsNullOrWhiteSpace(updateTypeDetailDTO.LastUpdatedBy))
//            {
//                throw new ArgumentException("Người sửa không được để trống");
//            }

//            typeDetail.LastUpdatedBy = updateTypeDetailDTO.LastUpdatedBy;
//            typeDetail.LastUpdatedTime = DateTime.Now;

//            await _repository.UpdateAsync(typeDetail);
//            return typeDetail;
//        }

//        public async Task DeleteAsync(string Id, string deletedBy)
//        {
//            var typeDetail = await _repository.GetByIdAsync(Id);
//            if (typeDetail == null)
//            {
//                throw new ArgumentException("Không thể tìm thấy Id để Delete");
//            }
//            if (string.IsNullOrWhiteSpace(deletedBy))
//            {
//                throw new ArgumentException("Người xóa không được để trống");
//            }
//            typeDetail.DeletedBy = deletedBy;
//            typeDetail.DeletedTime = DateTime.Now;
//            typeDetail.IsDeleted = true;
//            await _repository.UpdateAsync(typeDetail);
//        }
//    }
//}
