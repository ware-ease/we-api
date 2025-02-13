using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.AppAction;
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
    public class AppActionService : IAppActionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public AppActionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PageEntity<AppActionDTO>?> GetAllAsync(int? pageIndex, int? pageSize)
        {
            // Sắp xếp theo thời gian tạo mới nhất
            Func<IQueryable<AppAction>, IOrderedQueryable<AppAction>> orderBy = q => q.OrderByDescending(x => x.CreatedTime);

            // Lấy danh sách có phân trang
            var entities = await _unitOfWork.AppActionRepository
                .Get(filter: null, orderBy: orderBy, pageIndex: pageIndex, pageSize: pageSize);

            // Khởi tạo đối tượng PageEntity
            var pagin = new PageEntity<AppActionDTO>();
            pagin.List = _mapper.Map<IEnumerable<AppActionDTO>>(entities).ToList();

            // Đếm tổng số bản ghi
            pagin.TotalRecord = await _unitOfWork.AppActionRepository.Count(null);

            // Tính tổng số trang
            pagin.TotalPage = PaginHelper.PageCount(pagin.TotalRecord, pageSize ?? 10);

            return pagin;
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
            action.CreatedBy = actionCreate.CreatedBy;
            await _unitOfWork.AppActionRepository.Insert(action);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<AppActionDTO>(action);
        }

        public async Task<AppActionDTO> UpdateAsync(string id, UpdateAppActionDTO action)
        {
            var existingAction = await _unitOfWork.AppActionRepository.GetByID(id);
            if (existingAction == null) return null;

            existingAction.Code = action.Code ?? existingAction.Code;
            existingAction.LastUpdatedTime = DateTime.Now;
            existingAction.LastUpdatedBy = action.LastUpdatedBy;

            _unitOfWork.AppActionRepository.Update(existingAction);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<AppActionDTO>(existingAction);
        }

        public async Task<bool> DeleteAsync(string id, string deleteBy)
        {
            var action = await _unitOfWork.AppActionRepository.GetByID(id);
            if (action == null) return false;

            action.IsDeleted = true;
            action.DeletedTime = DateTime.Now;
            action.DeletedBy = deleteBy;

            _unitOfWork.AppActionRepository.Update(action);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<PageEntity<AppActionDTO>?> SearchAsync(string? searchKey, int? pageIndex, int? pageSize)
        {
            // Bộ lọc tìm kiếm theo Code hoặc Name
            Expression<Func<AppAction, bool>> filter = x =>
                string.IsNullOrEmpty(searchKey) ||
                x.Code.ToLower().Contains(searchKey.ToLower());

            // Sắp xếp theo thời gian tạo mới nhất
            Func<IQueryable<AppAction>, IOrderedQueryable<AppAction>> orderBy = q => q.OrderByDescending(x => x.CreatedTime);

            // Lấy danh sách có phân trang
            var entities = await _unitOfWork.AppActionRepository
                .Get(filter: filter, orderBy: orderBy, pageIndex: pageIndex, pageSize: pageSize);

            // Khởi tạo đối tượng PageEntity
            var pagin = new PageEntity<AppActionDTO>();
            pagin.List = _mapper.Map<IEnumerable<AppActionDTO>>(entities).ToList();

            // Đếm tổng số bản ghi khớp với bộ lọc
            pagin.TotalRecord = await _unitOfWork.AppActionRepository.Count(filter);

            // Tính tổng số trang
            pagin.TotalPage = PaginHelper.PageCount(pagin.TotalRecord, pageSize ?? 10);

            return pagin;
        }

    }
}