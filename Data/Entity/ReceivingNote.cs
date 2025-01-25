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
        [Required]
        public DateTime Date { get; set; }
        public ICollection<ReceivingDetail> ReceivingDetails { get; set; }

        [ForeignKey("Supplier")]
        public string SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        [ForeignKey("PurchaseReceipt")]
        public string PurchaseId { get; set; }
        public PurchaseReceipt PurchaseReceipt { get; set; }
    }
}
