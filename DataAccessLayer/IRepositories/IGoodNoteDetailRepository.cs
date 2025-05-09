﻿using Data.Entity;
using DataAccessLayer.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface IGoodNoteDetailRepository : IGenericRepository<GoodNoteDetail>
    {
        Task<List<GoodNoteDetail>> GetDetailsByGoodNoteIdAsync(string goodNoteId);
        
    }
}
