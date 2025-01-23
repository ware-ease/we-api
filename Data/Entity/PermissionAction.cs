using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("PermissionAction")]
    public class PermissionAction : BaseEntity
    {
        [ForeignKey("AppAction")]
        public string ActionId { get; set; }
        public AppAction Action { get; set; }

        [ForeignKey("Permission")]
        public string PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}
