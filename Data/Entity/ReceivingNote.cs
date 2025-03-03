using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entity.Base;

namespace Data.Entity
{
    [Table("ReceivingNote")]
    public class ReceivingNote : BaseEntity
    {
        public string? Reason { get; set; }
        public string? ShipperName { get; set; }
        public string? ReceiverName { get; set; }
        public string? Code { get; set; }
        public DateTime Date { get; set; }
        public ICollection<ReceivingDetail> ReceivingDetails { get; set; } = [];

        [ForeignKey("Warehouse")]
        public string? WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        [ForeignKey("Supplier")]
        public string? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
