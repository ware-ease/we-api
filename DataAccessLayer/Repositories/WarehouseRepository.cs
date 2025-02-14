using Data.Entity;
using DataAccessLayer.IRepositories;

namespace DataAccessLayer.Repositories
{
    public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
    {
        private readonly WaseEaseDbContext _context;

        public WarehouseRepository(WaseEaseDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

