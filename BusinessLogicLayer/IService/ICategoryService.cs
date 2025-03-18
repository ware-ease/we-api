using BusinessLogicLayer.Generic;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.Models.Category;
using BusinessLogicLayer.Models.Pagination;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface ICategoryService : IGenericService
    {
        Task<int> Count();
        Task<CategoryDTO> UpdateCategory(CategoryUpdateDTO request);
        /*Task<PagedResult<Category>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<Category> GetByIdAsync(string id);
        Task AddAsync(Category category);
        Task UpdateAsync(string Id, Category category);
        Task DeleteAsync(string Id, Category category);*/
    }
}
