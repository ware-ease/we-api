using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("ErrorTicket")]
    public class ErrorTicket : BaseEntity
    {
        public string? Reason { get; set; }
        public string? Code { get; set; }
        public string? Note { get; set; }
        public string? HandleBy { get; set; }

        [ForeignKey("InventoryCountDetail")]
        public string InventoryCountDetailId { get; set; }
        public InventoryCountDetail InventoryCountDetail { get; set; }
    }
}
