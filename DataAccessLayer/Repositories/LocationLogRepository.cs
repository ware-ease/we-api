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
    public class LocationLogRepository : GenericRepository<LocationLog>, ILocationLogRepository
    {
        public LocationLogRepository(WaseEaseDbContext context) : base(context)
        {
        }
    }
}