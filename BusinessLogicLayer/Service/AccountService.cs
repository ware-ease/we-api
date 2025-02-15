using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.Authentication;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Utils;
using Data.Entity;
using DataAccessLayer.UnitOfWork;
using System.Linq.Expressions;
using Profile = Data.Entity.Profile;

namespace BusinessLogicLayer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PageEntity<AccountDTO>?> GetAllAccountsAsync(int? pageIndex, int? pageSize)
        {
            // Sắp xếp theo ngày tạo mới nhất
            Func<IQueryable<Account>, IOrderedQueryable<Account>> orderBy = q => q.OrderByDescending(x => x.CreatedTime);

            // Lấy danh sách tài khoản có phân trang
            var entities = await _unitOfWork.AccountRepository
                .Get(filter: null, orderBy: orderBy, pageIndex: pageIndex, pageSize: pageSize);

            // Khởi tạo đối tượng PageEntity
            var pagin = new PageEntity<AccountDTO>();
            pagin.List = _mapper.Map<IEnumerable<AccountDTO>>(entities).ToList();

            // Đếm tổng số tài khoản
            pagin.TotalRecord = await _unitOfWork.AccountRepository.Count(null);

            // Tính tổng số trang
            pagin.TotalPage = PaginHelper.PageCount(pagin.TotalRecord, pageSize ?? 10);

            return pagin;
        }


        public async Task<AccountDTO?> CheckLoginAsync(string userName, string password)
        {
            var user = await _unitOfWork.AccountRepository
                .CheckLoginAsync(userName, PasswordHelper.ConvertToEncrypt(password));
            return _mapper.Map<AccountDTO>(user);
        }

        public async Task<TokenDTO> GenerateAccessTokenAsync(string id)
        {
            var token = await _unitOfWork.AccountRepository.GenerateAccessTokenAsync(id);
            var tokentDTO = _mapper.Map<TokenDTO>(token);
            return tokentDTO;
        }
        public async Task<AccountDTO?> GetAccountByIdAsync(string id)
        {
            var account = await _unitOfWork.AccountRepository.GetByID(id);
            return _mapper.Map<AccountDTO>(account);
        }

        public async Task<AccountDTO> CreateAccountAsync(AccountCreateDTO model)
        {
            try
            {
                // Kiểm tra xem username đã tồn tại chưa
                var existingUsername = await _unitOfWork.AccountRepository
                    .GetByCondition(a => a.UserName == model.UserName);
                if (existingUsername != null)
                {
                    throw new Exception("Username đã tồn tại");
                }

                // Kiểm tra xem email đã tồn tại chưa
                var existingEmail = await _unitOfWork.AccountRepository
                    .GetByCondition(a => a.Email == model.Email);
                if (existingEmail != null)
                {
                    throw new Exception("Email đã tồn tại");
                }

                var account = _mapper.Map<Account>(model);
                
                account.Password = PasswordHelper.ConvertToEncrypt(model.Password);
                account.CreatedTime = DateTime.Now;
                account.CreatedBy = model.CreatedBy;
                //Data.Entity.Profile profile = new Profile();
                //profile.AccountId = account.Id;

                await _unitOfWork.AccountRepository.Insert(account);
                //await _unitOfWork.ProfileRepository.Insert(profile);

                await _unitOfWork.SaveAsync();

                return _mapper.Map<AccountDTO>(account);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo tài khoản: {ex.Message}");
            }
        }


        public async Task<AccountDTO?> UpdateAccountAsync(string id, AccountUpdateDTO model)
        {
            var account = await _unitOfWork.AccountRepository.GetByID(id);
            if (account == null) return null;
            account.LastUpdatedTime = DateTime.Now;
            account.LastUpdatedBy = model.LastUpdatedBy;

            account.Email = model.Email ?? account.Email;

            _unitOfWork.AccountRepository.Update(account);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<AccountDTO>(account);
        }

        public async Task<bool> DeleteAccountAsync(string id, string deletedBy)
        {
            var account = await _unitOfWork.AccountRepository.GetByID(id);
            if (account == null) return false;

            account.IsDeleted = true;
            account.DeletedTime = DateTime.Now;
            account.DeletedBy = deletedBy; 

            _unitOfWork.AccountRepository.Update(account);
            await _unitOfWork.SaveAsync();
            return true;
        }


        public async Task<AccountDTO> UpdateUsernameAsync(string accountId, string newUsername, string LastUpdatedBy)
        {
            var account = await _unitOfWork.AccountRepository.GetByID(accountId);
            if (account == null)
                return null!;

            if (!string.IsNullOrEmpty(newUsername))
            {
                account.UserName = newUsername;
            }
            account.LastUpdatedTime = DateTime.Now;
            account.LastUpdatedBy = LastUpdatedBy;

            _unitOfWork.AccountRepository.Update(account);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<AccountDTO>(account);
        }

        public async Task<AccountDTO> UpdatePasswordAsync(string accountId, string currentPassword, string newPassword, string LastUpdatedBy)
        {
            var account = await _unitOfWork.AccountRepository.GetByID(accountId);
            if (account == null)
                return null!;
            if (string.IsNullOrEmpty(newPassword))
            {
                return _mapper.Map<AccountDTO>(account);
            }
            // Kiểm tra mật khẩu hiện tại
            if (!string.IsNullOrEmpty(currentPassword) && PasswordHelper.ConvertToEncrypt(currentPassword) != account.Password)
            {
                throw new Exception("Password không trùng khớp");
            }
            account.Password = PasswordHelper.ConvertToEncrypt(newPassword);
            account.LastUpdatedTime = DateTime.Now;
            account.LastUpdatedBy = LastUpdatedBy;
            _unitOfWork.AccountRepository.Update(account);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<AccountDTO>(account);
        }

        public async Task<PageEntity<AccountDTO>?> SearchAccountAsync(string? searchKey, int? pageIndex, int? pageSize)
        {
            // Tạo bộ lọc tìm kiếm theo AccountName hoặc Email
            Expression<Func<Account, bool>> filter = x =>
                string.IsNullOrEmpty(searchKey) ||
                x.UserName .ToLower().Contains(searchKey.ToLower()) ||
                x.Email.ToLower().Contains(searchKey.ToLower());

            // Sắp xếp theo AccountId giảm dần
            Func<IQueryable<Account>, IOrderedQueryable<Account>> orderBy = q => q.OrderByDescending(x => x.CreatedTime);

            // Lấy danh sách tài khoản có phân trang
            var entities = await _unitOfWork.AccountRepository
                .Get(filter: filter, orderBy: orderBy, pageIndex: pageIndex, pageSize: pageSize);

            // Khởi tạo đối tượng PageEntity
            var pagin = new PageEntity<AccountDTO>();
            pagin.List = _mapper.Map<IEnumerable<AccountDTO>>(entities).ToList();

            // Đếm tổng số tài khoản khớp với bộ lọc
            pagin.TotalRecord = await _unitOfWork.AccountRepository.Count(filter);

            // Tính tổng số trang
            pagin.TotalPage = PaginHelper.PageCount(pagin.TotalRecord, pageSize ?? 10);

            return pagin;
        }
        public async Task<HashSet<string>> GetUserPermissionsAsync(string userId)
        {
            var user = await _unitOfWork.AccountRepository.GetByCondition(
                filter: a => a.Id == userId,
                includeProperties: "AccountGroups.Group.GroupPermissions.Permission.PermissionActions.Action, AccountPermissions.Permission.PermissionActions.Action"
            );

            if (user == null) return new HashSet<string>();

            var permissions = new HashSet<string>();

            // ✅ Lấy quyền từ Group của User
            if (user.AccountGroups != null)
            {
                foreach (var userGroup in user.AccountGroups)
                {
                    if (userGroup.Group?.GroupPermissions != null)
                    {
                        foreach (var groupPermission in userGroup.Group.GroupPermissions)
                        {
                            if (groupPermission.Permission?.PermissionActions != null)
                            {
                                foreach (var action in groupPermission.Permission.PermissionActions)
                                {
                                    permissions.Add($"{groupPermission.Permission.Url}.{action.Action.Code}");
                                }
                            }
                        }
                    }
                }
            }

            // ✅ Lấy quyền riêng của User
            if (user.AccountPermissions != null)
            {
                foreach (var userPermission in user.AccountPermissions)
                {
                    if (userPermission.Permission?.PermissionActions != null)
                    {
                        foreach (var action in userPermission.Permission.PermissionActions)
                        {
                            permissions.Add($"{userPermission.Permission.Url}.{action.Action.Code}");
                        }
                    }
                }
            }

            return permissions;
        }

    }
}
