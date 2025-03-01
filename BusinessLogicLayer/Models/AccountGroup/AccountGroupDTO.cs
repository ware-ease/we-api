using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.AccountGroup
{
    public class AccountGroupDTO : BaseEntity
    {
        public string AccountId { get; set; }
        public string GroupId { get; set; }
    }

    public class CreateAccountGroupDTO
    {
        public List<string> AccountIds { get; set; }
        public List<string> GroupIds { get; set; }
        public string? CreatedBy { get; set; }

    }

    public class UpdateAccountGroupDTO
    {
        public string AccountId { get; set; }
        public string GroupId { get; set; }
        public string? LastUpdatedBy { get; set; }

    }
}