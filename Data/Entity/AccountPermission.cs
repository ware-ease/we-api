using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("AccountPermission")]
    public class AccountPermission : BaseEntity
    {
        [ForeignKey("Account")]
        public string AccountId { get; set; }
        public Account Account { get; set; }

        [ForeignKey("Permission")]
        public string PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}
