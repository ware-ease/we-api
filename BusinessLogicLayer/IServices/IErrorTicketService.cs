using BusinessLogicLayer.Generic;
using Data.Model.DTO;
using Data.Model.Request.ErrorTicket;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IErrorTicketService : IGenericService
    {
        Task<ErrorTicketDTO> AddErrorTicket(ErrorTicketCreateDTO request);
        Task<ErrorTicketDTO> UpdateErrorTicket(ErrorTicketUpdateDTO request);
        Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null);
    }
}
