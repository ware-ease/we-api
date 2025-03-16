using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("Batch")]
    public class Batch : BaseEntity
    {
        public string? SupplierId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public DateOnly MfgDate { get; set; }
        public DateOnly ExpDate { get; set; }
        public ICollection<IssueDetail> IssueDetails { get; set; } = [];
        public ICollection<ReceivingDetail> ReceivingDetails { get; set; } = [];
        public ICollection<GoodNoteDetail> GoodNoteDetails { get; set; } = [];

        public string? InventoryId { get; set; }

        [ForeignKey("Product")]
        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}
