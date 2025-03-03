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
    [Table("IssueDetail")]
    public class IssueDetail : BaseEntity
    {
        [ForeignKey("IssueNote")]
        public string NoteId { get; set; }
        public IssueNote IssueNote { get; set; }

        [ForeignKey("Batch")]
        public string BatchId { get; set; }
        public Batch Batch { get; set; }
    }
}
