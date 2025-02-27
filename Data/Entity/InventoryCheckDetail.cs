using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("InventoryCheckDetail")]
    public class InventoryCheckDetail : BaseEntity
    {
        public string CheckedQuantity { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }

        [ForeignKey("InventoryCheck")]
        public string InventoryCheckId { get; set; }
        public InventoryCheck InventoryCheck { get; set; }
    }
}
