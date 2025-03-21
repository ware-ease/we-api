using BusinessLogicLayer.Generic;
using Data.Model.DTO;
using Data.Model.Request.Schedule;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IScheduleService : IGenericService
    {
        Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null);
        Task<ScheduleDTO> AddSchedule(ScheduleCreateDTO request);
        Task<ScheduleDTO> UpdateSchedule(ScheduleUpdateDTO request);
    }
}
