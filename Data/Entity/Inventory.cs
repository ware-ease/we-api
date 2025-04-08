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
        public float CurrentQuantity { get; set; }
        public float? ArrangedQuantity { get; set; }
        public float? NotArrgangedQuantity { get; set; }

        public ICollection<InventoryLocation> InventoryLocations { get; set; } = [];
        //public ICollection<InventoryAdjustmentDetail> InventoryAdjustmentDetails { get; set; } = [];

        [ForeignKey("Warehouse")]
        public string WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        [ForeignKey("Batch")]
        public string BatchId { get; set; }
        public Batch Batch { get; set; }
    }
}
