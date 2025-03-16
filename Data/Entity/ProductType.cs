using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("ProductType")]
    public class ProductType : BaseEntity
    {
        public string? Name { get; set; }
        public string? Note { get; set; }

        [ForeignKey("Category")]
        public string CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Product> Products { get; set; } = [];
    }
}
