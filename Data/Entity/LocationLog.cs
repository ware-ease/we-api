using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("LocationLog")]
    public class LocationLog : BaseEntity
    {
        public float NewQuantity { get; set; }
        public float ChangeInQuantity { get; set; }

        [ForeignKey("Location")]
        public string LocationId { get; set; }
        public Location Location { get; set; }

        [ForeignKey("Inventory")]
        public string InventoryId { get; set; }
        public Inventory Inventory { get; set; }
    }
}
