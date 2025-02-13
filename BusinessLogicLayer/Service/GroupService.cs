using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.AppAction;
using BusinessLogicLayer.Models.Group;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Utils;
using Data.Entity;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<PageEntity<GroupDTO>?> GetAllAsync(int? pageIndex, int? pageSize)
        {
            // Sắp xếp theo thời gian tạo mới nhất
            Func<IQueryable<Group>, IOrderedQueryable<Group>> orderBy = q => q.OrderByDescending(x => x.CreatedTime);

            // Lấy danh sách có phân trang
            var entities = await _unitOfWork.GroupRepository
                .Get(filter: null, orderBy: orderBy, pageIndex: pageIndex, pageSize: pageSize);

            // Khởi tạo đối tượng PageEntity
            var pagin = new PageEntity<GroupDTO>();
            pagin.List = _mapper.Map<IEnumerable<GroupDTO>>(entities).ToList();

            // Đếm tổng số bản ghi
            pagin.TotalRecord = await _unitOfWork.GroupRepository.Count(null);

            // Tính tổng số trang
            pagin.TotalPage = PaginHelper.PageCount(pagin.TotalRecord, pageSize ?? 10);

            return pagin;
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
            group.CreatedBy = groupDto.CreatedBy;

            await _unitOfWork.GroupRepository.Insert(group);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<GroupDTO>(group);
        }

        public async Task<GroupDTO?> UpdateGroupAsync(string id, UpdateGroupDTO groupDto)
        {
            var group = await _unitOfWork.GroupRepository.GetByID(id);
            if (group == null) return null;

            group.Name = groupDto.Name;
            group.LastUpdatedTime = DateTime.Now;
            group.LastUpdatedBy = groupDto.LastUpdatedBy;

            _unitOfWork.GroupRepository.Update(group);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<GroupDTO>(group);
        }

        public async Task<bool> DeleteGroupAsync(string id, string deleteBy)
        {
            var group = await _unitOfWork.GroupRepository.GetByID(id);
            if (group == null) return false;

            group.IsDeleted = true;
            group.DeletedTime = System.DateTime.Now;
            group.DeletedBy = deleteBy;

            _unitOfWork.GroupRepository.Update(group);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<PageEntity<GroupDTO>?> SearchAsync(string? searchKey, int? pageIndex, int? pageSize)
        {
            Expression<Func<Group, bool>> filter = x =>
                string.IsNullOrEmpty(searchKey) ||
                x.Name.ToLower().Contains(searchKey.ToLower());

            // Sắp xếp theo thời gian tạo mới nhất
            Func<IQueryable<Group>, IOrderedQueryable<Group>> orderBy = q => q.OrderByDescending(x => x.CreatedTime);

            // Lấy danh sách có phân trang
            var entities = await _unitOfWork.GroupRepository
                .Get(filter: filter, orderBy: orderBy, pageIndex: pageIndex, pageSize: pageSize);

            // Khởi tạo đối tượng PageEntity
            var pagin = new PageEntity<GroupDTO>();
            pagin.List = _mapper.Map<IEnumerable<GroupDTO>>(entities).ToList();

            // Đếm tổng số bản ghi khớp với bộ lọc
            pagin.TotalRecord = await _unitOfWork.GroupRepository.Count(filter);

            // Tính tổng số trang
            pagin.TotalPage = PaginHelper.PageCount(pagin.TotalRecord, pageSize ?? 10);

            return pagin;
        }
    }
}
