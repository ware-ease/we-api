using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.AccountPermission
{
    public class AccountPermissionDTO : BaseEntity
    {
        public string AccountId { get; set; }
        public string PermissionId { get; set; }
    }

    public class CreateAccountPermissionDTO
    {
        public string AccountId { get; set; }
        public string PermissionId { get; set; }
        public string? CreatedBy { get; set; }
    }
    public class UpdateAccountPermissionDTO
    {
        public string AccountId { get; set; }
        public string PermissionId { get; set; }
        public string? LastUpdatedBy { get; set; }

    }
}
