using Data.Entity;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;

namespace DataAccessLayer.Repositories
{
    public class ProfileRepository : GenericRepository<Profile>, IProfileRepository
    {
        private readonly WaseEaseDbContext _context;

        public ProfileRepository(WaseEaseDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
