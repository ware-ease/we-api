using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Cell;
using BusinessLogicLayer.Models.Floor;
using BusinessLogicLayer.Models.Pagination;
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
    public class CellService : ICellService
    {
        private readonly ICellRepository _cellRepository;
        private readonly IFloorRepository _floorRepository;
        private readonly IGenericPaginationService _genericPaginationService;
        private readonly IMapper _mapper;

        public CellService(ICellRepository cellRepository, IFloorRepository floorRepository, IGenericPaginationService genericPaginationService, IMapper mapper)
        {
            _cellRepository = cellRepository;
            _floorRepository = floorRepository;
            _genericPaginationService = genericPaginationService;
            _mapper = mapper;
        }

        public async Task<PagedResult<Cell>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = _cellRepository.GetAllQueryable();
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<PagedResult<Cell>> GetAllByFloorIdAsync(string shelfId, int? pageNumber, int? pageSize)
        {
            var floor = await _floorRepository.GetByIdAsync(shelfId);
            if (floor == null)
            {
                throw new ArgumentException("Không tìm thấy Floor Id");
            }
            var query = _cellRepository.GetCellsByFloorId(shelfId);
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<Cell> GetByIdAsync(string id)
        {
            var cell = await _cellRepository.GetByIdAsync(id);
            if (cell == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id");
            }
            return cell;
        }

        public async Task<Cell> AddAsync(string floorId, CreateCellDTO createCellDTO)
        {
            var floor = await _floorRepository.GetByIdAsync(floorId);
            if (floor == null)
            {
                throw new ArgumentException("Không tìm thấy Floor Id");
            }
            if (floor.IsDeleted == true)
            {
                throw new ArgumentException("Floor này đã bị xóa");
            }
            var cellNumber = await _cellRepository.GetCellByFloorIdAndNumberAsync(floorId, createCellDTO.Number);
            if (cellNumber != null)
            {
                throw new ArgumentException($"Cell có Number là {createCellDTO.Number} với cùng FloorId đã tồn tại");
            }
            if (string.IsNullOrWhiteSpace(createCellDTO.CreatedBy))
            {
                throw new ArgumentException("Người tạo không được để trống");
            }

            var cell = _mapper.Map<Cell>(createCellDTO);
            cell.CreatedTime = DateTime.Now;
            cell.FloorId = floorId;
            await _cellRepository.AddAsync(cell);
            return cell;
        }

        public async Task<Cell> UpdateAsync(string cellId, UpdateCellDTO updateCellDTO)
        {
            var cell = await _cellRepository.GetByIdAsync(cellId);
            if (cell == null)
            {
                throw new ArgumentException("Không thể tìm thấy Cell với ID này");
            }

            if (updateCellDTO.Number != cell.Number)
            {
                var floor = await _floorRepository.GetByIdAsync(cell.FloorId);
                var cellNumber = await _cellRepository.GetCellByFloorIdAndNumberAsync(cell.FloorId, updateCellDTO.Number);
                if (cellNumber != null)
                {
                    throw new ArgumentException($"Cell có Number là {updateCellDTO.Number} với cùng FloorId đã tồn tại");
                }
                cell.Number = updateCellDTO.Number;
            }

            if (!string.IsNullOrWhiteSpace(updateCellDTO.FloorId))
            {
                var updateFloor = await _floorRepository.GetByIdAsync(updateCellDTO.FloorId);
                if (updateFloor == null)
                {
                    throw new ArgumentException("Không thể tìm thấy Floor với ID này");
                }
                if (updateFloor.IsDeleted == true)
                {
                    throw new ArgumentException("Floor này đã bị xóa");
                }
                cell.FloorId = updateCellDTO.FloorId;
            }

            if (string.IsNullOrWhiteSpace(updateCellDTO.LastUpdatedBy))
            {
                throw new ArgumentException("Người sửa không được để trống");
            }

            cell.LastUpdatedBy = updateCellDTO.LastUpdatedBy;
            cell.LastUpdatedTime = DateTime.Now;

            await _cellRepository.UpdateAsync(cell);
            return cell;
        }

        public async Task DeleteAsync(string Id, string deletedBy)
        {
            var cell = await _cellRepository.GetByIdAsync(Id);
            if (cell == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id để Delete");
            }
            if (string.IsNullOrWhiteSpace(deletedBy))
            {
                throw new ArgumentException("Người xóa không được để trống");
            }
            cell.DeletedBy = deletedBy;
            cell.DeletedTime = DateTime.Now;
            cell.IsDeleted = true;
            await _cellRepository.UpdateAsync(cell);
        }
    }
}
