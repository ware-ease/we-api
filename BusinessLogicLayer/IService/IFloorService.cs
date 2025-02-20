using BusinessLogicLayer.Models.Floor;
using BusinessLogicLayer.Models.Pagination;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IFloorService
    {
        Task<PagedResult<Floor>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<PagedResult<Floor>> GetAllByShelfIdAsync(string shelfId, int? pageNumber, int? pageSize);
        Task<Floor> GetByIdAsync(string id);
        Task<Floor> AddAsync(string shelfId, CreateFloorDTO createFloorDTO);
        Task<Floor> UpdateAsync(string floorId, UpdateFloorDTO updateFloorDTO);
        Task DeleteAsync(string Id, string deletedBy);
    }
}
