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
    [Table("IssueNote")]
    public class IssueNote : BaseEntity
    {
        public DateTime Date { get; set; }

        [ForeignKey("Customer")]
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }

        [ForeignKey("SaleReceipt")]
        public string SaleReceiptId { get; set; }
        public SaleReceipt SaleReceipt { get; set; }

    }
}
