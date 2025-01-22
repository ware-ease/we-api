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
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public ICollection<AccountGroup> AccountGroups { get; set; }
        public ICollection<AccountPermission> AccountPermissions { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<AccountWarehouse> AccountWarehouses { get; set; }

    }
}
