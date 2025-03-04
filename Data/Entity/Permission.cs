using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("Permission")]
    public class Permission : BaseEntity
    {
        public string? Code { get; set; }

        [ForeignKey("Route")]
        public string RouteId { get; set; }
        public Route Route { get; set; }

        public ICollection<GroupPermission> GroupPermissions { get; set; } = [];
        public ICollection<AccountPermission> AccountPermissions { get; set; } = [];
    }
}
