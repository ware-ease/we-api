using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("Inventory")]
    public class Inventory : BaseEntity
    {
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
        public int MinQuantity { get; set; }

        [ForeignKey("Warehouse")]
        public string WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        [ForeignKey("Product")]
        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}
