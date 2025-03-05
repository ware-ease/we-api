using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Utils;
using Data.Entity;
using Data.Model.Request.Warehouse;
using DataAccessLayer.UnitOfWork;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Service
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WarehouseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PageEntity<WarehouseDTO>> GetAllAsync(int pageIndex, int pageSize)
        {
            try
            {
                var orderBy = (Func<IQueryable<Warehouse>, IOrderedQueryable<Warehouse>>)
                    (q => q.OrderByDescending(x => x.CreatedTime));

                var entities = await _unitOfWork.WarehouseRepository.Get(filter: null, orderBy: orderBy, pageIndex: pageIndex, pageSize: pageSize);

                var pagin = new PageEntity<WarehouseDTO>
                {
                    List = _mapper.Map<IEnumerable<WarehouseDTO>>(entities).ToList(),
                    TotalRecord = await _unitOfWork.WarehouseRepository.Count(null),
                    TotalPage = PaginHelper.PageCount(await _unitOfWork.WarehouseRepository.Count(null), pageSize)
                };

                return pagin;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách kho: {ex.Message}");
            }
        }

        public async Task<WarehouseDTO?> GetByIdAsync(string id)
        {
            try
            {
                var warehouse = await _unitOfWork.WarehouseRepository.GetByID(id);
                return warehouse == null ? null : _mapper.Map<WarehouseDTO>(warehouse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy kho theo ID: {ex.Message}");
            }
        }

        public async Task<WarehouseDTO> CreateAsync(CreateWarehouseDTO model)
        {
            try
            {
                var warehouse = _mapper.Map<Warehouse>(model);
                warehouse.CreatedTime = DateTime.Now;

                await _unitOfWork.WarehouseRepository.Insert(warehouse);
                await _unitOfWork.SaveAsync();

                return _mapper.Map<WarehouseDTO>(warehouse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo kho: {ex.Message}");
            }
        }

        public async Task<WarehouseDTO?> UpdateAsync(string id, UpdateWarehouseDTO model)
        {
            try
            {
                var warehouse = await _unitOfWork.WarehouseRepository.GetByID(id);
                if (warehouse == null) return null;

                warehouse.Name = model.Name;
                warehouse.Address = model.Address;
                warehouse.Length = model.Length;
                warehouse.Width = model.Width;
                warehouse.ShelfCount = model.ShelfCount;
                warehouse.LastUpdatedTime = DateTime.Now;

                _unitOfWork.WarehouseRepository.Update(warehouse);
                await _unitOfWork.SaveAsync();

                return _mapper.Map<WarehouseDTO>(warehouse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật kho: {ex.Message}");
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var warehouse = await _unitOfWork.WarehouseRepository.GetByID(id);
                if (warehouse == null) return false;

                warehouse.IsDeleted = true;
                warehouse.DeletedTime = DateTime.Now;

                _unitOfWork.WarehouseRepository.Update(warehouse);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa kho: {ex.Message}");
            }
        }

        public async Task<PageEntity<WarehouseDTO>> SearchAsync(string? searchKey, int? pageIndex, int? pageSize)
        {
            try
            {
                Expression<Func<Warehouse, bool>> filter = x =>
                    string.IsNullOrEmpty(searchKey) ||
                    x.Name.ToLower().Contains(searchKey.ToLower()) ||
                    x.Address.ToLower().Contains(searchKey.ToLower());

                var orderBy = (Func<IQueryable<Warehouse>, IOrderedQueryable<Warehouse>>)
                    (q => q.OrderByDescending(x => x.CreatedTime));

                var entities = await _unitOfWork.WarehouseRepository.Get(filter: filter, orderBy: orderBy, pageIndex: pageIndex, pageSize: pageSize);

                var pagin = new PageEntity<WarehouseDTO>
                {
                    List = _mapper.Map<IEnumerable<WarehouseDTO>>(entities).ToList(),
                    TotalRecord = await _unitOfWork.WarehouseRepository.Count(filter),
                    TotalPage = PaginHelper.PageCount(await _unitOfWork.WarehouseRepository.Count(filter), pageSize ?? 10)
                };

                return pagin;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm kho: {ex.Message}");
            }
        }
    }
}