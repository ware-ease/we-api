using Data.Entity;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class GoodNoteDetailRepository : GenericRepository<GoodNoteDetail>, IGoodNoteDetailRepository
    {
        public GoodNoteDetailRepository(WaseEaseDbContext context) : base(context)
        {
        }
        public async Task<List<GoodNoteDetail>> GetDetailsByGoodNoteIdAsync(string goodNoteId)
        {
            return await _context.GoodNoteDetails
                .Include(x => x.Batch)
                .Where(x => x.GoodNoteId == goodNoteId)
                .ToListAsync();
        }

    }
}