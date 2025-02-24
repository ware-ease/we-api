using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Group;
using BusinessLogicLayer.Models.GroupPermission;
using BusinessLogicLayer.Models.Permission;
using Data.Entity;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class GroupPermissionService : IGroupPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public GroupPermissionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<GroupPermissionDTO>> GetAllAsync()
        {
            var data = await _unitOfWork.GroupPermissionRepository.Get();
            return _mapper.Map<IEnumerable<GroupPermissionDTO>>(data);
        }

        public async Task<GroupPermissionDTO> GetByIdAsync(string groupId, string permissionId)
        {
            //var entity = await _unitOfWork.GroupPermissionRepository.GetByCondition(
            //    filter: x => x.GroupId == groupId && x.PermissionId == permissionId);
            //return _mapper.Map<GroupPermissionDTO>(entity);

            return null;
        }

        public async Task<GroupPermissionDTO> CreateAsync(CreateGroupPermissionDTO model)
        {
            //var existingEntity = await _unitOfWork.GroupPermissionRepository.GetByCondition(
            //    filter: x => x.GroupId == model.GroupId && x.PermissionId == model.PermissionId);

            //if (existingEntity != null)
            //{
            //    throw new InvalidOperationException("Quyền này đã được cấp cho nhóm này.");
            //}

            //var entity = _mapper.Map<GroupAction>(model);
            //entity.CreatedTime = DateTime.Now;
            //await _unitOfWork.GroupPermissionRepository.Insert(entity);
            //await _unitOfWork.SaveAsync();
            //return _mapper.Map<GroupPermissionDTO>(entity);

            return null;
        }

        public async Task<bool> DeleteAsync(string groupId, string permissionId)
        {
            //var entity = await _unitOfWork.GroupPermissionRepository.GetByCondition(
            //    filter: x => x.GroupId == groupId && x.PermissionId == permissionId);

            //if (entity == null) return false;

            //_unitOfWork.GroupPermissionRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<PermissionDTO>> GetPermissionsByGroupIdAsync(string groupId)
        {
            //var groupPermissions = await _unitOfWork.GroupPermissionRepository.Get(x => x.GroupId == groupId);
            //var permissionIds = groupPermissions.Select(x => x.PermissionId).ToList();

            //var permissions = await _unitOfWork.PermissionRepository.Get(p => permissionIds.Contains(p.Id));
            //return _mapper.Map<IEnumerable<PermissionDTO>>(permissions);

            return null;
        }

        public async Task<IEnumerable<GroupDTO>> GetGroupsByPermissionIdAsync(string permissionId)
        {
            //var groupPermissions = await _unitOfWork.GroupPermissionRepository.Get(x => x.PermissionId == permissionId);
            //var groupIds = groupPermissions.Select(x => x.GroupId).ToList();

            //var groups = await _unitOfWork.GroupRepository.Get(g => groupIds.Contains(g.Id));
            //return _mapper.Map<IEnumerable<GroupDTO>>(groups);

            return null;
        }
    }
}