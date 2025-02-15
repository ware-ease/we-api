using Data.Entity;
using DataAccessLayer.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class AccountGroupRepository : GenericRepository<AccountGroup>, IAccountGroupRepository
    {
        private readonly WaseEaseDbContext _context;

        public AccountGroupRepository(WaseEaseDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
