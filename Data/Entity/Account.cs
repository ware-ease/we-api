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
    [Table("Account")]
    public class Account : BaseEntity
    {

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public ICollection<AccountGroup> AccountGroups { get; set; }
        public ICollection<AccountAction> AccountActions { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<AccountWarehouse> AccountWarehouses { get; set; }

        public Profile Profile { get; set; }

    }
}
