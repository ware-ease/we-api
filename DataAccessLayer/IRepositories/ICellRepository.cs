using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface ICellRepository
    {
        IQueryable<Cell> GetAllQueryable();
        IQueryable<Cell> GetCellsByFloorId(string floorId);
        Task<List<Cell>> GetAllAsync();
        Task<Cell> GetByIdAsync(string id);
        Task AddAsync(Cell cell);
        Task UpdateAsync(Cell cell);
        Task DeleteAsync(Cell cell);
        Task<Cell> GetCellByFloorIdAndNumberAsync(string floorId, int number);
    }
}
