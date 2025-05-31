using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Category;
using BusinessLogicLayer.Models.Pagination;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.Batch;
using Data.Model.Request.Category;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class CategoryService : GenericService<Category>, ICategoryService
    {
        public CategoryService(IGenericRepository<Category> genericRepository, IMapper mapper, IUnitOfWork unitOfWork)
            : base(genericRepository, mapper, unitOfWork)
        {
        }

        public override async Task<ServiceResponse> Get<TResult>()
        {
            var categories = await _genericRepository.GetAllNoPaging();

            List<TResult> mappedResults = _mapper.Map<List<TResult>>(categories);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Đã lấy thành công!",
                Data = mappedResults
            };
        }

        public override async Task<ServiceResponse> Get<TResult>(string id)
        {
            var category = await _genericRepository.GetByCondition(b => b.Id == id);

            if (category == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Danh mục không tồn tại!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(category);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Đã lấy thành công!",
                Data = result
            };
        }

        public async Task<int> Count()
        {
            var batches = await _genericRepository.GetAllNoPaging();
            return batches.Count(b => !b.IsDeleted);
        }

        public async Task<CategoryDTO> UpdateCategory(CategoryUpdateDTO request)
        {
            var existingCategory = await _genericRepository.GetByCondition(p => p.Id == request.Id);
            if (existingCategory == null)
                throw new Exception("Danh mục không tồn tại");

            if (!string.IsNullOrEmpty(request.Name))
            {
                existingCategory.Name = request.Name;
            }
            if (!string.IsNullOrEmpty(request.Note))
            {
                existingCategory.Note = request.Note;
            }

            _genericRepository.Update(existingCategory);
            await _unitOfWork.SaveAsync();

            var updatedCategory = await _genericRepository.GetByCondition(p => p.Id == existingCategory.Id);
            if (updatedCategory == null)
                throw new Exception("Cập nhật thất bại, danh mục không thể tìm thấy sau cập nhật");

            return _mapper.Map<CategoryDTO>(updatedCategory);
        }

        public async Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null)
        {

            Expression<Func<Category, bool>> filter = p =>
                (string.IsNullOrEmpty(keyword)
                || (p.Name + " " + p.Note).Contains(keyword));

            var totalRecords = await _genericRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize);

            var mappedResults = _mapper.Map<IEnumerable<TResult>>(results);

            int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? totalRecords));

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Tìm kiếm thành công!",
                Data = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageIndex = pageIndex ?? 1,
                    PageSize = pageSize ?? totalRecords,
                    Records = mappedResults
                }
            };
        }

        /*public async Task<PagedResult<Category>> GetAllAsync(int? pageNumber, int? pageSize)
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
        }*/
    }
}
