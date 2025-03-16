using Data.Entity.Base;
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
        public float ExpectedQuantity { get; set; }
        public float CountedQuantity { get; set; }
        public string? Note { get; set; }

        [ForeignKey("InventoryCount")]
        public string InventoryCountId { get; set; }
        public InventoryCount InventoryCount { get; set; }

        [ForeignKey("Product")]
        public string ProductId { get; set; }
        public Product Product { get; set; }

        public string? ErrorTicketId { get; set; }
    }
}
