using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.PurchaseReceipt;
using BusinessLogicLayer.Models.ReceivingDetail;
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
    public class ReceivingDetailService : IReceivingDetailService
    {
        private readonly IReceivingDetailRepository _receivingDetailRepository;
        private readonly IReceivingNoteRepository _receivingNoteRepository;
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IGenericPaginationService _genericPaginationService;
        private readonly IMapper _mapper;

        public ReceivingDetailService(IReceivingDetailRepository receivingDetailRepository, IReceivingNoteRepository receivingNoteRepository, IProductTypeRepository productTypeRepository, IGenericPaginationService genericPaginationService, IMapper mapper)
        {
            _receivingDetailRepository = receivingDetailRepository;
            _receivingNoteRepository = receivingNoteRepository;
            _productTypeRepository = productTypeRepository;
            _genericPaginationService = genericPaginationService;
            _mapper = mapper;
        }

        public async Task<PagedResult<ReceivingDetail>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = _receivingDetailRepository.GetAllQueryable();
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<ReceivingDetail> GetByIdAsync(string id)
        {
            var receivingDetail = await _receivingDetailRepository.GetByIdAsync(id);
            if (receivingDetail == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id");
            }
            return receivingDetail;
        }

        public async Task<ReceivingDetail> AddAsync(string noteId, string productTypeId, CreateReceivingDetailDTO createReceivingDetailDTO)
        {
            var note = await _receivingNoteRepository.GetByIdAsync(noteId);
            if (note == null)
            {
                throw new ArgumentException("Không tìm thấy ReceivingNote Id");
            }
            var productType = await _productTypeRepository.GetByIdAsync(productTypeId);
            if (productType == null)
            {
                throw new ArgumentException("Không tìm thấy ProductType Id");
            }
            if (createReceivingDetailDTO.Quanlity <= 0)
            {
                throw new ArgumentException("Quantity không được nhỏ hơn 0");
            }
            if (createReceivingDetailDTO.Price <= 0)
            {
                throw new ArgumentException("Price không được nhỏ hơn 0");
            }
            if (string.IsNullOrWhiteSpace(createReceivingDetailDTO.CreatedBy))
            {
                throw new ArgumentException("Người tạo không được để trống");
            }

            var receivingDetail = _mapper.Map<ReceivingDetail>(createReceivingDetailDTO);
            receivingDetail.CreatedTime = DateTime.Now;
            receivingDetail.NoteId = noteId;
            //receivingDetail.ProductTypeId = productTypeId;
            await _receivingDetailRepository.AddAsync(receivingDetail);
            return receivingDetail;
        }

        public async Task<ReceivingDetail> UpdateAsync(string receivingDetailId, UpdateReceivingDetailDTO updateReceivingDetailDTO)
        {
            var receivingDetail = await _receivingDetailRepository.GetByIdAsync(receivingDetailId);
            if (receivingDetail == null)
            {
                throw new ArgumentException("Không thể tìm thấy ReceivingDetail với ID này");
            }

            if (updateReceivingDetailDTO.Quanlity <= 0)
            {
                throw new ArgumentException("Quantity không được nhỏ hơn 0");
            }
            if (updateReceivingDetailDTO.Price <= 0)
            {
                throw new ArgumentException("Price không được nhỏ hơn 0");
            }

            if (!string.IsNullOrWhiteSpace(updateReceivingDetailDTO.NoteId))
            {
                var updateNote = await _receivingNoteRepository.GetByIdAsync(updateReceivingDetailDTO.NoteId);
                if (updateNote == null)
                {
                    throw new ArgumentException("Không thể tìm thấy ReceivingNote với ID này");
                }
                receivingDetail.NoteId = updateReceivingDetailDTO.NoteId;
            }
            if (!string.IsNullOrWhiteSpace(updateReceivingDetailDTO.ProductTypeId))
            {
                var updateProductType = await _productTypeRepository.GetByIdAsync(updateReceivingDetailDTO.ProductTypeId);
                if (updateProductType == null)
                {
                    throw new ArgumentException("Không thể tìm thấy ProductType với ID này");
                }
                //receivingDetail.ProductTypeId = updateReceivingDetailDTO.ProductTypeId;
            }

            if (string.IsNullOrWhiteSpace(updateReceivingDetailDTO.LastUpdatedBy))
            {
                throw new ArgumentException("Người sửa không được để trống");
            }

            receivingDetail.LastUpdatedBy = updateReceivingDetailDTO.LastUpdatedBy;
            receivingDetail.LastUpdatedTime = DateTime.Now;

            await _receivingDetailRepository.UpdateAsync(receivingDetail);
            return receivingDetail;
        }

        public async Task DeleteAsync(string Id, string deletedBy)
        {
            var receivingDetail = await _receivingDetailRepository.GetByIdAsync(Id);
            if (receivingDetail == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id để Delete");
            }
            if (string.IsNullOrWhiteSpace(deletedBy))
            {
                throw new ArgumentException("Người xóa không được để trống");
            }
            receivingDetail.DeletedBy = deletedBy;
            receivingDetail.DeletedTime = DateTime.Now;
            receivingDetail.IsDeleted = true;
            await _receivingDetailRepository.UpdateAsync(receivingDetail);
        }
    }
}
