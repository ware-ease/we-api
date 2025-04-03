using BusinessLogicLayer.Generic;
using Data.Enum;
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
        Task<ServiceResponse> GetById(string id);
        Task<ServiceResponse> CreateAsync<TResult>(GoodRequestCreateDTO request);
        Task<ServiceResponse> UpdateAsync(string id, GoodRequestUpdateDTO request);
        Task<ServiceResponse> SearchGoodRequests(int? pageIndex = null, int? pageSize = null,
                                                                       string? keyword = null, GoodRequestEnum? requestType = null,
                                                                                               GoodRequestStatusEnum? status = null);
        Task<ServiceResponse> UpdateStatusAsync(string id, GoodRequestStatusEnum newStatus);
    }
}
