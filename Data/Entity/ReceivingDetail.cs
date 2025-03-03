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
    [Table("ReceivingDetail")]
    public class ReceivingDetail : BaseEntity
    {
        [ForeignKey("ReceivingNote")]
        public string NoteId { get; set; }
        public ReceivingNote receivingNote { get; set; }

        [ForeignKey("Batch")]
        public string BatchId { get; set; }
        public Batch Batch { get; set; }
    }
}
