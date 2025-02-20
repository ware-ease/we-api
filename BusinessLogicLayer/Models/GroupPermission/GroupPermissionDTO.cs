using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.GroupPermission
{
    public class GroupPermissionDTO : BaseEntity
    {
        public string GroupId { get; set; }
        public string PermissionId { get; set; }
    }

    public class CreateGroupPermissionDTO
    {
        public string GroupId { get; set; }
        public string PermissionId { get; set; }
        public string? CreatedBy { get; set; }
    }
    public class UpdateGroupPermissionDTO
    {
        public string GroupId { get; set; }
        public string PermissionId { get; set; }
        public string? LastUpdatedBy { get; set; }

    }
}
