using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Enum;
using DataAccessLayer.Generic;
using DataAccessLayer.UnitOfWork;
using DotNetEnv;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class WorkerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        /*private readonly IGenericRepository<Account> _accountRepository;
        private readonly IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;*/
        private readonly ILogger<WorkerService> _logger;
        private readonly IConfiguration _configuration;


        public WorkerService(IServiceScopeFactory scopeFactory, ILogger<WorkerService> logger, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const int batchSize = 100; // number
            int skip = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var accountRepo = scope.ServiceProvider.GetRequiredService<IGenericRepository<Account>>();
                var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // Get 100 account with unverified status with created time more than 7 days each time
                var expiredAccounts = await accountRepo.Search(
                    filter: a => a.Status == AccountStatus.Unverified && a.CreatedTime.Value.AddDays(7) <= DateTime.Now,
                    orderBy: q => q.OrderBy(a => a.CreatedTime), //order by CreatedTime
                    pageIndex: skip / batchSize, // Page index
                    pageSize: batchSize
                );

                if (expiredAccounts.Any())
                {
                    foreach (var account in expiredAccounts)
                    {
                        account.Status = AccountStatus.Locked;
                        accountRepo.Update(account);


                        /*Console.WriteLine($"SMTP_FROM: {Environment.GetEnvironmentVariable("SMTP_FROM")}");
                        Console.WriteLine($"SMTP_HOST: {Environment.GetEnvironmentVariable("SMTP_HOST")}");
                        Console.WriteLine($"SMTP_PORT: {Environment.GetEnvironmentVariable("SMTP_PORT")}");*/

                        var subject = "Tài khoản của bạn đã bị khóa";
                        var body = $"<p>Tài khoản của bạn ({account.Username}) đã bị khóa do không xác thực trong vòng 7 ngày.</p>" +
                            "<p>Vui lòng liên hệ với quản trị viên để biết thêm chi tiết.</p>";
                        // send email to locked account
                        await SendEmailAsync(account.Email!, subject, body);
                    }

                    await unitOfWork.SaveAsync();

                    _logger.LogInformation($"[WorkerService] Đã khóa {expiredAccounts.Count()} tài khoản hết hạn.");

                    // Update skip for the next batch
                    skip += batchSize;
                }
                else
                {
                    // If no more unverified accounts, wait for 1 hour before checking again
                    _logger.LogInformation("[WorkerService] Không còn tài khoản Unverified cần xử lý.");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                    skip = 0; // Reset skip to 0 for the next iteration
                }

                // Wait for 5 minutes before checking again
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Load environment variables from .env file
            var from = Environment.GetEnvironmentVariable("SMTP_FROM");
            var host = Environment.GetEnvironmentVariable("SMTP_HOST");
            var port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT"));
            var user = Environment.GetEnvironmentVariable("SMTP_USER");
            var pass = Environment.GetEnvironmentVariable("SMTP_PASS");

            // Create the email message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Admin WareEaseSystem", from));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            // Create the email body
            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            // Send the email using MailKit
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(user, pass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
