using BusinessLogicLayer.Generic;
using Data.Model.Request.GoodRequest;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IGoodRequestService : IGenericService
    {
        Task<ServiceResponse> GetAll<TResult>();
        Task<ServiceResponse> GetById<TResult>(string id);
        Task<ServiceResponse> CreateAsync<TResult>(GoodRequestCreateDTO request);
        Task<ServiceResponse> UpdateAsync<TResult>(string id, GoodRequestUpdateDTO request);
    }
}
