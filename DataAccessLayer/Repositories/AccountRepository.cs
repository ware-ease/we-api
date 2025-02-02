using Data.Entity;
using DataAccessLayer.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.Repositories
{
    public class AccountRepository  : IAccountRepository
    {
        private readonly WaseEaseDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountRepository(WaseEaseDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Account>> GetAccountsAsync()
        {
            return await _context.Accounts.ToListAsync();
        }

        //public async Task<AppUser?> CheckLoginAsync(string userName, string password)
        //{
        //    password = PasswordHelper.ConvertToEncrypt(password);
        //    var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.UserName.Equals(userName)&& u.Password == password && u.Status == (int)Status.Exist);
        //    if (user == null)
        //    {
        //        return null;
        //    }
        //    return user;
        //}
        //public async Task<Token> GenerateAccessTokenAsync(int id)
        //{
        //    var user = await _context.AppUsers.FindAsync(id);
        //    if (user == null)
        //    {
        //        return null!;
        //    }
        //    var Token = await JwtHelper.GenerateAccessTokenAsync(user, _context, _configuration);
        //    return Token;
        //}

    }
}
