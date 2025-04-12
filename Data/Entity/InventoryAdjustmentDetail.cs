using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("InventoryAdjustmentDetail")]
    public class InventoryAdjustmentDetail : BaseEntity
    {
        public float NewQuantity { get; set; }
        public float ChangeInQuantity { get; set; }
        public string? Note { get; set; }

        [ForeignKey("InventoryAdjustment")]
        public string InventoryAdjustmentId { get; set; }
        public InventoryAdjustment InventoryAdjustment { get; set; }

        [ForeignKey("LocationLog")]
        public string? InventoryId { get; set; }
        public Inventory Inventory { get; set; }
    }
}
