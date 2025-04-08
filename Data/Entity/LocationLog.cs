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
        public string? Note { get; set; }

        [ForeignKey("InventoryLocation")]
        public string InventoryLocationId { get; set; }
        public InventoryLocation InventoryLocation { get; set; }
    }
}
