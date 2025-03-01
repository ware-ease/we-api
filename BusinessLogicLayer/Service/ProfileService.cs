using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Group;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.Profile;
using BusinessLogicLayer.Utils;
using Data.Entity;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Profile = Data.Entity.Profile;

namespace BusinessLogicLayer.Service
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProfileService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PageEntity<ProfileDTO>?> GetAllAsync(int? pageIndex, int? pageSize)
        {
            // Sắp xếp theo thời gian tạo mới nhất
            Func<IQueryable<Profile>, IOrderedQueryable<Profile>> orderBy = q => q.OrderByDescending(x => x.CreatedTime);

            // Lấy danh sách có phân trang
            var entities = await _unitOfWork.ProfileRepository
                .Get(filter: null, orderBy: orderBy, pageIndex: pageIndex, pageSize: pageSize);

            // Khởi tạo đối tượng PageEntity
            var pagin = new PageEntity<ProfileDTO>();
            pagin.List = _mapper.Map<IEnumerable<ProfileDTO>>(entities).ToList();

            // Đếm tổng số bản ghi
            pagin.TotalRecord = await _unitOfWork.ProfileRepository.Count(null);

            // Tính tổng số trang
            pagin.TotalPage = PaginHelper.PageCount(pagin.TotalRecord, pageSize ?? 10);

            return pagin;
        }

        public async Task<ProfileDTO?> GetProfileByUserIdAsync(string id)
        {
            var profile = await _unitOfWork.ProfileRepository.GetByCondition(p => p.AccountId == id);
            return _mapper.Map<ProfileDTO>(profile);
        }

        public async Task<ProfileDTO> CreateProfileAsync(ProfileCreateDTO model)
        {
            try
            {
                //// Kiểm tra accountId có hợp lệ không
                //var account = await _unitOfWork.AccountRepository.GetByID(model.AccountId);
                //if (account == null)
                //{
                //    throw new ArgumentException("Account ID không hợp lệ hoặc không tồn tại.");
                //}

                //// Kiểm tra xem accountId đã có profile chưa
                //var existingProfile = await _unitOfWork.ProfileRepository.GetByCondition(p => p.AccountId == model.AccountId);
                //if (existingProfile != null)
                //{
                //    throw new InvalidOperationException("Tài khoản này đã có profile, không thể tạo mới.");
                //}

                //// Ánh xạ dữ liệu từ DTO sang entity
                //var profile = _mapper.Map<Data.Entity.Profile>(model);
                //profile.CreatedTime = DateTime.Now;
                //profile.CreatedBy = model.CreatedBy;
                //// Thêm vào database
                //await _unitOfWork.ProfileRepository.Insert(profile);
                //await _unitOfWork.SaveAsync();

                //return _mapper.Map<ProfileDTO>(profile);
                return null;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Lỗi đầu vào: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Lỗi logic: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi tạo profile.", ex);
            }
        }

        public async Task<ProfileDTO?> UpdateProfileAsync(string id, ProfileUpdateDTO model)
        {
            try
            {
                var profile = await _unitOfWork.ProfileRepository.GetByID(id);
                if (profile == null)
                    throw new KeyNotFoundException("Profile không tồn tại.");
                profile.LastUpdatedTime = DateTime.Now;
                profile.LastUpdatedBy = model.LastUpdatedBy;

                _mapper.Map(model, profile);
                _unitOfWork.ProfileRepository.Update(profile);
                await _unitOfWork.SaveAsync();

                return _mapper.Map<ProfileDTO>(profile);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException($"Lỗi: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi cập nhật profile.", ex);
            }
        }

        public async Task<bool> DeleteProfileAsync(string id, string deleteBy)
        {
            try
            {
                var profile = await _unitOfWork.ProfileRepository.GetByID(id);
                if (profile == null)
                    throw new KeyNotFoundException("Profile không tồn tại.");

                profile.IsDeleted = true;
                profile.DeletedTime = DateTime.Now;
                profile.DeletedBy = deleteBy;

                _unitOfWork.ProfileRepository.Update(profile);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException($"Lỗi: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi xóa profile.", ex);
            }
        }

        public async Task<ProfileDTO?> UpdateProfileByAccountIdAsync(string accountId, ProfileUpdateDTO model)
        {
            var profile = await _unitOfWork.ProfileRepository.GetByCondition(p =>p.AccountId == accountId);
            if (profile == null) return null;

            profile.FirstName = model.FirstName ?? profile.FirstName;
            profile.LastName = model.LastName ?? profile.LastName;
            profile.Sex = model.Sex;
            profile.Address = model.Address ?? profile.Address;
            profile.Phone = model.Phone ?? profile.Phone;
            profile.Nationality = model.Nationality ?? profile.Nationality;
            profile.LastUpdatedBy = model.LastUpdatedBy ?? profile.LastUpdatedBy;

             _unitOfWork.ProfileRepository.Update(profile);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<ProfileDTO>(profile);
        }

        public async Task<PageEntity<ProfileDTO>?> SearchAsync(string? searchKey, int? pageIndex, int? pageSize)
        {
            Expression<Func<Data.Entity.Profile, bool>> filter = x =>
                string.IsNullOrEmpty(searchKey) ||
                x.FirstName.ToLower().Contains(searchKey.ToLower()) ||
                x.Address.ToLower().Contains(searchKey.ToLower()) ||
                x.Nationality.ToLower().Contains(searchKey.ToLower()) ||
                x.Phone.ToLower().Contains(searchKey.ToLower()) ||
                x.LastName!.ToLower().Contains(searchKey.ToLower());

            // Sắp xếp theo thời gian tạo mới nhất
            Func<IQueryable<Data.Entity.Profile>, IOrderedQueryable<Data.Entity.Profile>> orderBy = q => q.OrderByDescending(x => x.CreatedTime);

            // Lấy danh sách có phân trang
            var entities = await _unitOfWork.ProfileRepository
                .Get(filter: filter, orderBy: orderBy, pageIndex: pageIndex, pageSize: pageSize);

            // Khởi tạo đối tượng PageEntity
            var pagin = new PageEntity<ProfileDTO>();
            pagin.List = _mapper.Map<IEnumerable<ProfileDTO>>(entities).ToList();

            // Đếm tổng số bản ghi khớp với bộ lọc
            pagin.TotalRecord = await _unitOfWork.ProfileRepository.Count(filter);

            // Tính tổng số trang
            pagin.TotalPage = PaginHelper.PageCount(pagin.TotalRecord, pageSize ?? 10);

            return pagin;
        }
    }
}
