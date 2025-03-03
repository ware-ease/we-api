﻿using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.AccountAction;
using BusinessLogicLayer.Models.AccountGroup;
using BusinessLogicLayer.Models.Authentication;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Utils;
using Data.Entity;
using Data.Model.DTO;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.UnitOfWork;
using DotNetEnv;
using MailKit.Security;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;
using System.Linq.Expressions;
namespace BusinessLogicLayer.Services
{
    public class AccountService : GenericService<Account>, IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public AccountService(IGenericRepository<Account> genericRepository, IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache) : base(genericRepository, mapper, unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            Env.Load();
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Admin WareEaseSystem", Env.GetString("SMTP_FROM")));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                await smtp.ConnectAsync(Env.GetString("SMTP_HOST"), Env.GetInt("SMTP_PORT"), SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(Env.GetString("SMTP_USER"), Env.GetString("SMTP_PASS"));
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
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
            string includes = "AccountGroups.Group,AccountWarehouses.Warehouse,AccountActions.Action";

            var account = await _unitOfWork.AccountRepository.GetByCondition(a => a.Id == id, includeProperties: includes);
            return _mapper.Map<AccountDTO>(account);
        }

        public async Task<AccountDTO> CreateAccountAsync(AccountCreateDTO model)
        {
            try
            {
                // Kiểm tra xem username đã tồn tại chưa
                var existingUsername = await _unitOfWork.AccountRepository
                    .GetByCondition(a => a.Username == model.Username);
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

                string password = Guid.NewGuid().ToString();
                account.Password = PasswordHelper.ConvertToEncrypt(password);
                account.CreatedTime = DateTime.Now;
                account.CreatedBy = model.CreatedBy;
                Data.Entity.Profile profile = new Data.Entity.Profile();
                profile = _mapper.Map<Data.Entity.Profile>(model.Profile);

                //await _unitOfWork.SaveAsync();

                profile.AccountId = account.Id;
                profile.CreatedBy = model.CreatedBy;
                profile.CreatedTime = DateTime.Now;
                account.Profile = profile;
                profile.Account = account;

                await _unitOfWork.AccountRepository.Add(account);
                await _unitOfWork.ProfileRepository.Add(profile);

                // **Thêm AccountGroup nếu có groupIds**
                if (model.groupIds != null && model.groupIds.Any())
                {
                    List<string> validGroupIds = new List<string>();
                    List<string> invalidGroupIds = new List<string>();

                    foreach (var groupId in model.groupIds)
                    {
                        var group = await _unitOfWork.GroupRepository.GetByCondition(g => g.Id == groupId);
                        if (group != null)
                        {
                            validGroupIds.Add(groupId);
                        }
                        else
                        {
                            invalidGroupIds.Add(groupId);
                        }
                    }

                    // Kiểm tra nếu có groupId không hợp lệ
                    if (invalidGroupIds.Any())
                    {
                        throw new Exception($"Nhóm không tồn tại: {string.Join(", ", invalidGroupIds)}");
                    }

                    // Thêm vào AccountGroup từng bản ghi
                    foreach (var groupId in validGroupIds)
                    {
                        var accountGroup = new AccountGroup
                        {
                            AccountId = account.Id,
                            GroupId = groupId,
                            CreatedBy = model.CreatedBy,
                            CreatedTime = DateTime.Now
                        };
                        //await _unitOfWork.AccountGroupRepository.Insert(accountGroup);
                    }
                }

                await _unitOfWork.SaveAsync();
                // Gửi email thông tin tài khoản
                string emailSubject = "Thông tin tài khoản của bạn";
                string emailBody = $@"<h3>Chào {model.Username},</h3>
                                        <p>Bạn đã được tạo tài khoản thành công trên hệ thống.</p>
                                        <p><strong>Username:</strong> {model.Username}</p>
                                        <p><strong>Password:</strong> {password}</p>
                                        <p>Vui lòng đăng nhập và đổi mật khẩu để bảo mật hơn.</p>
                                        <p>Trân trọng,</p>
                                        <p>Đội ngũ hỗ trợ</p>";

                await SendEmailAsync(model.Email, emailSubject, emailBody);

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
                account.Username = newUsername;
            }
            account.LastUpdatedTime = DateTime.Now;
            account.LastUpdatedBy = LastUpdatedBy;

            _unitOfWork.AccountRepository.Update(account);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<AccountDTO>(account);
        }

