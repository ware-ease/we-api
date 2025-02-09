﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.Pagination
{
    public class PaginationParams
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
