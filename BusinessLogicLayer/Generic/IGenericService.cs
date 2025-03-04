using Data.Entity.Base;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Generic
{
    public interface IGenericService
    {
        Task<ServiceResponse> Add<TResult, TRequest>(TRequest request);
        Task<ServiceResponse> Get<TResult>();
        Task<ServiceResponse> Get<TResult>(string id);
        Task<ServiceResponse> Update<TResult, TRequest>(TRequest request);
        Task<ServiceResponse> Delete(string id);
    }
}
