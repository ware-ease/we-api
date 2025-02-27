using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.StockCard;
using BusinessLogicLayer.Models.StockCardDetail;
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
    public class StockCardDetailService : IStockCardDetailService
    {
        private readonly IStockCardDetailRepository _stockCardDetailRepository;
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IStockCardRepository _stockCardRepository;
        private readonly IGenericPaginationService _genericPaginationService;
        private readonly IMapper _mapper;

        public StockCardDetailService(IStockCardDetailRepository stockCardDetailRepository, 
            IProductTypeRepository productTypeRepository, 
            IStockCardRepository stockCardRepository, 
            IGenericPaginationService genericPaginationService, 
            IMapper mapper)
        {
            _stockCardDetailRepository = stockCardDetailRepository;
            _productTypeRepository = productTypeRepository;
            _stockCardRepository = stockCardRepository;
            _genericPaginationService = genericPaginationService;
            _mapper = mapper;
        }

        public async Task<PagedResult<StockCardDetail>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = _stockCardDetailRepository.GetAllQueryable();
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<PagedResult<StockCardDetail>> GetQueryableByStockCardId(string stockCardId, 
            int? pageNumber, int? pageSize)
        {
            var stockCard = await _stockCardRepository.GetByIdAsync(stockCardId);
            if (stockCard == null)
            {
                throw new ArgumentException("Không tìm thấy StockCard Id");
            }
            var query = _stockCardDetailRepository.GetQueryableByStockCardId(stockCardId);
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<PagedResult<StockCardDetail>> GetQueryableByProductTypeId(string productTypeId, 
            int? pageNumber, int? pageSize)
        {
            var productType = await _productTypeRepository.GetByIdAsync(productTypeId);
            if (productType == null)
            {
                throw new ArgumentException("Không tìm thấy ProductType Id");
            }
            var query = _stockCardDetailRepository.GetQueryableByStockCardId(productTypeId);
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<StockCardDetail> GetByIdAsync(string stockCardId, string productTypeId)
        {
            var stockCard = await _stockCardRepository.GetByIdAsync(stockCardId);
            if (stockCard == null)
            {
                throw new ArgumentException("Không tìm thấy StockCard Id");
            }
            var productType = await _productTypeRepository.GetByIdAsync(productTypeId);
            if (productType == null)
            {
                throw new ArgumentException("Không tìm thấy ProductType Id");
            }

            var stockCardDetail = await _stockCardDetailRepository.GetByIdAsync(stockCardId, productTypeId);
            if (stockCardDetail == null)
            {
                throw new ArgumentException("Không thể tìm thấy StockCardDetail");
            }
            return stockCardDetail;
        }

        public async Task<StockCardDetail> AddAsync(string stockCardId, string productTypeId, 
            CreateStockCardDetailDTO createStockCardDetailDTO)
        {
            //===================================================================//
            var stockCard = await _stockCardRepository.GetByIdAsync(stockCardId);
            if (stockCard == null)
            {
                throw new ArgumentException("Không tìm thấy StockCard Id");
            }
            if (stockCard.IsDeleted == true)
            {
                throw new ArgumentException("StockCard này đã bị xóa");
            }
            var productType = await _productTypeRepository.GetByIdAsync(productTypeId);
            if (productType == null)
            {
                throw new ArgumentException("Không tìm thấy ProductType Id");
            }
            if (productType.IsDeleted == true)
            {
                throw new ArgumentException("ProductType này đã bị xóa");
            }
            //===================================================================//



            if (string.IsNullOrWhiteSpace(createStockCardDetailDTO.Stock))
            {
                throw new ArgumentException("Stock không được để trống");
            }
            if (createStockCardDetailDTO.Date > DateTime.Now)
            {
                throw new ArgumentException("Date không được đặt ở tương lai");
            }
            if (string.IsNullOrWhiteSpace(createStockCardDetailDTO.CreatedBy))
            {
                throw new ArgumentException("Người tạo không được để trống");
            }

            var stockCardDetail = _mapper.Map<StockCardDetail>(createStockCardDetailDTO);
            //stockCardDetail.In = createStockCardDetailDTO.In;
            //stockCardDetail.Out = createStockCardDetailDTO.Out;
            //stockCardDetail.CreatedTime = DateTime.Now;
            //stockCardDetail.StockCardId = stockCardId;
            //stockCardDetail.ProductTypeId = productTypeId;
            await _stockCardDetailRepository.AddAsync(stockCardDetail);
            return stockCardDetail;
        }

        public async Task<StockCardDetail> UpdateAsync(string stockCardId, string productTypeId, 
            UpdateStockCardDetailDTO updateStockCardDetailDTO)
        {
            //===================================================================//
            var stockCard = await _stockCardRepository.GetByIdAsync(stockCardId);
            if (stockCard == null)
            {
                throw new ArgumentException("Không tìm thấy StockCard Id");
            }
            if (stockCard.IsDeleted == true)
            {
                throw new ArgumentException("StockCard này đã bị xóa");
            }
            var productType = await _productTypeRepository.GetByIdAsync(productTypeId);
            if (productType == null)
            {
                throw new ArgumentException("Không tìm thấy ProductType Id");
            }
            if (productType.IsDeleted == true)
            {
                throw new ArgumentException("ProductType này đã bị xóa");
            }
            var stockCardDetail = await _stockCardDetailRepository.GetByIdAsync(stockCardId, productTypeId);
            if (stockCardDetail == null)
            {
                throw new ArgumentException("Không thể tìm thấy StockCardDetail");
            }
            //===================================================================//




            //===================================================================//
            if (updateStockCardDetailDTO.Date > DateTime.Now)
            {
                throw new ArgumentException("Date không được đặt ở tương lai");
            }
            /*if (!string.IsNullOrWhiteSpace(updateStockCardDetailDTO.StockCardId))
            {
                var updateStockCard = await _stockCardRepository.GetByIdAsync(updateStockCardDetailDTO.StockCardId);
                if (updateStockCard == null)
                {
                    throw new ArgumentException("Không thể tìm thấy StockCard với Id này");
                }
                if (updateStockCard.IsDeleted == true)
                {
                    throw new ArgumentException("StockCard này đã bị xóa");
                }
                stockCardDetail.StockCardId = updateStockCardDetailDTO.StockCardId;
            }
            if (!string.IsNullOrWhiteSpace(updateStockCardDetailDTO.ProductTypeId))
            {
                var updateProductType = await _productTypeRepository.GetByIdAsync(updateStockCardDetailDTO.ProductTypeId);
                if (updateProductType == null)
                {
                    throw new ArgumentException("Không thể tìm thấy ProductType với Id này");
                }
                if (updateProductType.IsDeleted == true)
                {
                    throw new ArgumentException("ProductType này đã bị xóa");
                }
                stockCardDetail.ProductTypeId = updateStockCardDetailDTO.ProductTypeId;
            }*/
            //===================================================================//




            if (string.IsNullOrWhiteSpace(updateStockCardDetailDTO.LastUpdatedBy))
            {
                throw new ArgumentException("Người sửa không được để trống");
            }

            //stockCardDetail.In = updateStockCardDetailDTO.In;
            //stockCardDetail.Out = updateStockCardDetailDTO.Out;
            stockCardDetail.LastUpdatedBy = updateStockCardDetailDTO.LastUpdatedBy;
            stockCardDetail.LastUpdatedTime = DateTime.Now;

            await _stockCardDetailRepository.UpdateAsync(stockCardDetail);
            return stockCardDetail;
        }

        public async Task DeleteAsync(string stockCardId, string productTypeId, string deletedBy)
        {
            var stockCard = await _stockCardRepository.GetByIdAsync(stockCardId);
            if (stockCard == null)
            {
                throw new ArgumentException("Không tìm thấy StockCard Id");
            }
            var productType = await _productTypeRepository.GetByIdAsync(productTypeId);
            if (productType == null)
            {
                throw new ArgumentException("Không tìm thấy ProductType Id");
            }
            var stockCardDetail = await _stockCardDetailRepository.GetByIdAsync(stockCardId, productTypeId);
            if (stockCardDetail == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id để Delete");
            }
            if (stockCardDetail.IsDeleted == true)
            {
                throw new ArgumentException($"StockCardDetail đã từng bị xóa bởi {stockCardDetail.DeletedBy}");
            }
            if (string.IsNullOrWhiteSpace(deletedBy))
            {
                throw new ArgumentException("Người xóa không được để trống");
            }
            stockCardDetail.DeletedBy = deletedBy;
            stockCardDetail.DeletedTime = DateTime.Now;
            stockCardDetail.IsDeleted = true;
            await _stockCardDetailRepository.UpdateAsync(stockCardDetail);
        }
    }
}
