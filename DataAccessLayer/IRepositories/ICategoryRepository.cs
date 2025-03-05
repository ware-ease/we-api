using Data.Entity;
using DataAccessLayer.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        /*IQueryable<Category> GetAllQueryable();
        Task<Category> GetByIdAsync(string id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);*/

    }
}
