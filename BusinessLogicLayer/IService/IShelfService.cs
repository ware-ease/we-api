using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.Shelf;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IShelfService
    {
        Task<PagedResult<Shelf>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<PagedResult<Shelf>> GetAllByWarehouseIdAsync(string warehouseId, int? pageNumber, int? pageSize);
        Task<Shelf> GetByIdAsync(string id);
        Task<Shelf> AddAsync(string warehouseId, CreateShelfDTO createShelfDTO);
        Task<Shelf> UpdateAsync(string shelfId, UpdateShelfDTO updateShelfDTO);
        Task DeleteAsync(string Id, string deletedBy);
    }
}
