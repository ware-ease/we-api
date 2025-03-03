using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.AccountAction
{
    public class AccountActionDTO : BaseEntity
    {
        public string AccountId { get; set; }
        public string ActionId { get; set; }
    }

    public class CreateAccountActionDTO
    {
        public List<string> AccountIds { get; set; }
        public List<string> ActionIds { get; set; }
        public string? CreatedBy { get; set; }

    }

    public class UpdateAccountActionDTO
    {
        public string AccountId { get; set; }
        public string GroupId { get; set; }
        public string? LastUpdatedBy { get; set; }

    }
    public class DeleteAccountActionDTO
    {
        public List<string> AccountIds { get; set; }
        public List<string> ActionIds { get; set; }
    }
}
