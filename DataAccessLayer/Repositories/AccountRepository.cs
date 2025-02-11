using Data.Entity;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.Repositories
{
    public class AccountRepository  : GenericRepository<Account>, IAccountRepository
    {
        private readonly WaseEaseDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountRepository(WaseEaseDbContext context, IConfiguration configuration) : base(context)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Account>> GetAccountsAsync()
        {
            return await _context.Accounts.ToListAsync();
        }

        public async Task<Account?> CheckLoginAsync(string userName, string password)
        {
            //password = PasswordHelper.ConvertToEncrypt(password);           
                var user = await _context.Accounts
                    .Include(a => a.AccountGroups) // Load nhóm tài khoản
                        //.ThenInclude(ag => ag.Group) // Load thông tin nhóm
                    .Include(a => a.AccountPermissions) // Load quyền tài khoản
                        //.ThenInclude(ap => ap.Permission) // Load chi tiết quyền
                    .Include(a => a.AccountWarehouses) // Load kho tài khoản
                        //.ThenInclude(aw => aw.Warehouse) // Load chi tiết kho
                    .FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);

                return user;
        }
        public async Task<Token> GenerateAccessTokenAsync(string id)
        {
            var user = await _context.Accounts.FindAsync(id);
            if (user == null)
            {
                return null!;
            }
            var Token = await JwtHelper.GenerateAccessTokenAsync(user, _context, _configuration);
            return Token;
        }

    }
}
