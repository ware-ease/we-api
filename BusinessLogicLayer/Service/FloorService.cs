using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Floor;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.PurchaseDetail;
using BusinessLogicLayer.Models.Shelf;
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
    public class FloorService : IFloorService
    {
        private readonly IFloorRepository _floorRepository;
        private readonly IShelfRepository _shelfRepository;
        private readonly IGenericPaginationService _genericPaginationService;
        private readonly IMapper _mapper;

        public FloorService(IFloorRepository floorRepository, IShelfRepository shelfRepository, IGenericPaginationService genericPaginationService, IMapper mapper)
        {
            _floorRepository = floorRepository;
            _shelfRepository = shelfRepository;
            _genericPaginationService = genericPaginationService;
            _mapper = mapper;
        }

        public async Task<PagedResult<Floor>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = _floorRepository.GetAllQueryable();
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<PagedResult<Floor>> GetAllByShelfIdAsync(string shelfId,int? pageNumber, int? pageSize)
        {
            var shelf = await _shelfRepository.GetByIdAsync(shelfId);
            if (shelf == null)
            {
                throw new ArgumentException("Không tìm thấy Shelf Id");
            }
            var query = _floorRepository.GetFloorsByShelfIdQueryable(shelfId);
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<Floor> GetByIdAsync(string id)
        {
            var floor = await _floorRepository.GetByIdAsync(id);
            if (floor == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id");
            }
            return floor;
        }

        public async Task<Floor> AddAsync(string shelfId, CreateFloorDTO createFloorDTO)
        {
            var shelf = await _shelfRepository.GetByIdAsync(shelfId);
            if (shelf == null)
            {
                throw new ArgumentException("Không tìm thấy Shelf Id");
            }
            if (shelf.IsDeleted == true)
            {
                throw new ArgumentException("Shelf này đã bị xóa");
            }
            var floorNumber = await _floorRepository.GetFloorByShelfIdAndNumberAsync(shelfId, createFloorDTO.Number);
            if (floorNumber != null)
            {
                throw new ArgumentException($"Floor có Number là {createFloorDTO.Number} với cùng ShelfId đã tồn tại");
            }
            if (string.IsNullOrWhiteSpace(createFloorDTO.CreatedBy))
            {
                throw new ArgumentException("Người tạo không được để trống");
            }

            var floor = _mapper.Map<Floor>(createFloorDTO);
            floor.CreatedTime = DateTime.Now;
            floor.ShelfId = shelfId;
            await _floorRepository.AddAsync(floor);
            return floor;
        }

        public async Task<Floor> UpdateAsync(string floorId, UpdateFloorDTO updateFloorDTO)
        {
            var floor = await _floorRepository.GetByIdAsync(floorId);
            if (floor == null)
            {
                throw new ArgumentException("Không thể tìm thấy Floor với ID này");
            }

            if (updateFloorDTO.Number != floor.Number)
            {
                var shelf = await _shelfRepository.GetByIdAsync(floor.ShelfId);
                var floorNumber = await _floorRepository.GetFloorByShelfIdAndNumberAsync(floor.ShelfId, updateFloorDTO.Number);
                if (floorNumber != null)
                {
                    throw new ArgumentException($"Floor có Number là {updateFloorDTO.Number} với cùng ShelfId đã tồn tại");
                }
                floor.Number = updateFloorDTO.Number;
            }

            if (!string.IsNullOrWhiteSpace(updateFloorDTO.ShelfId))
            {
                var updateShelf = await _shelfRepository.GetByIdAsync(updateFloorDTO.ShelfId);
                if (updateShelf == null)
                {
                    throw new ArgumentException("Không thể tìm thấy Shelf với ID này");
                }
                if (updateShelf.IsDeleted == true)
                {
                    throw new ArgumentException("Shelf này đã bị xóa");
                }
                floor.ShelfId = updateFloorDTO.ShelfId;
            }

            if (string.IsNullOrWhiteSpace(updateFloorDTO.LastUpdatedBy))
            {
                throw new ArgumentException("Người sửa không được để trống");
            }

            floor.LastUpdatedBy = updateFloorDTO.LastUpdatedBy;
            floor.LastUpdatedTime = DateTime.Now;

            await _floorRepository.UpdateAsync(floor);
            return floor;
        }

        public async Task DeleteAsync(string Id, string deletedBy)
        {
            var floor = await _floorRepository.GetByIdAsync(Id);
            if (floor == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id để Delete");
            }
            if (string.IsNullOrWhiteSpace(deletedBy))
            {
                throw new ArgumentException("Người xóa không được để trống");
            }
            floor.DeletedBy = deletedBy;
            floor.DeletedTime = DateTime.Now;
            floor.IsDeleted = true;
            await _floorRepository.UpdateAsync(floor);
        }
    }
}
