﻿using Data.Entity.Base;
using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("Unit")]
    public class Unit : BaseEntity
    {
        public string? Name { get; set; }
        public string? Note { get; set; }
        public UnitEnum Type { get; set; } = UnitEnum.Int;
        public ICollection<Product> Products { get; set; } = [];
    }
}
