using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.PurchaseReceipt;
using BusinessLogicLayer.Models.ReceivingNote;
using BusinessLogicLayer.Models.Supplier;
using Data.Entity;
using DataAccessLayer.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class ReceivingNoteService : IReceivingNoteService
    {
        private readonly IReceivingNoteRepository _receivingNoteRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IPurchaseReceiptRepository _purchaseReceiptRepository;
        private readonly IGenericPaginationService _genericPaginationService;
        private readonly IMapper _mapper;

        public ReceivingNoteService(IReceivingNoteRepository receivingNoteRepository,
            IGenericPaginationService genericPaginationService,
            IMapper mapper,
            ISupplierRepository supplierRepository,
            IPurchaseReceiptRepository purchaseReceiptRepository)
        {
            _receivingNoteRepository = receivingNoteRepository;
            _genericPaginationService = genericPaginationService;
            _mapper = mapper;
            _supplierRepository = supplierRepository;
            _purchaseReceiptRepository = purchaseReceiptRepository;
        }

        public async Task<PagedResult<ReceivingNote>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = _receivingNoteRepository.GetAllQueryable();
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<ReceivingNote> GetByIdAsync(string id)
        {
            var note = await _receivingNoteRepository.GetByIdAsync(id);
            if (note == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id");
            }
            return note;
        }

        public async Task<ReceivingNote> AddAsync(string supplierId, string purchaseId, CreateReceivingNoteDTO createReceivingNoteDTO)
        {
            var note = _mapper.Map<ReceivingNote>(createReceivingNoteDTO);
            if (!string.IsNullOrWhiteSpace(supplierId))
            {
                var supplier = await _supplierRepository.GetByIdAsync(supplierId);
                if (supplier == null)
                {
                    throw new ArgumentException("Không thể tìm thấy Id của Supplier");
                }
                note.SupplierId = supplierId;
            }
            if (!string.IsNullOrWhiteSpace(purchaseId))
            {
                var purchaseReceipt = await _purchaseReceiptRepository.GetByIdAsync(purchaseId);
                if (purchaseReceipt == null)
                {
                    throw new ArgumentException("Không thể tìm thấy Id của PurchaseReceipt");
                }
                note.PurchaseId = purchaseId;
            }
            if (string.IsNullOrWhiteSpace(createReceivingNoteDTO.CreatedBy))
            {
                throw new ArgumentException("Người tạo không được để trống");
            }

            note.CreatedTime = DateTime.Now;
            await _receivingNoteRepository.AddAsync(note);
            return note;
        }

        public async Task<ReceivingNote> UpdateAsync(string id, UpdateReceivingNoteDTO updateReceivingNoteDTO)
        {
            var note = await _receivingNoteRepository.GetByIdAsync(id);
            if (note == null)
            {
                throw new ArgumentException("Không thể tìm thấy ReceivingNote với ID này");
            }

            if (updateReceivingNoteDTO.Date != null)
            {
                if (updateReceivingNoteDTO.Date >= DateTime.Now)
                {
                    throw new ArgumentException("Không thể đặt ngày ở tương lai");
                }
                note.Date = updateReceivingNoteDTO.Date;
            }

            if (!string.IsNullOrWhiteSpace(updateReceivingNoteDTO.SupplierId))
            {
                var supplier = await _supplierRepository.GetByIdAsync(updateReceivingNoteDTO.SupplierId);
                if (supplier == null)
                {
                    throw new ArgumentException("Không thể tìm thấy Supplier với ID này");
                }
                note.SupplierId = updateReceivingNoteDTO.SupplierId;
            }

            if (!string.IsNullOrWhiteSpace(updateReceivingNoteDTO.PurchaseId))
            {
                var purchase = await _purchaseReceiptRepository.GetByIdAsync(updateReceivingNoteDTO.PurchaseId);
                if (purchase == null)
                {
                    throw new ArgumentException("Không thể tìm thấy PurchaseReicept với ID này");
                }
                note.PurchaseId = updateReceivingNoteDTO.PurchaseId;
            }

            if (string.IsNullOrWhiteSpace(updateReceivingNoteDTO.LastUpdatedBy))
            {
                throw new ArgumentException("Người tạo không được để trống");
            }


            note.LastUpdatedBy = updateReceivingNoteDTO.LastUpdatedBy;
            note.LastUpdatedTime = DateTime.Now;

            await _receivingNoteRepository.UpdateAsync(note);
            return note;
        }

        public async Task DeleteAsync(string Id, string deletedBy)
        {
            var note = await _receivingNoteRepository.GetByIdAsync(Id);
            if (note == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id để Delete");
            }
            if (string.IsNullOrWhiteSpace(deletedBy))
            {
                throw new ArgumentException("Người xóa không được để trống");
            }
            note.DeletedBy = deletedBy;
            note.DeletedTime = DateTime.Now;
            note.IsDeleted = true;
            await _receivingNoteRepository.UpdateAsync(note);
        }
    }
}
