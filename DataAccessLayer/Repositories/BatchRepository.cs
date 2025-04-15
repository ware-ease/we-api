using Data.Entity;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class BatchRepository : GenericRepository<Batch>, IBatchRepository
    {
        public BatchRepository(WaseEaseDbContext context) : base(context)
        {
        }
        public IQueryable<Batch> GetQueryable()
        {
            return _context.Batches.AsQueryable();
        }

    }
}