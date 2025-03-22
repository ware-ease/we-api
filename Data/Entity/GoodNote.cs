using Data.Entity.Base;
using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("GoodNote")]
    public class GoodNote : BaseEntity
    {
        public GoodNoteEnum NoteType { get; set; }
        public GoodNoteEnum Status { get; set; }
        public string? ShipperName { get; set; }
        public string? ReceiverName { get; set; }
        public string? Code { get; set; }
        public DateTime? Date { get; set; }

        [ForeignKey("GoodRequest")]
        public string GoodRequestId { get; set; }
        public GoodRequest GoodRequest { get; set; }
    }
}
