using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.AppAction;
using BusinessLogicLayer.Models.Permission;
using BusinessLogicLayer.Models.PermissionAction;
using Data.Entity;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class PermissionActionService : IPermissionActionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public PermissionActionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<PermissionActionDTO>> GetAllAsync()
        {
            //var data = await _unitOfWork.PermissionActionRepository.Get();
            //return _mapper.Map<IEnumerable<PermissionActionDTO>>(data);

            return null;
        }

        public async Task<PermissionActionDTO> GetByIdAsync(string permissionId, string actionId)
        {
            //var entity = await _unitOfWork.PermissionActionRepository.GetByCondition(
            //    filter: x => x.PermissionId == permissionId && x.ActionId == actionId);
            //return _mapper.Map<PermissionActionDTO>(entity);

            return null;
        }

        public async Task<PermissionActionDTO> CreateAsync(CreatePermissionActionDTO model)
        {
            // Kiểm tra xem action đã được gán cho permission hay chưa
            //var existingEntity = await _unitOfWork.PermissionActionRepository.GetByCondition(
            //    filter: x => x.PermissionId == model.PermissionId && x.ActionId == model.ActionId);

            //if (existingEntity != null)
            //{
            //    throw new InvalidOperationException("Action này đã được gán cho quyền này.");
            //}

            //var entity = _mapper.Map<PermissionAction>(model);
            //entity.CreatedTime = DateTime.Now;
            //await _unitOfWork.PermissionActionRepository.Insert(entity);
            //await _unitOfWork.SaveAsync();
            //return _mapper.Map<PermissionActionDTO>(entity);

            return null;
        }

        public async Task<bool> DeleteAsync(string permissionId, string actionId)
        {
            //var entity = await _unitOfWork.PermissionActionRepository.GetByCondition(
            //    filter: x => x.PermissionId == permissionId && x.ActionId == actionId);

            //if (entity == null) return false;

            //_unitOfWork.PermissionActionRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<AppActionDTO>> GetActionsByPermissionIdAsync(string permissionId)
        {
            //var permissionActions = await _unitOfWork.PermissionActionRepository.Get(x => x.PermissionId == permissionId);
            //var actionIds = permissionActions.Select(x => x.ActionId).ToList();

            //var actions = await _unitOfWork.AppActionRepository.Get(a => actionIds.Contains(a.Id));
            //return _mapper.Map<IEnumerable<AppActionDTO>>(actions);

            return null;
        }
        public async Task<IEnumerable<PermissionDTO>> GetPermissionsByActionIdAsync(string actionId)
        {
            //var permissionActions = await _unitOfWork.PermissionActionRepository.Get(x => x.ActionId == actionId);
            //var permissionIds = permissionActions.Select(x => x.PermissionId).ToList();

            //var permissions = await _unitOfWork.PermissionRepository.Get(a => permissionIds.Contains(a.Id));
            //return _mapper.Map<IEnumerable<PermissionDTO>>(permissions);

            return null;
        }

    }
}