using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface IFloorRepository
    {
        IQueryable<Floor> GetAllQueryable();
        IQueryable<Floor> GetFloorsByShelfIdQueryable(string shelfId);
        Task<List<Floor>> GetAllAsync();
        Task<Floor> GetByIdAsync(string id);
        Task AddAsync(Floor floor);
        Task UpdateAsync(Floor floor);
        Task DeleteAsync(Floor floor);
        Task<Floor> GetFloorByShelfIdAndNumberAsync(string shelfId, int number);
    }
}
