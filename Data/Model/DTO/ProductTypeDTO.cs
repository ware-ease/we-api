﻿using Data.Entity;
using Data.Model.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class ProductTypeDTO : BaseDTO
    {
        public string Name { get; set; }
        public string Note { get; set; }
        public string CategoryName { get; set; }

    }
}