        #region Change password
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
        public async Task SendOtpAsync(string email)
        {
            string otp = new Random().Next(100000, 999999).ToString();
            _cache.Set($"otp_{email}", otp, TimeSpan.FromMinutes(2)); // Lưu cache 2 phút

            string body = $"Mã OTP của bạn là: <b>{otp}</b>. Mã này có hiệu lực trong 2 phút.";
            await SendEmailAsync(email, "Xác nhận đổi mật khẩu", body);
        }

        public bool VerifyOtp(string email, string otp)
        {
            if (_cache.TryGetValue($"otp_{email}", out string? cachedOtp))
            {
                if (cachedOtp == otp)
                {
                    _cache.Remove($"otp_{email}"); // Xóa OTP sau khi sử dụng
                    return true;
                }
            }
            return false;
        }
        #endregion

        public async Task<PageEntity<AccountDTO>?> SearchAccountAsync(string? searchKey, int? pageIndex, int? pageSize)
        {
            // Tạo bộ lọc tìm kiếm theo AccountName hoặc Email
            Expression<Func<Account, bool>> filter = x =>
                string.IsNullOrEmpty(searchKey) ||
                x.Username.ToLower().Contains(searchKey.ToLower()) ||
                x.Email.ToLower().Contains(searchKey.ToLower());

            // Sắp xếp theo AccountId giảm dần
            Func<IQueryable<Account>, IOrderedQueryable<Account>> orderBy = q => q.OrderByDescending(x => x.CreatedTime);

            string includes = "AccountGroups.Group,AccountWarehouses.Warehouse,AccountActions.Action";

            // Lấy danh sách tài khoản có phân trang
            var entities = await _unitOfWork.AccountRepository
                .Get(filter: filter, orderBy: orderBy, includeProperties: includes , pageIndex: pageIndex, pageSize: pageSize);

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
            //if (user.AccountGroups != null)
            //{
            //    foreach (var userGroup in user.AccountGroups)
            //    {
            //        if (userGroup.Group?.GroupPermissions != null)
            //        {
            //            foreach (var groupPermission in userGroup.Group.GroupPermissions)
            //            {
            //                if (groupPermission.Permission?.PermissionActions != null)
            //                {
            //                    foreach (var action in groupPermission.Permission.PermissionActions)
            //                    {
            //                        permissions.Add($"{groupPermission.Permission.Url}.{action.Action.Code}");
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            // ✅ Lấy quyền riêng của User
            //if (user.AccountPermissions != null)
            //{
            //    foreach (var userPermission in user.AccountPermissions)
            //    {
            //        if (userPermission.Permission?.PermissionActions != null)
            //        {
            //            foreach (var action in userPermission.Permission.PermissionActions)
            //            {
            //                permissions.Add($"{userPermission.Permission.Url}.{action.Action.Code}");
            //            }
            //        }
            //    }
            //}

            return permissions;
        }

        public async Task<bool> CreateAsync(CreateAccountGroupDTO model)
        {
            foreach (var accountId in model.AccountIds)
            {
                foreach (var groupId in model.GroupIds)
                {
                    // Kiểm tra xem nhóm này đã được thêm cho tài khoản này chưa
                    //var existingEntity = await _unitOfWork.AccountGroupRepository.GetByCondition(
                    //    filter: x => x.AccountId == accountId && x.GroupId == groupId);

                    //if (existingEntity != null)
                    //{
                    //    continue; // Nếu đã tồn tại thì bỏ qua
                    //}

                    //var entity = new AccountGroup
                    //{
                    //    AccountId = accountId,
                    //    GroupId = groupId,
                    //    CreatedBy = model.CreatedBy,
                    //    CreatedTime = DateTime.Now
                    //};

                    //await _unitOfWork.AccountGroupRepository.Insert(entity);
                }
            }

            await _unitOfWork.SaveAsync(); // Chỉ lưu một lần sau khi insert xong tất cả
            return true;
        }

