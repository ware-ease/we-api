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
    [Table("PurchaseReceipt")]
    public class PurchaseReceipt : BaseEntity
    {
        public DateTime? Date { get; set; }
        public ICollection<ReceivingNote> ReceivingNotes { get; set; }

        [ForeignKey("Supplier")]
        public string SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}
