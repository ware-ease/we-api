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
    [Table("Supplier")]
    public class Supplier : BaseEntity
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public bool Status { get; set; }
        public ICollection<ReceivingNote> ReceivingNotes { get; set; } = [];
    }
}
