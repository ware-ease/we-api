using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.Authentication;
using BusinessLogicLayer.Utils;
using Data.Entity;
using Data.Model.DTO;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(IGenericRepository<Data.Entity.Account> genericRepository, IMapper mapper, IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        public async Task<ServiceResponse> Add(AccountCreateDTO request)
        {
            try
            {
                // Kiểm tra xem username đã tồn tại chưa
                var existingUsername = await _unitOfWork.AccountRepository
                    .GetByCondition(a => a.Username == request.Username);
                if (existingUsername != null)
                {
                    throw new Exception("Username đã tồn tại");
                }

                // Kiểm tra xem email đã tồn tại chưa
                var existingEmail = await _unitOfWork.AccountRepository
                    .GetByCondition(a => a.Email == request.Email);
                if (existingEmail != null)
                {
                    throw new Exception("Email đã tồn tại");
                }

                var account = _mapper.Map<Data.Entity.Account>(request);

                string password = Guid.NewGuid().ToString();
                account.Password = PasswordHelper.ConvertToEncrypt(password);
                account.CreatedTime = DateTime.Now;
                account.CreatedBy = request.CreatedBy;
                Data.Entity.Profile profile = new Data.Entity.Profile();
                profile = _mapper.Map<Data.Entity.Profile>(request.Profile);

                //await _unitOfWork.SaveAsync();

                profile.AccountId = account.Id;
                profile.CreatedBy = request.CreatedBy;
                profile.CreatedTime = DateTime.Now;
                account.Profile = profile;
                profile.Account = account;

                await _unitOfWork.AccountRepository.Add(account);
                await _unitOfWork.ProfileRepository.Add(profile);

                // **Thêm AccountGroup nếu có groupIds**
                if (request.groupIds != null && request.groupIds.Any())
                {
                    List<string> validGroupIds = new List<string>();
                    List<string> invalidGroupIds = new List<string>();

                    foreach (var groupId in request.groupIds)
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
                            CreatedBy = request.CreatedBy,
                            CreatedTime = DateTime.Now
                        };
                        //await _unitOfWork.AccountGroupRepository.Insert(accountGroup);
                    }
                }

                await _unitOfWork.SaveAsync();
                // Gửi email thông tin tài khoản
                string emailSubject = "Thông tin tài khoản của bạn";
                string emailBody = $@"<h3>Chào {request.Username},</h3>
                                        <p>Bạn đã được tạo tài khoản thành công trên hệ thống.</p>
                                        <p><strong>Username:</strong> {request.Username}</p>
                                        <p><strong>Password:</strong> {password}</p>
                                        <p>Vui lòng đăng nhập và đổi mật khẩu để bảo mật hơn.</p>
                                        <p>Trân trọng,</p>
                                        <p>Đội ngũ hỗ trợ</p>";

                await SendEmailAsync(request.Email, emailSubject, emailBody);

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
    }
}
