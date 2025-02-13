using Data.Entity;
using DataAccessLayer.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class AppActionRepository : GenericRepository<AppAction>, IAppActionRepository
    {
        private readonly WaseEaseDbContext _context;

        public AppActionRepository(WaseEaseDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
