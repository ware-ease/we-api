using Data.Entity.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entity
{
    [Table("Inventory")]
    public class Inventory : BaseEntity
    {
        public float CurrentQuantity { get; set; }
        public float? ArrangedQuantity { get; set; }
        public float? NotArrgangedQuantity { get; set; }

        [ForeignKey("Warehouse")]
        public string WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        [ForeignKey("Batch")]
        public string BatchId { get; set; }
        public Batch Batch { get; set; }

        public ICollection<InventoryCountDetail> InventoryCountDetails { get; set; } = [];
        public ICollection<InventoryAdjustmentDetail> InventoryAdjustmentDetails { get; set; } = [];
    }
}
