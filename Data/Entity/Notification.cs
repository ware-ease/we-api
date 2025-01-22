using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("Notification")]
    public class Notification : BaseEntity
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public bool Status { get; set; }
        [Required]

        [ForeignKey("Account")]
        public string AccountId { get; set; }
        public Account Account { get; set; }
    }
}
