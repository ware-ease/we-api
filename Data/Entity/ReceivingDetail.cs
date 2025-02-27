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
    [Table("ReceiptDetail")]
    public class ReceivingDetail : BaseEntity
    {
        public int Quanlity { get; set; }
        public float Price { get; set; }

        [ForeignKey("ReceivingNote")]
        public string NoteId { get; set; }
        public ReceivingNote receivingNote { get; set; }

        [ForeignKey("Brand")]
        public string BrandId { get; set; }
        public Brand Brand { get; set; }
    }
}
