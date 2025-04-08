using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("InventoryAdjustment")]
    public class InventoryAdjustment : BaseEntity
    {
        public DateTime? Date { get; set; }
        public string? Reason { get; set; }
        public string? Note { get; set; }
        public string? RelatedDocument { get; set; }

        public ICollection<InventoryAdjustmentDetail> InventoryAdjustmentDetails { get; set; } = [];

        [ForeignKey("Warehouse")]
        public string WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
