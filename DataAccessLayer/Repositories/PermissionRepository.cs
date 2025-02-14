using Data.Entity;
using DataAccessLayer.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        private readonly WaseEaseDbContext _context;

        public PermissionRepository(WaseEaseDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
