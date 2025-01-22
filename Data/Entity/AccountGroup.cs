using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("AccountGroup")]
    public class AccountGroup : BaseEntity
    {
        [ForeignKey("Account")]
        public string AccountId { get; set; }
        public Account Account { get; set; }

        [ForeignKey("Group")]
        public string GroupId { get; set; }
        public Group Group { get; set; }
    }
}
