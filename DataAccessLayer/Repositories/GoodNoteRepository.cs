using Data.Entity;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;

namespace DataAccessLayer.Repositories
{
    public class GoodNoteRepository : GenericRepository<GoodNote>, IGoodNoteRepository
    {
        public GoodNoteRepository(WaseEaseDbContext context) : base(context)
        {
        }
    }
}