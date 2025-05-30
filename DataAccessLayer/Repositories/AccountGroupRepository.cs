﻿using Data.Entity;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class AccountGroupRepository : GenericRepository<AccountGroup>, IAccountGroupRepository
    {
        public AccountGroupRepository(WaseEaseDbContext context) : base(context)
        {
        }
    }
}