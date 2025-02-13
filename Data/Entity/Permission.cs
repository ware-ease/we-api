using Data.Entity.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entity
{
    [Table("Permission")]
    public class Permission : BaseEntity
    {
        public string Url { get; set; }

        public ICollection<GroupPermission> GroupPermissions { get; set; }
        public ICollection<PermissionAction> PermissionActions { get; set; }
    }
}
