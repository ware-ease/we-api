using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Utils;
using Data.Entity;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.Account;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.UnitOfWork;
using DotNetEnv;
using MailKit.Security;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services
{
    public class AccountService : GenericService<Data.Entity.Account>, IAccountService
    {
        public AccountService(IGenericRepository<Data.Entity.Account> genericRepository, IMapper mapper, IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
        }

        public override async Task<ServiceResponse> Get<AccountDTO>()
        {
            var results = _unitOfWork.AccountRepository.GetWithFullInfo().OrderByDescending(a => a.CreatedTime);

            List<Data.Model.DTO.AccountDTO> mappedResults = _mapper.Map<List<Data.Model.DTO.AccountDTO>>(results);

            foreach (var mappedResult in mappedResults)
            {
                if (mappedResult.CreatedBy != null)
                {
                    var createdByAccount = await _unitOfWork.AccountRepository.GetByCondition(a => a.Id == mappedResult.CreatedBy/*, "Profile,AccountGroups,Group"*/);
                    if (createdByAccount != null)
                    {
                        mappedResult.CreatedByAvatarUrl = createdByAccount.Profile!.AvatarUrl;
                        mappedResult.CreatedByFullName = $"{createdByAccount.Profile.FirstName} {createdByAccount.Profile.LastName}";
                        mappedResult.CreatedByGroup = createdByAccount.AccountGroups.FirstOrDefault()?.Group?.Name;
                    }
                }
            }

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Tìm thành công!",
                Data = mappedResults
            };
        }

        public override async Task<ServiceResponse> Get<AccountDTO>(string id)
        {
            Data.Entity.Account? entity = await _unitOfWork.AccountRepository.GetWithFullInfo(id);

            Data.Model.DTO.AccountDTO result = _mapper.Map<Data.Model.DTO.AccountDTO>(entity);

            var groups = result.Groups;

            if (entity != null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Tìm thành công!",
                    Data = result
                };
            }

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.NotFound,
                Message = "Không tìm thấy!",
                Data = id
            };
        }

        public async Task<ServiceResponse> Add(AccountCreateDTO request)
        {
            try
            {
                var existingUsername = await _unitOfWork.AccountRepository
                    .GetByCondition(a => a.Username.ToLower() == request.Username!.ToLower());
                if (existingUsername != null)
                {
                    return new ServiceResponse
                    {
                        Status = Data.Enum.SRStatus.Duplicated,
                        Message = "Tên đăng nhập đã tồn tại!",
                        Data = _mapper.Map<AccountDTO>(request)
                    };
                }

                var existingEmail = await _unitOfWork.AccountRepository
                    .GetByCondition(a => a.Email.ToLower() == request.Email!.ToLower());
                if (existingEmail != null)
                {
                    return new ServiceResponse
                    {
                        Status = Data.Enum.SRStatus.Duplicated,
                        Message = "Email đã tồn tại!",
                        Data = _mapper.Map<AccountDTO>(request)
                    };
                }

                var account = _mapper.Map<Data.Entity.Account>(request);

                //account.Id = Guid.NewGuid().ToString();
                account.Profile!.Id = Guid.NewGuid().ToString();
                string password = Guid.NewGuid().ToString("N");
                account.Password = PasswordHelper.ConvertToEncrypt(password);
                account.Profile.CreatedTime = DateTime.Now;
                account.CreatedTime = DateTime.Now;
                account.Profile.CreatedBy = request.CreatedBy;
                account.CreatedBy = request.CreatedBy;

                account.AccountGroups.Add(new AccountGroup
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedTime = DateTime.Now,
                    CreatedBy = request.CreatedBy,
                    GroupId = request.GroupId,
                    IsDeleted = false,
                });

                if ((request.GroupId.Equals("2") || request.GroupId.Equals("3")) && request.WarehouseIds != null)
                {
                    foreach (var warehouseId in request.WarehouseIds)
                    {
                        account.AccountWarehouses.Add(new AccountWarehouse
                        {
                            Id = Guid.NewGuid().ToString(),
                            CreatedTime = DateTime.Now,
                            CreatedBy = request.CreatedBy,
                            WarehouseId = warehouseId,
                            IsDeleted = false,
                            Status = true,
                        });
                    }
                }

                await _unitOfWork.AccountRepository.Add(account);

                await _unitOfWork.SaveAsync();

                string emailSubject = "Thông tin tài khoản của bạn";
                string emailBody = $@"<h3>Chào {request.Username},</h3>
                                        <p>Bạn đã được tạo tài khoản thành công trên hệ thống.</p>
                                        <p><strong>Username:</strong> {request.Username}</p>
                                        <p><strong>Password:</strong> {password}</p>
                                        <p>Vui lòng đăng nhập và đổi mật khẩu trong vòng 7 ngày để bảo mật hơn.</p>
                                        <p>Trân trọng,</p>
                                        <p>Đội ngũ hỗ trợ</p>";

                await SendEmailAsync(request.Email!, emailSubject, emailBody);

                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Thêm tài khoản thành công!",
                    Data = _mapper.Map<AccountDTO>(account)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Error,
                    Message = ex.Message,
                    Data = _mapper.Map<AccountDTO>(request)
                };
            }
        }

        public async Task<ServiceResponse> Update(string id, AccountUpdateDTO request)
        {
            try
            {
                var existingAccount = await _unitOfWork.AccountRepository
                    .GetByCondition(a => a.Id == id, "Profile");

                if (existingAccount == null)
                    throw new Exception("Không tìm thấy tài khoản!");

                if (!string.IsNullOrEmpty(request.Email))
                {
                    if (existingAccount.Email != request.Email)
                    {
                        var existingEmail = await _unitOfWork.AccountRepository
                            .GetByCondition(a => a.Email.ToLower() == request.Email!.ToLower());
                        if (existingEmail != null)
                        {
                            return new ServiceResponse
                            {
                                Status = Data.Enum.SRStatus.Duplicated,
                                Message = "Email đã tồn tại!",
                                Data = id
                            };
                        }
                    }
                }

                if (!string.IsNullOrEmpty(request.Profile.FirstName))
                {
                    existingAccount.Profile!.FirstName = request.Profile.FirstName;
                }
                if (!string.IsNullOrEmpty(request.Profile.LastName))
                {
                    existingAccount.Profile!.LastName = request.Profile.LastName;
                }
                if (!string.IsNullOrEmpty(request.Profile.Address))
                {
                    existingAccount.Profile!.Address = request.Profile.Address;
                }
                if (!string.IsNullOrEmpty(request.Profile.AvatarUrl))
                {
                    existingAccount.Profile!.AvatarUrl = request.Profile.AvatarUrl;
                }
                if (!string.IsNullOrEmpty(request.Profile.Nationality))
                {
                    existingAccount.Profile!.Nationality = request.Profile.Nationality;
                }
                if (!string.IsNullOrEmpty(request.Profile.Phone))
                {
                    existingAccount.Profile!.Phone = request.Profile.Phone;
                }
                if (request.Profile.Sex != existingAccount.Profile!.Sex)
                {
                    existingAccount.Profile!.Sex = request.Profile.Sex;
                }

                _unitOfWork.AccountRepository.Update(existingAccount);

                await _unitOfWork.SaveAsync();

                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Cập nhật thành công!",
                    Data = _mapper.Map<AccountDTO>(existingAccount)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Error,
                    Message = ex.Message,
                    Data = id
                };
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
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

        public async Task<ServiceResponse> ChangePassword(string id, string oldPassword, string password)
        {
            var account = await _unitOfWork.AccountRepository.GetByCondition(a => a.Id == id && a.Password == PasswordHelper.ConvertToEncrypt(oldPassword));

            if (account == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Unauthorized,
                    Message = "Sai mật khẩu!",
                    Data = id,
                };
            }

            account.Password = PasswordHelper.ConvertToEncrypt(password);
            account.Status = Data.Enum.AccountStatus.Verified;
            _unitOfWork.AccountRepository.Update(account);
            await _unitOfWork.SaveAsync();

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Cập nhật thành công!",
                Data = { }
            };
        }

        public async Task<ServiceResponse> UpdateStatus(string id, AccountStatus newStatus)
        {
            var account = await _unitOfWork.AccountRepository.GetByCondition(a => a.Id == id);

            if (account == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Không tìm thấy tài khoản!",
                    Data = id,
                };
            }
                
            if(newStatus == AccountStatus.Unverified)
            {
                account.UnverifiedAt = DateTime.Now;
            }
            account.Status = newStatus;
            _unitOfWork.AccountRepository.Update(account);
            await _unitOfWork.SaveAsync();

            // Send mail 
            if(account.Email  != null) {
                if (account.Status == AccountStatus.Locked)
                {
                    string emailSubject = "Cập nhật trạng thái tài khoản";
                    string emailBody = $@"<h3>Chào {account.Username},</h3>
                                        <p>Tài khoản của bạn đã bị khóa.</p>
                                        <p><strong>Trạng thái tài khoản:</strong> {newStatus.ToString()}</p>
                                        <p>Trân trọng,</p>
                                        <p>Đội ngũ hỗ trợ</p>";

                    await SendEmailAsync(account.Email!, emailSubject, emailBody);
                }
                else
                {
                    string emailSubject = "Cập nhật trạng thái tài khoản";
                    string emailBody = $@"<h3>Chào {account.Username},</h3>
                                        <p>Trạng thái tài khoản của bạn đã được cập nhật thành công.</p>
                                        <p><strong>Trạng thái tài khoản:</strong> {newStatus.ToString()}</p>
                                        <p>Trân trọng,</p>
                                        <p>Đội ngũ hỗ trợ</p>";

                    await SendEmailAsync(account.Email!, emailSubject, emailBody);
                }
            }
            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Cập nhật trạng thái tài khoản thành công!",
                Data = { }
            };
        }
        //get task of staff
        public async Task<ServiceResponse> GetTasks(string id, string? warehouseId, InventoryCountDetailStatus? status, int? pageIndex = null, int? pageSize = null)
        {
            var account = await _unitOfWork.AccountRepository.GetByCondition(a => a.Id == id);

            if (account == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Không tìm thấy tài khoản",
                    Data = id,
                };
            }
            Expression<Func<InventoryCount, bool>> filter;
            filter = p =>
                (string.IsNullOrEmpty(warehouseId) || p.Schedule!.Warehouse.Id == warehouseId) &&
                p.InventoryCheckDetails.Any(d => d.AccountId == id && (!status.HasValue || d.Status == status));

            var totalRecords = await _unitOfWork.InventoryCountRepository.Count(filter);

            var results = await _unitOfWork.InventoryCountRepository.Search(
                filter: filter,
                pageIndex: pageIndex, pageSize: pageSize,
                includeProperties: "InventoryCheckDetails,Schedule.Warehouse,InventoryCheckDetails.Inventory.Batch.Product");

            var mappedResults = _mapper.Map<List<InventoryCountDTO>>(results);

            //Filter InventoryCountDetails to only include those with the specified AccountId
            foreach (var dto in mappedResults)
            {
                //Filter InventoryCountDetails to only include those with the specified AccountId
                dto.InventoryCountDetails = dto.InventoryCountDetails?
                    .Where(d => d.AccountId == id)
                    .ToList()!;

                //Assign avatar and full name for the creator of the InventoryCount
                if (!string.IsNullOrEmpty(dto.CreatedBy))
                {
                    var accountCreated = await _unitOfWork.AccountRepository.GetByCondition(
                        a => a.Id == dto.CreatedBy, "Profile,AccountGroups,AccountGroups.Group");

                    if (accountCreated != null)
                    {
                        dto.CreatedByAvatarUrl = accountCreated.Profile!.AvatarUrl;
                        dto.CreatedByFullName = $"{accountCreated.Profile.FirstName} {accountCreated.Profile.LastName}";
                        dto.CreatedByGroup = accountCreated.AccountGroups.FirstOrDefault()?.Group?.Name;
                    }
                }

                //Assign avatar and full name for each detail
                if (dto.InventoryCountDetails != null && dto.InventoryCountDetails.Any())
                {
                    foreach (var detailDto in dto.InventoryCountDetails)
                    {
                        if (!string.IsNullOrEmpty(detailDto.CreatedBy))
                        {
                            var detailAccount = await _unitOfWork.AccountRepository.GetByCondition(
                                a => a.Id == detailDto.AccountId, "Profile,AccountGroups,AccountGroups.Group");

                            if (detailAccount != null)
                            {
                                detailDto.CreatedByAvatarUrl = detailAccount.Profile!.AvatarUrl;
                                detailDto.CreatedByFullName = $"{detailAccount.Profile.FirstName} {detailAccount.Profile.LastName}";
                                detailDto.CreatedByGroup = detailAccount.AccountGroups.FirstOrDefault()?.Group?.Name;
                            }
                        }
                    }
                }
            }

            //Remove those with no InventoryCountDetails
            mappedResults = mappedResults
                .Where(c => c.InventoryCountDetails != null && c.InventoryCountDetails.Any())
                .ToList();

            int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? totalRecords));

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Tìm thành công!",
                Data = new
                {
                    TotalRecords = mappedResults.Count,
                    TotalPages = totalPages,
                    PageIndex = pageIndex ?? 1,
                    PageSize = pageSize ?? mappedResults.Count,
                    Records = mappedResults
                }
            };
        }
    }
}
