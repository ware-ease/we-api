using BusinessLogicLayer.Models.Cell;
using BusinessLogicLayer.Models.Pagination;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface ICellService
    {
        Task<PagedResult<Cell>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<PagedResult<Cell>> GetAllByFloorIdAsync(string shelfId, int? pageNumber, int? pageSize);
        Task<Cell> GetByIdAsync(string id);
        Task<Cell> AddAsync(string floorId, CreateCellDTO createCellDTO);
        Task<Cell> UpdateAsync(string cellId, UpdateCellDTO updateCellDTO);
        Task DeleteAsync(string Id, string deletedBy);
    }
}
