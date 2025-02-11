using BusinessLogicLayer.IService;
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

        public GroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Group>> GetAllGroupsAsync()
        {
            return await _unitOfWork.GroupRepository.Get();
        }

        public async Task<Group> GetGroupByIdAsync(string id)
        {
            return await _unitOfWork.GroupRepository.GetByID(id);
        }

        public async Task<Group> CreateGroupAsync(Group group)
        {
            await _unitOfWork.GroupRepository.Insert(group);
            await _unitOfWork.SaveAsync();
            return group;
        }

        public async Task<Group> UpdateGroupAsync(string id, Group group)
        {
            var existingGroup = await _unitOfWork.GroupRepository.GetByID(id);
            if (existingGroup == null)
                return null;

            existingGroup.Name = group.Name;
            await _unitOfWork.SaveAsync();
            return existingGroup;
        }

        public async Task<bool> DeleteGroupAsync(string id)
        {
            var group = await _unitOfWork.GroupRepository.GetByID(id);
            if (group == null)
                return false;

            _unitOfWork.GroupRepository.Delete(group);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
