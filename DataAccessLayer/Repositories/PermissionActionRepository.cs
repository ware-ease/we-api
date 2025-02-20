using Data.Entity;
using DataAccessLayer.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class PermissionActionRepository : GenericRepository<PermissionAction>, IPermissionActionRepository
    {
        private readonly WaseEaseDbContext _context;

        public PermissionActionRepository(WaseEaseDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
