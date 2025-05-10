using BusinessLogicLayer.Generic;
using Data.Model.Request.Partner;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IPartnerService : IGenericService
    {
        Task<ServiceResponse> GetAll<TResult>();
        Task<ServiceResponse> GetById<TResult>(string id);
        Task<ServiceResponse> CreateAsync<TResult>(PartnerCreateDTO request);
        Task<ServiceResponse> UpdateAsync<TResult>(string id, PartnerUpdateDTO request);
        Task<ServiceResponse> DeleteAsync(string id);
        Task<ServiceResponse> SearchPartners(int? pageIndex = null, int? pageSize = null,
                                                      string? keyword = null, int? partnerType = null);
    }
}
