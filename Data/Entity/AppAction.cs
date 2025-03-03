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
    [Table("Action")]
    public class AppAction : BaseEntity
    {
        public string? Code { get; set; }

        [ForeignKey("Permission")]
        public string PermissionId { get; set; }
        public Permission Permission { get; set; }

        public ICollection<GroupAction> GroupActions { get; set; } = [];
        public ICollection<AccountAction> AccountActions { get; set; } = [];
    }
}
