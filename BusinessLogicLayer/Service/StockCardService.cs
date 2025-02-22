using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Cell;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.StockCard;
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
    public class StockCardService : IStockCardService
    {
        private readonly IStockCardRepository _stockCardRepository;
        private readonly ICellRepository _cellRepository;
        private readonly IGenericPaginationService _genericPaginationService;
        private readonly IMapper _mapper;

        public StockCardService(IStockCardRepository stockCardRepository, ICellRepository cellRepository, IGenericPaginationService genericPaginationService, IMapper mapper)
        {
            _stockCardRepository = stockCardRepository;
            _cellRepository = cellRepository;
            _genericPaginationService = genericPaginationService;
            _mapper = mapper;
        }

        public async Task<PagedResult<StockCard>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = _stockCardRepository.GetAllQueryable();
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<PagedResult<StockCard>> GetAllStockCardByCellId(string cellId, int? pageNumber, int? pageSize)
        {
            var cell = await _cellRepository.GetByIdAsync(cellId);
            if (cell == null)
            {
                throw new ArgumentException("Không tìm thấy Cell Id");
            }
            var query = _stockCardRepository.GetQueryableByCellId(cellId);
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<StockCard> GetByIdAsync(string id)
        {
            var stockCard = await _stockCardRepository.GetByIdAsync(id);
            if (stockCard == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id");
            }
            return stockCard;
        }

        public async Task<StockCard> AddAsync(string cellId, CreateStockCardDTO createStockCardDTO)
        {
            var cell = await _cellRepository.GetByIdAsync(cellId);
            if (cell == null)
            {
                throw new ArgumentException("Không tìm thấy Cell Id");
            }
            if (cell.IsDeleted == true)
            {
                throw new ArgumentException("Cell này đã bị xóa");
            }
            if (string.IsNullOrWhiteSpace(createStockCardDTO.Name))
            {
                throw new ArgumentException("Name không được để trống");
            }
            if (string.IsNullOrWhiteSpace(createStockCardDTO.Unit))
            {
                throw new ArgumentException("Unit không được để trống");
            }
            if (createStockCardDTO.Date > DateTime.Now)
            {
                throw new ArgumentException("Date không được đặt ở tương lai");
            }
            if (string.IsNullOrWhiteSpace(createStockCardDTO.CreatedBy))
            {
                throw new ArgumentException("Người tạo không được để trống");
            }

            var stockCard = _mapper.Map<StockCard>(createStockCardDTO);
            stockCard.CreatedTime = DateTime.Now;
            stockCard.CellId = cellId;
            await _stockCardRepository.AddAsync(stockCard);
            return stockCard;
        }

        public async Task<StockCard> UpdateAsync(string stockCardId, UpdateStockCardDTO updateStockCardDTO)
        {
            var stockCard = await _stockCardRepository.GetByIdAsync(stockCardId);
            if (stockCard == null)
            {
                throw new ArgumentException("Không thể tìm thấy StockCard với ID này");
            }
            if (!string.IsNullOrWhiteSpace(updateStockCardDTO.Name))
            {
                stockCard.Name = updateStockCardDTO.Name;
            }
            if (!string.IsNullOrWhiteSpace(updateStockCardDTO.Unit))
            {
                stockCard.Unit = updateStockCardDTO.Unit;
            }
            if (updateStockCardDTO.Date > DateTime.Now)
            {
                throw new ArgumentException("Date không được đặt ở tương lai");
            }
            if (!string.IsNullOrWhiteSpace(updateStockCardDTO.CellId))
            {
                var updateCell = await _cellRepository.GetByIdAsync(updateStockCardDTO.CellId);
                if (updateCell == null)
                {
                    throw new ArgumentException("Không thể tìm thấy Cell với Id này");
                }
                if (updateCell.IsDeleted == true)
                {
                    throw new ArgumentException("Cell này đã bị xóa");
                }
                stockCard.CellId = updateStockCardDTO.CellId;
            }

            if (string.IsNullOrWhiteSpace(updateStockCardDTO.LastUpdatedBy))
            {
                throw new ArgumentException("Người sửa không được để trống");
            }

            stockCard.LastUpdatedBy = updateStockCardDTO.LastUpdatedBy;
            stockCard.LastUpdatedTime = DateTime.Now;

            await _stockCardRepository.UpdateAsync(stockCard);
            return stockCard;
        }

        public async Task DeleteAsync(string Id, string deletedBy)
        {
            var stockCard = await _stockCardRepository.GetByIdAsync(Id);
            if (stockCard == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id để Delete");
            }
            if (string.IsNullOrWhiteSpace(deletedBy))
            {
                throw new ArgumentException("Người xóa không được để trống");
            }
            stockCard.DeletedBy = deletedBy;
            stockCard.DeletedTime = DateTime.Now;
            stockCard.IsDeleted = true;
            await _stockCardRepository.UpdateAsync(stockCard);
        }
    }
}
