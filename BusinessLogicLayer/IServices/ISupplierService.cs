using BusinessLogicLayer.Generic;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.Supplier;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.DTO.Base;
using Data.Model.Request.Supplier;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface ISupplierService : IGenericService
    {
        Task<ServiceResponse> Get<TResult>() where TResult : BaseDTO;
        Task<ServiceResponse> Add<TResult, TRequest>(TRequest request);
        Task<SupplierDTO> UpdateSupplier(SupplierUpdateDTO request);
        Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null);
        /*Task<PagedResult<Supplier>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<Supplier> GetByIdAsync(string id);
        Task<Supplier> AddAsync(CreateSupplierDTO supplierDTO);
        Task<Supplier> UpdateAsync(string id, UpdateSupplierDTO supplierDTO);
        Task DeleteAsync(string Id, string deletedBy);*/
    }
}