using Data.Entity;
using DataAccessLayer.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class AccountActionRepository : GenericRepository<AccountAction>, IAccountActionRepository
    {
        private readonly WaseEaseDbContext _context;

        public AccountActionRepository(WaseEaseDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
