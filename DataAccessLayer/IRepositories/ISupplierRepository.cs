using Data.Entity;
using DataAccessLayer.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface ISupplierRepository : IGenericRepository<Supplier>
    {
        /*IQueryable<Supplier> GetAllQueryable();
        Task<List<Supplier>> GetAllAsync(string supplierId);
        Task<Supplier> GetByIdAsync(string id);
        Task AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task DeleteAsync(Supplier supplier);*/

    }
}
