using Data.Entity;
using DataAccessLayer.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface IInventoryRepository : IGenericRepository<Inventory>
    {
        //Task<List<Inventory>> GetInventoriesByProductIdAsync(string productId);
        Task<List<Inventory>> GetAvailableInventoriesAsync(string productId, string warehouseId);
    }
}
