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
    [Table("Supplier")]
    public class Supplier : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public bool Status { get; set; }
        public ICollection<ReceivingNote> ReceivingNotes { get; set; }
        public ICollection<PurchaseReceipt> PurchaseReceipts { get; set; }

    }
}
