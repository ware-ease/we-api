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
    public class IssueDetail : BaseEntity
    {
        public int Quanlity { get; set; }
        public float Price { get; set; }

        [ForeignKey("IssueNote")]
        public string NoteId { get; set; }
        public IssueNote IssueNote { get; set; }

        [ForeignKey("Batch")]
        public string BatchId { get; set; }
        public Batch Batch { get; set; }
    }
}
