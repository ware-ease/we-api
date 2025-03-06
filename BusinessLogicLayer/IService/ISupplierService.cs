using BusinessLogicLayer.Generic;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.Supplier;
using Data.Entity;
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
        Task<ServiceResponse> Update<SupplierDTO, SupplierUpdateDTO>(SupplierUpdateDTO request);
        /*Task<PagedResult<Supplier>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<Supplier> GetByIdAsync(string id);
        Task<Supplier> AddAsync(CreateSupplierDTO supplierDTO);
        Task<Supplier> UpdateAsync(string id, UpdateSupplierDTO supplierDTO);
        Task DeleteAsync(string Id, string deletedBy);*/
    }
}
