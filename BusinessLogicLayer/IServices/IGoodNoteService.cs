using BusinessLogicLayer.Generic;
using Data.Enum;
using Data.Model.Request.GoodNote;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IGoodNoteService : IGenericService
    {
        Task<ServiceResponse> SearchGoodNotes<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                      string? keyword = null, GoodNoteEnum? goodNoteType = null,
                                                                                              GoodNoteStatusEnum? status = null,
                                                                                              string? requestedWarehouseId = null);
        Task<ServiceResponse> GetById<TResult>(string id);
        Task<ServiceResponse> CreateAsync<TResult>(GoodNoteCreateDTO request);
        Task<ServiceResponse> DeleteAsync(string id);
        Task<ServiceResponse> UpdateStatusAsync(string id, GoodNoteStatusEnum newStatus);
        Task<ServiceResponse> UpdateAsync(string id, GoodNoteUpdateDTO request);
    }
}
