using BusinessLogicLayer.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IGenericPaginationService
    {
        Task<PagedResult<T>> GetPagedDataAsync<T>(
            IQueryable<T> query, int? pageNumber, int? pageSize) where T : class;
    }

}
