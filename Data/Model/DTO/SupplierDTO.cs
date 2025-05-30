﻿using Data.Model.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class SupplierDTO : BaseDTO
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public bool Status { get; set; }
    }
}
