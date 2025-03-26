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
        public string? Title { get; set; }
        public string? Content { get; set; }
        public bool Status { get; set; }

        public ICollection<AccountNotification> AccountNotifications { get; set; } = [];

        /*[ForeignKey("Account")]
        public string AccountId { get; set; }
        public Account Account { get; set; }*/
    }
}
