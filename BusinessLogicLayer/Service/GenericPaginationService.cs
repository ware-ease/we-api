using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class GenericPaginationService : IGenericPaginationService
    {
        public async Task<PagedResult<T>> GetPagedDataAsync<T>(
        IQueryable<T> query, int? pageNumber, int? pageSize) where T : class
        {
            int totalCount = await query.CountAsync(); // Đếm tổng số bản ghi

            if (!pageNumber.HasValue || !pageSize.HasValue) // Nếu không có phân trang
            {
                return new PagedResult<T>
                {
                    Items = await query.ToListAsync(),
                    TotalCount = totalCount,
                    PageNumber = 1,
                    PageSize = totalCount
                };
            }

            int page = pageNumber.Value;
            int size = pageSize.Value;

            List<T> items = await query
                .Skip((page - 1) * size) // Bỏ qua các bản ghi của trang trước
                .Take(size) // Lấy đúng số lượng cần thiết
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = size
            };
        }

    }

}
