using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("AccountAction")]
    public class AccountAction : BaseEntity
    {
        [ForeignKey("Account")]
        public string AccountId { get; set; }
        public Account Account { get; set; }

        [ForeignKey("Action")]
        public string ActionId { get; set; }
        public AppAction Action { get; set; }
    }
}
