using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("Brand")]
    public class Brand : BaseEntity
    {
        public string? Name { get; set; }
        public ICollection<Product>? Products { get; set; } = [];
    }
}
