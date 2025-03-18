using BusinessLogicLayer.Generic;
using Data.Entity;
using Data.Entity.Base;
using Data.Model.DTO;
using Data.Model.DTO.Base;
using Data.Model.Request.Customer;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface ICustomerService : IGenericService
    {
        Task<ServiceResponse> Get<TResult>() where TResult : BaseDTO;
        Task<ServiceResponse> Add<TResult, TRequest>(TRequest request);
        Task<CustomerDTO> UpdateCustomer(CustomerUpdateDTO request);
    }
}