        public async Task<bool> CreateAccountActionAsync(CreateAccountActionDTO model)
        {
            foreach (var accountId in model.AccountIds)
            {
                foreach (var actionId in model.ActionIds)
                {
                    // Kiểm tra xem nhóm này đã được thêm cho tài khoản này chưa
                    //var existingEntity = await _unitOfWork.AccountActionRepository.GetByCondition(
                    //    filter: x => x.AccountId == accountId && x.ActionId == actionId);

                    //if (existingEntity != null)
                    //{
                    //    continue; // Nếu đã tồn tại thì bỏ qua
                    //}

                    //var entity = new AccountAction
                    //{
                    //    AccountId = accountId,
                    //    ActionId = actionId,
                    //    CreatedBy = model.CreatedBy,
                    //    CreatedTime = DateTime.Now
                    //};

                    //await _unitOfWork.AccountActionRepository.Insert(entity);
                }
            }

            await _unitOfWork.SaveAsync(); // Chỉ lưu một lần sau khi insert xong tất cả
            return true;
        }
        public async Task<bool> DeleteMultipleAccountGroupAsync(List<string> accountIds, List<string> groupIds)
        {
            try
            {
                List<AccountGroup> entitiesToDelete = new List<AccountGroup>();
                List<string> notFoundPairs = new List<string>();

                foreach (var accountId in accountIds)
                {
                    foreach (var groupId in groupIds)
                    {
                        //var entity = await _unitOfWork.AccountGroupRepository.GetByCondition(
                        //    x => x.AccountId == accountId && x.GroupId == groupId);

                        //if (entity != null)
                        //{
                        //    entitiesToDelete.Add(entity);
                        //}
                        //else
                        //{
                        //    notFoundPairs.Add($"(AccountId: {accountId}, GroupId: {groupId})");
                        //}
                    }
                }

                if (!entitiesToDelete.Any())
                {
                    return false; // Không có bản ghi nào để xóa
                }

                // Nếu có cặp Account-Group không tồn tại, báo lỗi
                if (notFoundPairs.Any())
                {
                    throw new Exception($"Không tìm thấy các cặp Account-Group: {string.Join(", ", notFoundPairs)}");
                }

                // Xóa từng bản ghi một
                foreach (var entity in entitiesToDelete)
                {
                    //_unitOfWork.AccountGroupRepository.Delete(entity);
                }

                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa dữ liệu: {ex.Message}");
            }
        }

        public async Task<bool> DeleteMultipleAccountActionsAsync(List<string> accountIds, List<string> actionIds)
        {
            try
            {
                List<AccountPermission> entitiesToDelete = new List<AccountPermission>();
                List<string> notFoundPairs = new List<string>();

                foreach (var accountId in accountIds)
                {
                    foreach (var actionId in actionIds)
                    {
                        //var entity = await _unitOfWork.AccountActionRepository.GetByCondition(
                        //    x => x.AccountId == accountId && x.ActionId == actionId);

                        //if (entity != null)
                        //{
                        //    entitiesToDelete.Add(entity);
                        //}
                        //else
                        //{
                        //    notFoundPairs.Add($"(AccountId: {accountId}, ActionId: {actionId})");
                        //}
                    }
                }

                if (!entitiesToDelete.Any())
                {
                    return false; // Không có bản ghi nào để xóa
                }

                // Nếu có cặp Account-Action không tồn tại, báo lỗi
                if (notFoundPairs.Any())
                {
                    throw new Exception($"Không tìm thấy các cặp Account-Action: {string.Join(", ", notFoundPairs)}");
                }

                // Xóa từng bản ghi một
                foreach (var entity in entitiesToDelete)
                {
                    //_unitOfWork.AccountActionRepository.Delete(entity);
                }

                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa dữ liệu: {ex.Message}");
            }
        }

    }
}
