using Data.Entity.Base;
using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("InventoryCountDetail")]
    public class InventoryCountDetail : BaseEntity
    {
        public InventoryCountDetailStatus Status { get; set; }
        public float ExpectedQuantity { get; set; }
        public float CountedQuantity { get; set; }
        public string? Note { get; set; }

        [ForeignKey("InventoryCount")]
        public string InventoryCountId { get; set; }
        public InventoryCount InventoryCount { get; set; }

        [ForeignKey("Inventory")]
        public string InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        public string? ErrorTicketId { get; set; }
    }
}
