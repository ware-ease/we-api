using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.PermissionAction
{
    public class PermissionActionDTO : BaseEntity
    {
        public string ActionId { get; set; }
        public string PermissionId { get; set; }
    }

    public class CreatePermissionActionDTO
    {
        public string ActionId { get; set; }
        public string PermissionId { get; set; }
        public string? CreatedBy { get; set; }
    }
    public class UpdatePermissionActionDTO
    {
        public string ActionId { get; set; }
        public string PermissionId { get; set; }
        public string? LastUpdatedBy { get; set; }

    }
}
