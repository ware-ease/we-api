using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Utils;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.DTO.Base;
using Data.Model.Request.Account;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.Migrations;
using DataAccessLayer.UnitOfWork;
using DotNetEnv;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class AccountService : GenericService<Data.Entity.Account>, IAccountService
    {
        public AccountService(IGenericRepository<Data.Entity.Account> genericRepository, IMapper mapper, IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
        }

        public override async Task<ServiceResponse> Get<AccountDTO>()
        {
            var results = _unitOfWork.AccountRepository.GetWithFullInfo();

            List<Data.Model.DTO.AccountDTO> mappedResults = _mapper.Map<List<Data.Model.DTO.AccountDTO>>(results);

            foreach (var mappedResult in mappedResults)
            {
                if (mappedResult.CreatedBy != null)
                {
                    var createdBy = await GetCreatedBy(mappedResult.CreatedBy);

                    if (createdBy != null)
                    {
                        mappedResult.CreatedBy = createdBy!.Username;
                    }
                    else
                    {
                        mappedResult.CreatedBy = "Deleted user";
                    }
                }
            }

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
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
                    Message = "Get successfully!",
                    Data = result
                };
            }

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.NotFound,
                Message = "Not found!",
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
                        Message = "Username is existed!",
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
                        Message = "Email is existed!",
                        Data = _mapper.Map<AccountDTO>(request)
                    };
                }

                var account = _mapper.Map<Data.Entity.Account>(request);

                //account.Id = Guid.NewGuid().ToString();
                account.Profile!.Id = Guid.NewGuid().ToString();
                string password = account.Username;
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

                if (request.GroupId.Equals("2") && request.WarehouseIds != null)
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
                                        <p>Vui lòng đăng nhập và đổi mật khẩu để bảo mật hơn.</p>
                                        <p>Trân trọng,</p>
                                        <p>Đội ngũ hỗ trợ</p>";

                await SendEmailAsync(request.Email!, emailSubject, emailBody);

                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Add successfully!",
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
                    Message = "Wrong password!",
                    Data = id,
                };
            }

            account.Password = PasswordHelper.ConvertToEncrypt(password);

            _unitOfWork.AccountRepository.Update(account);
            await _unitOfWork.SaveAsync();

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Update successfully!",
                Data = { }
            };
        }
    }
}
