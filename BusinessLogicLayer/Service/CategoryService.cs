using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Category;
using BusinessLogicLayer.Models.Pagination;
using Data.Entity;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IGenericPaginationService _paginationService;

        public CategoryService(ICategoryRepository repository, IGenericPaginationService paginationService)
        {
            _repository = repository;
            _paginationService = paginationService;
        }

        public async Task<PagedResult<Category>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = _repository.GetAllQueryable();
            return await _paginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<Category> GetByIdAsync(string id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id");
            }
            return category;
        }

        public async Task AddAsync(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ArgumentException("Tên Category không được để trống");
            }
            if (string.IsNullOrWhiteSpace(category.Note))
            {
                throw new ArgumentException("Note Category không được để trống");
            }
            if (string.IsNullOrWhiteSpace(category.CreatedBy))
            {
                throw new ArgumentException("Người tạo không được để trống");
            }
            await _repository.AddAsync(category);
        }

        public async Task UpdateAsync(string Id, Category category)
        {
            var categoryData = await _repository.GetByIdAsync(Id);
            if (categoryData == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id để Update");
            }
            if (!string.IsNullOrWhiteSpace(category.Name))
            {
                categoryData.Name = category.Name;
            }
            if (!string.IsNullOrWhiteSpace(category.Note))
            {
                categoryData.Note = category.Note;
            }
            if (string.IsNullOrWhiteSpace(category.LastUpdatedBy))
            {
                throw new ArgumentException("Người chỉnh sửa không được để trống");
            }
            categoryData.LastUpdatedBy = category.LastUpdatedBy;
            categoryData.LastUpdatedTime = DateTime.Now;
            await _repository.UpdateAsync(categoryData);
        }

        public async Task DeleteAsync(string Id, Category category)
        {
            var categoryData = await _repository.GetByIdAsync(Id);
            if (categoryData == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id để Delete");
            }
            if (string.IsNullOrWhiteSpace(category.DeletedBy))
            {
                throw new ArgumentException("Người xóa không được để trống");
            }
            categoryData.DeletedBy = category.DeletedBy;
            categoryData.DeletedTime = DateTime.Now;
            categoryData.IsDeleted = true;
            await _repository.UpdateAsync(categoryData);
        }
    }
}
