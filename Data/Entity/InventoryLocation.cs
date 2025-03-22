using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("InventoryLocation")]
    public class InventoryLocation : BaseEntity
    {
        public int Quantity { get; set; }

        [ForeignKey("Inventory")]
        public string InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        [ForeignKey("Location")]
        public string LocationId { get; set; }
        public Location Location { get; set; }
    }
}
