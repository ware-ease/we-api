﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entity.Base;

namespace Data.Entity
{
    [Table("Category")]
    public class Category : BaseEntity
    {
        public string? Name { get; set; }
        public string? Note { get; set; }
        public ICollection<ProductType> ProductTypes { get; set; } = [];
    }
}
