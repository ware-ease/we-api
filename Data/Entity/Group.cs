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
    [Table("Group")]
    public class Group : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<GroupAction> GroupActions { get; set; }
        public ICollection<AccountGroup> AccountGroups { get; set; }
    }
}
