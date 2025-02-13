using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.PurchaseReceipt;
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
    public class PurchaseReceiptService : IPurchaseReceiptService
    {
        private readonly IPurchaseReceiptRepository _purchaseReceiptRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IGenericPaginationService _genericPaginationService;
        private readonly IMapper _mapper;
        public PurchaseReceiptService(IPurchaseReceiptRepository purchaseReceiptRepository, ISupplierRepository supplierRepository, IGenericPaginationService genericPaginationService, IMapper mapper)
        {
            _purchaseReceiptRepository = purchaseReceiptRepository;
            _supplierRepository = supplierRepository;
            _genericPaginationService = genericPaginationService;
            _mapper = mapper;
        }

        public async Task<PagedResult<PurchaseReceipt>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = _purchaseReceiptRepository.GetAllQueryable();
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<PurchaseReceipt> GetByIdAsync(string id)
        {
            var supplier = await _purchaseReceiptRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id");
            }
            return supplier;
        }

        public async Task<PurchaseReceipt> AddAsync(string supplierId, CreatePurchaseReceiptDTO createPurchaseReceiptDTO)
        {
            var supplier = await _supplierRepository.GetByIdAsync(supplierId);
            if (supplier == null)
            {
                throw new ArgumentException("Không tìm thấy supplier Id");
            }
            if (createPurchaseReceiptDTO.Date == null)
            {
                throw new ArgumentException("Ngày tạo hóa đơn không được để trống");
            }
            if (string.IsNullOrWhiteSpace(createPurchaseReceiptDTO.CreatedBy))
            {
                throw new ArgumentException("Người tạo không được để trống");
            }

            var purchaseReceipt = _mapper.Map<PurchaseReceipt>(createPurchaseReceiptDTO);
            purchaseReceipt.CreatedTime = DateTime.Now;
            purchaseReceipt.SupplierId = supplierId;
            await _purchaseReceiptRepository.AddAsync(purchaseReceipt);
            return purchaseReceipt;
        }

        public async Task<PurchaseReceipt> UpdateAsync(string purchaseReceiptId, UpdatePurchaseReceiptDTO updatePurchaseReceiptDTO)
        {
            var purchaseReceipt = await _purchaseReceiptRepository.GetByIdAsync(purchaseReceiptId);
            if (purchaseReceipt == null)
            {
                throw new ArgumentException("Không thể tìm thấy PurchaseReceipt với ID này");
            }

            if (!string.IsNullOrWhiteSpace(updatePurchaseReceiptDTO.SupplierId))
            {
                var updateSupplier = await _supplierRepository.GetByIdAsync(updatePurchaseReceiptDTO.SupplierId);
                if (updateSupplier == null)
                {
                    throw new ArgumentException("Không thể tìm thấy Supplier với ID này");
                }
                purchaseReceipt.SupplierId = updatePurchaseReceiptDTO.SupplierId;
            }
            if (updatePurchaseReceiptDTO.Date != null)
            {
                if (updatePurchaseReceiptDTO.Date >= DateTime.Now)
                {
                    throw new ArgumentException("Không thể đặt ngày ở tương lai");
                }
                purchaseReceipt.Date = updatePurchaseReceiptDTO.Date;
            }
            if (string.IsNullOrWhiteSpace(updatePurchaseReceiptDTO.LastUpdatedBy))
            {
                throw new ArgumentException("Người tạo không được để trống");
            }

            purchaseReceipt.LastUpdatedBy = updatePurchaseReceiptDTO.LastUpdatedBy;
            purchaseReceipt.LastUpdatedTime = DateTime.Now;

            await _purchaseReceiptRepository.UpdateAsync(purchaseReceipt);
            return purchaseReceipt;
        }

        public async Task DeleteAsync(string Id, string deletedBy)
        {
            var purchaseReceipt = await _purchaseReceiptRepository.GetByIdAsync(Id);
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
            await _purchaseReceiptRepository.UpdateAsync(purchaseReceipt);
        }
    }
}
