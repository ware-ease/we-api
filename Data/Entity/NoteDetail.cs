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
    [Table("NoteDetail")]
    public class NoteDetail : BaseEntity
    {
        [Required]
        public int Quantity { get; set; }

        [ForeignKey("ReceivingNote")]
        public string ReceivingNoteId { get; set; }
        public ReceivingNote ReceivingNote { get; set; }

        [ForeignKey("ProductType")]
        public string ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }
    }
}
