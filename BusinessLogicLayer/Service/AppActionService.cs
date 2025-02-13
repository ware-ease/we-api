using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.AppAction;
using Data.Entity;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class AppActionService : IAppActionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public AppActionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppActionDTO>> GetAllAsync()
        {
            var appactions = await _unitOfWork.AppActionRepository.Get();
            return _mapper.Map<IEnumerable<AppActionDTO>>(appactions);
        }

        public async Task<AppActionDTO> GetByIdAsync(string id)
        {
            var appaction = await _unitOfWork.AppActionRepository.GetByID(id);
            return _mapper.Map<AppActionDTO>(appaction);
        }

        public async Task<AppActionDTO> CreateAsync(CreateAppActionDTO actionCreate)
        {
            var action = _mapper.Map<AppAction>(actionCreate);
            action.CreatedTime = DateTime.Now;

            await _unitOfWork.AppActionRepository.Insert(action);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<AppActionDTO>(action);
        }

        public async Task<AppActionDTO> UpdateAsync(string id, CreateAppActionDTO action)
        {
            var existingAction = await _unitOfWork.AppActionRepository.GetByID(id);
            if (existingAction == null) return null;

            existingAction.Code = action.Code;
            existingAction.LastUpdatedTime = DateTime.Now;

            _unitOfWork.AppActionRepository.Update(existingAction);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<AppActionDTO>(existingAction);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var action = await _unitOfWork.AppActionRepository.GetByID(id);
            if (action == null) return false;

            action.IsDeleted = true;
            action.DeletedTime = DateTime.Now;

            _unitOfWork.AppActionRepository.Update(action);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}