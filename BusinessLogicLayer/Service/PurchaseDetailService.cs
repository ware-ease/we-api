using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.PurchaseDetail;
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
    public class PurchaseDetailService : IPurchaseDetailService
    {
        private readonly IPurchaseDetailRepository _purchaseDetailRepository;
        private readonly IPurchaseReceiptRepository _purchaseReceiptRepository;
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IGenericPaginationService _genericPaginationService;
        private readonly IMapper _mapper;

        public PurchaseDetailService(IPurchaseDetailRepository purchaseDetailRepository, IPurchaseReceiptRepository purchaseReceiptRepository, IProductTypeRepository productTypeRepository, IGenericPaginationService genericPaginationService, IMapper mapper)
        {
            _purchaseDetailRepository = purchaseDetailRepository;
            _purchaseReceiptRepository = purchaseReceiptRepository;
            _productTypeRepository = productTypeRepository;
            _genericPaginationService = genericPaginationService;
            _mapper = mapper;
        }

        public async Task<PagedResult<PurchaseDetail>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = _purchaseDetailRepository.GetAllQueryable();
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<PurchaseDetail> GetByIdAsync(string id)
        {
            var purchaseDetail = await _purchaseDetailRepository.GetByIdAsync(id);
            if (purchaseDetail == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id");
            }
            return purchaseDetail;
        }

        public async Task<PurchaseDetail> AddAsync(string receiptId, string productTypeId, CreatePurchaseDetailDTO createPurchaseDetailDTO)
        {
            var receipt = await _purchaseReceiptRepository.GetByIdAsync(receiptId);
            if (receipt == null)
            {
                throw new ArgumentException("Không tìm thấy PurchaseReicept Id");
            }
            var productType = await _productTypeRepository.GetByIdAsync(productTypeId);
            if (productType == null)
            {
                throw new ArgumentException("Không tìm thấy ProductType Id");
            }
            if (createPurchaseDetailDTO.Quanlity <= 0)
            {
                throw new ArgumentException("Quantity không được nhỏ hơn 0");
            }
            if (createPurchaseDetailDTO.Price <= 0)
            {
                throw new ArgumentException("Price không được nhỏ hơn 0");
            }
            if (string.IsNullOrWhiteSpace(createPurchaseDetailDTO.CreatedBy))
            {
                throw new ArgumentException("Người tạo không được để trống");
            }

            var purchaseDetail = _mapper.Map<PurchaseDetail>(createPurchaseDetailDTO);
            purchaseDetail.CreatedTime = DateTime.Now;
            purchaseDetail.ReceiptId = receiptId;
            purchaseDetail.ProductTypeId = productTypeId;
            await _purchaseDetailRepository.AddAsync(purchaseDetail);
            return purchaseDetail;
        }

        public async Task<PurchaseDetail> UpdateAsync(string purchaseDetailId, UpdatePurchaseDetailDTO updatePurchaseDetailDTO)
        {
            var purchaseDetail = await _purchaseDetailRepository.GetByIdAsync(purchaseDetailId);
            if (purchaseDetail == null)
            {
                throw new ArgumentException("Không thể tìm thấy PurchaseDetail với ID này");
            }

            if (updatePurchaseDetailDTO.Quanlity <= 0)
            {
                throw new ArgumentException("Quantity không được nhỏ hơn 0");
            }
            if (updatePurchaseDetailDTO.Price <= 0)
            {
                throw new ArgumentException("Price không được nhỏ hơn 0");
            }


            if (!string.IsNullOrWhiteSpace(updatePurchaseDetailDTO.ReceiptId))
            {
                var updateReceipt = await _purchaseReceiptRepository.GetByIdAsync(updatePurchaseDetailDTO.ReceiptId);
                if (updateReceipt == null)
                {
                    throw new ArgumentException("Không thể tìm thấy PurchaseReiceipt với ID này");
                }
                purchaseDetail.ReceiptId = updatePurchaseDetailDTO.ReceiptId;
            }
            if (!string.IsNullOrWhiteSpace(updatePurchaseDetailDTO.ProductTypeId))
            {
                var updateProductType = await _productTypeRepository.GetByIdAsync(updatePurchaseDetailDTO.ProductTypeId);
                if (updateProductType == null)
                {
                    throw new ArgumentException("Không thể tìm thấy ProductType với ID này");
                }
                purchaseDetail.ProductTypeId = updatePurchaseDetailDTO.ProductTypeId;
            }
            if (string.IsNullOrWhiteSpace(updatePurchaseDetailDTO.LastUpdatedBy))
            {
                throw new ArgumentException("Người sửa không được để trống");
            }

            purchaseDetail.LastUpdatedBy = updatePurchaseDetailDTO.LastUpdatedBy;
            purchaseDetail.LastUpdatedTime = DateTime.Now;

            await _purchaseDetailRepository.UpdateAsync(purchaseDetail);
            return purchaseDetail;
        }

        public async Task DeleteAsync(string Id, string deletedBy)
        {
            var purchaseDetail = await _purchaseDetailRepository.GetByIdAsync(Id);
            if (purchaseDetail == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id để Delete");
            }
            if (string.IsNullOrWhiteSpace(deletedBy))
            {
                throw new ArgumentException("Người xóa không được để trống");
            }
            purchaseDetail.DeletedBy = deletedBy;
            purchaseDetail.DeletedTime = DateTime.Now;
            purchaseDetail.IsDeleted = true;
            await _purchaseDetailRepository.UpdateAsync(purchaseDetail);
        }
    }
}
