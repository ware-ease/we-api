using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("GroupPermission")]
    public class GroupPermission : BaseEntity
    {
        [ForeignKey("Group")]
        public string GroupId { get; set; }
        public Group Group { get; set; }

        [ForeignKey("Permission")]
        public string PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}
