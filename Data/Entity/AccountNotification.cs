using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("AccountNotification")]
    public class AccountNotification : BaseEntity
    {
        [ForeignKey("Notification")]
        public string NotificationId { get; set; }
        public Notification Notification { get; set; }

        [ForeignKey("Account")]
        public string AccountId { get; set; }
        public Account Account { get; set; }
    }
}
