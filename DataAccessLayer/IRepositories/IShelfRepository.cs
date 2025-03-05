using Data.Entity;
using DataAccessLayer.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface IShelfRepository : IGenericRepository<Shelf>
    {
        IQueryable<Shelf> GetAllQueryable();
        IQueryable<Shelf> GetShelvesByWarehouseIdQueryable(string warehouseId);
        Task<List<Shelf>> GetAllAsync();
        Task<Shelf> GetByIdAsync(string id);
        Task AddAsync(Shelf shelf);
        Task UpdateAsync(Shelf shelf);
        Task DeleteAsync(Shelf shelf);
        Task<int> GetActiveShelfCountByWarehouseIdAsync(string warehouseId);
        Task<Shelf> GetShelfByWarehouseIdAndNumberAsync(string warehouseId, int number);
    }
}
