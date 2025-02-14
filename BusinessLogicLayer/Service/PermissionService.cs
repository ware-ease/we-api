using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.Permission;
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
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PermissionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PageEntity<PermissionDTO>?> GetAllAsync(int? pageIndex, int? pageSize)
        {
            // Sắp xếp theo thời gian tạo mới nhất
            Func<IQueryable<Permission>, IOrderedQueryable<Permission>> orderBy = q => q.OrderByDescending(x => x.CreatedTime);

            // Lấy danh sách có phân trang
            var entities = await _unitOfWork.PermissionRepository
                .Get(filter: null, orderBy: orderBy, pageIndex: pageIndex, pageSize: pageSize);

            // Khởi tạo đối tượng PageEntity
            var pagin = new PageEntity<PermissionDTO>
            {
                List = _mapper.Map<IEnumerable<PermissionDTO>>(entities).ToList(),
                TotalRecord = await _unitOfWork.PermissionRepository.Count(null),
                TotalPage = PaginHelper.PageCount(await _unitOfWork.PermissionRepository.Count(null), pageSize ?? 10)
            };

            return pagin;
        }

        public async Task<PermissionDTO?> GetByIdAsync(string id)
        {
            var permission = await _unitOfWork.PermissionRepository.GetByID(id);
            return permission == null ? null : _mapper.Map<PermissionDTO>(permission);
        }

        public async Task<PermissionDTO> CreateAsync(CreatePermissionDTO model)
        {
            var permission = _mapper.Map<Permission>(model);
            permission.CreatedTime = DateTime.Now;
            permission.CreatedBy = model.CreatedBy;

            await _unitOfWork.PermissionRepository.Insert(permission);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<PermissionDTO>(permission);
        }

        public async Task<PermissionDTO?> UpdateAsync(string id, UpdatePermissionDTO model)
        {
            var permission = await _unitOfWork.PermissionRepository.GetByID(id);
            if (permission == null) return null;

            permission.Url = model.Url;
            permission.LastUpdatedTime = DateTime.Now;
            permission.LastUpdatedBy = model.LastUpdatedBy;

            _unitOfWork.PermissionRepository.Update(permission);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<PermissionDTO>(permission);
        }

        public async Task<bool> DeleteAsync(string id, string deleteBy)
        {
            var permission = await _unitOfWork.PermissionRepository.GetByID(id);
            if (permission == null) return false;

            permission.IsDeleted = true;
            permission.DeletedTime = DateTime.Now;
            permission.DeletedBy = deleteBy;

            _unitOfWork.PermissionRepository.Update(permission);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<PageEntity<PermissionDTO>?> SearchAsync(string? searchKey, int? pageIndex, int? pageSize)
        {
            Expression<Func<Permission, bool>> filter = x =>
                string.IsNullOrEmpty(searchKey) ||
                x.Url.ToLower().Contains(searchKey.ToLower());

            // Sắp xếp theo thời gian tạo mới nhất
            Func<IQueryable<Permission>, IOrderedQueryable<Permission>> orderBy = q => q.OrderByDescending(x => x.CreatedTime);

            // Lấy danh sách có phân trang
            var entities = await _unitOfWork.PermissionRepository
                .Get(filter: filter, orderBy: orderBy, pageIndex: pageIndex, pageSize: pageSize);

            // Khởi tạo đối tượng PageEntity
            var pagin = new PageEntity<PermissionDTO>
            {
                List = _mapper.Map<IEnumerable<PermissionDTO>>(entities).ToList(),
                TotalRecord = await _unitOfWork.PermissionRepository.Count(filter),
                TotalPage = PaginHelper.PageCount(await _unitOfWork.PermissionRepository.Count(filter), pageSize ?? 10)
            };

            return pagin;
        }
    }
}
    
