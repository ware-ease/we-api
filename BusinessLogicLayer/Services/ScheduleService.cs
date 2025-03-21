using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.Product;
using Data.Model.Request.Schedule;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ScheduleService : GenericService<Schedule>, IScheduleService
    {
        private readonly ILocationRepository _locationRepository;
        public ScheduleService(IGenericRepository<Schedule> genericRepository,
            ILocationRepository locationRepository,
            IMapper mapper, IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            _locationRepository = locationRepository;
        }

        public async Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null)
        {

            Expression<Func<Schedule, bool>> filter = p =>
                (string.IsNullOrEmpty(keyword) || p.Location.Name.Contains(keyword));

            var totalRecords = await _genericRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize,
                includeProperties: "Location");

            var mappedResults = _mapper.Map<IEnumerable<TResult>>(results);

            int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? totalRecords));

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Search successful!",
                Data = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageIndex = pageIndex ?? 1,
                    PageSize = pageSize ?? totalRecords,
                    Records = mappedResults
                }
            };
        }

        public override async Task<ServiceResponse> Get<TResult>(string id)
        {
            var schedule = await _genericRepository.GetByCondition(
                p => p.Id == id,
                includeProperties: "Location"
            );

            if (schedule == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Schedule not found!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(schedule);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = result
            };
        }

        public async Task<ScheduleDTO> AddSchedule(ScheduleCreateDTO request)
        {
            if (request.Date < DateOnly.FromDateTime(DateTime.Now))
                throw new Exception("Không được đặt lịch ở quá khứ");

            var location = await _locationRepository.GetByCondition(p => p.Id == request.LocationId);
            if (location == null)
                throw new Exception("Location không tồn tại");

            var schedule = _mapper.Map<Schedule>(request);
            await _genericRepository.Insert(schedule);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<ScheduleDTO>(schedule);
        }

        public async Task<ScheduleDTO> UpdateSchedule(ScheduleUpdateDTO request)
        {
            var existingSchedule = await _genericRepository.GetByCondition(p => p.Id == request.Id);
            if (existingSchedule == null)
                throw new Exception("Schedule not found");

            if (request.Date.HasValue)
            {
                existingSchedule.Date = request.Date;
            }
            if (request.StartTime.HasValue)
            {
                existingSchedule.StartTime = request.StartTime;
            }
            if (request.EndTime.HasValue)
            {
                existingSchedule.EndTime = request.EndTime;
            }

            if (!string.IsNullOrEmpty(request.LocationId))
            {
                var location = await _locationRepository.GetByCondition(p => p.Id == request.LocationId);
                if (location == null)
                    throw new Exception("Location not found");
                existingSchedule.LocationId = request.LocationId;
            }

            _genericRepository.Update(existingSchedule);
            await _unitOfWork.SaveAsync();

            var updatedSchedule = await _genericRepository.GetByCondition(p => p.Id == existingSchedule.Id);
            if (updatedSchedule == null)
                throw new Exception("Update failed, schedule not found after update");

            return _mapper.Map<ScheduleDTO>(updatedSchedule);
        }
    }
}
