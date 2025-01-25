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
        [Required]
        public int Quanlity { get; set; }
        [Required]
        public float Price { get; set; }

        [ForeignKey("IssueNote")]
        public string NoteId { get; set; }
        public IssueNote IssueNote { get; set; }

        [ForeignKey("ProductType")]
        public string ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }
    }
}
