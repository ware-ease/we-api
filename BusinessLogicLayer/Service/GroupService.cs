using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Group;
using Data.Entity;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GroupService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GroupDTO>> GetAllGroupsAsync()
        {
            var groups = await _unitOfWork.GroupRepository.Get();
            return _mapper.Map<IEnumerable<GroupDTO>>(groups);
        }

        public async Task<GroupDTO?> GetGroupByIdAsync(string id)
        {
            var group = await _unitOfWork.GroupRepository.GetByID(id);
            return group == null ? null : _mapper.Map<GroupDTO>(group);
        }

        public async Task<GroupDTO> CreateGroupAsync(CreateGroupDTO groupDto)
        {
            var group = _mapper.Map<Group>(groupDto);
            group.CreatedTime = DateTime.Now;
            //created by
            await _unitOfWork.GroupRepository.Insert(group);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<GroupDTO>(group);
        }

        public async Task<GroupDTO?> UpdateGroupAsync(string id, CreateGroupDTO groupDto)
        {
            var group = await _unitOfWork.GroupRepository.GetByID(id);
            if (group == null) return null;

            group.Name = groupDto.Name;
            group.LastUpdatedTime = DateTime.Now;

            _unitOfWork.GroupRepository.Update(group);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<GroupDTO>(group);
        }

        public async Task<bool> DeleteGroupAsync(string id)
        {
            var group = await _unitOfWork.GroupRepository.GetByID(id);
            if (group == null) return false;

            group.IsDeleted = true;
            group.DeletedTime = System.DateTime.Now;

            _unitOfWork.GroupRepository.Update(group);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
