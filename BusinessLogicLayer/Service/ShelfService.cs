using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.PurchaseDetail;
using BusinessLogicLayer.Models.Shelf;
using Data.Entity;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class ShelfService : IShelfService
    {
        private readonly IShelfRepository _shelfRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericPaginationService _genericPaginationService;
        private readonly IMapper _mapper;

        public ShelfService(IShelfRepository shelfRepository, IUnitOfWork unitOfWork, IGenericPaginationService genericPaginationService, IMapper mapper)
        {
            _shelfRepository = shelfRepository;
            _unitOfWork = unitOfWork;
            _genericPaginationService = genericPaginationService;
            _mapper = mapper;
        }

        public async Task<PagedResult<Shelf>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = _shelfRepository.GetAllQueryable();
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<PagedResult<Shelf>> GetAllByWarehouseIdAsync(string warehouseId, int? pageNumber, int? pageSize)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByID(warehouseId);
            if (warehouse == null)
            {
                throw new ArgumentException("Không tìm thấy Warehouse Id");
            }
            var query = _shelfRepository.GetShelvesByWarehouseIdQueryable(warehouseId);
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<Shelf> GetByIdAsync(string id)
        {
            var shelf = await _shelfRepository.GetByIdAsync(id);
            if (shelf == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id");
            }
            return shelf;
        }

        public async Task<Shelf> AddAsync(string warehouseId, CreateShelfDTO createShelfDTO)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByID(warehouseId);
            if (warehouse == null)
            {
                throw new ArgumentException("Không tìm thấy Warehouse Id");
            }
            if (warehouse.IsDeleted == true)
            {
                throw new ArgumentException("Warehouse này đã bị xóa");
            }
            var shelfNumber = await _shelfRepository.GetShelfByWarehouseIdAndNumberAsync(warehouseId, createShelfDTO.Number);
            if (shelfNumber != null)
            {
                throw new ArgumentException($"Shelf có Number là {createShelfDTO.Number} đã tồn tại trong Warehouse {warehouse.Name}");
            }
            var activeShelfCount = await _shelfRepository.GetActiveShelfCountByWarehouseIdAsync(warehouseId);
            if (activeShelfCount >= warehouse.ShelfCount)
            {
                throw new ArgumentException("Số lượng Shelf đang vượt quá tổng số Shelf cho phép trong Warehouse");
            }

            var shelf = _mapper.Map<Shelf>(createShelfDTO);
            shelf.CreatedTime = DateTime.Now;
            //shelf.WarehouseId = warehouseId;
            await _shelfRepository.AddAsync(shelf);
            return shelf;
        }

        public async Task<Shelf> UpdateAsync(string shelfId, UpdateShelfDTO updateShelfDTO)
        {
            //var shelf = await _shelfRepository.GetByIdAsync(shelfId);
            //if (shelf == null)
            //{
            //    throw new ArgumentException("Không thể tìm thấy Shelf với ID này");
            //}

            //if (updateShelfDTO.Number != shelf.Number)
            //{
            //    var warehouse = await _unitOfWork.WarehouseRepository.GetByID(shelf.WarehouseId);
            //    var shelfNumber = await _shelfRepository.GetShelfByWarehouseIdAndNumberAsync(shelf.WarehouseId, updateShelfDTO.Number);
            //    if (shelfNumber != null)
            //    {
            //        throw new ArgumentException($"Shelf có Number là {updateShelfDTO.Number} đã tồn tại trong Warehouse {warehouse.Name}");
            //    }
            //    shelf.Number = updateShelfDTO.Number;
            //}

            //if (!string.IsNullOrWhiteSpace(updateShelfDTO.WarehouseId))
            //{
            //    var updateWarehouse = await _unitOfWork.WarehouseRepository.GetByID(updateShelfDTO.WarehouseId);
            //    if (updateWarehouse == null)
            //    {
            //        throw new ArgumentException("Không thể tìm thấy Warehouse với ID này");
            //    }
            //    if (updateWarehouse.IsDeleted == true)
            //    {
            //        throw new ArgumentException("Warehouse này đã bị xóa");
            //    }
            //    shelf.WarehouseId = updateShelfDTO.WarehouseId;
            //}

            //if (string.IsNullOrWhiteSpace(updateShelfDTO.LastUpdatedBy))
            //{
            //    throw new ArgumentException("Người sửa không được để trống");
            //}

            //shelf.LastUpdatedBy = updateShelfDTO.LastUpdatedBy;
            //shelf.LastUpdatedTime = DateTime.Now;

            //await _shelfRepository.UpdateAsync(shelf);
            //return shelf;

            return null;
        }

        public async Task DeleteAsync(string Id, string deletedBy)
        {
            var shelf = await _shelfRepository.GetByIdAsync(Id);
            if (shelf == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id để Delete");
            }
            if (string.IsNullOrWhiteSpace(deletedBy))
            {
                throw new ArgumentException("Người xóa không được để trống");
            }
            shelf.DeletedBy = deletedBy;
            shelf.DeletedTime = DateTime.Now;
            shelf.IsDeleted = true;
            await _shelfRepository.UpdateAsync(shelf);
        }
    }
}
