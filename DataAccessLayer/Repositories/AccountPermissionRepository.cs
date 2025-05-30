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
    public class AccountPermissionRepository : GenericRepository<AccountPermission>, IAccountPermissionRepository
    {
        public AccountPermissionRepository(WaseEaseDbContext context) : base(context)
        {
        }
    }
}
