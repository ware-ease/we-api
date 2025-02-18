using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface IProductTypeRepository
    {
        IQueryable<ProductType> GetAllQueryable();
        Task<ProductType> GetByIdAsync(string id);
        Task<List<ProductType>> GetAllAsync();
        Task AddAsync(ProductType productType);
        Task UpdateAsync(ProductType productType);
        Task DeleteAsync(string id);
    }
}
