using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("GoodNoteDetail")]
    public class GoodNoteDetail : BaseEntity
    {
        public float Quantity { get; set; }
        public string? Note { get; set; }

        [ForeignKey("GoodNote")]
        public string GoodNoteId { get; set; }
        public GoodNote GoodNote { get; set; }

        [ForeignKey("Batch")]
        public string BatchId { get; set; }
        public Batch Batch { get; set; }
    }
}
