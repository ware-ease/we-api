using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface IProductTypeTypeDetailRepository
    {
        IQueryable<ProductTypeTypeDetail> GetAllQueryable();
        Task<List<ProductTypeTypeDetail>> GetAllAsync();
        Task<ProductTypeTypeDetail> GetByIdAsync(string id);
        Task AddAsync(ProductTypeTypeDetail productTypeTypeDetail);
        Task UpdateAsync(ProductTypeTypeDetail productTypeTypeDetail);
        Task DeleteAsync(ProductTypeTypeDetail productTypeTypeDetail);
    }
}
