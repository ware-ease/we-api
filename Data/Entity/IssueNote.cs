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
        public string? Reason { get; set; }
        public string? IssuerName { get; set; }
        public string? ReceiverName { get; set; }
        public string? Code { get; set; }
        public string? Destination {  get; set; }
        public DateTime Date { get; set; }
        public ICollection<IssueDetail> IssueDetails { get; set; } = [];

        [ForeignKey("Warehouse")]
        public string? WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        [ForeignKey("Customer")]
        public string? CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }
}
