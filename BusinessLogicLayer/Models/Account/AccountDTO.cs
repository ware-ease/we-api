using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Data.Entity;
using Data.Entity.Base;
using BusinessLogicLayer.Models.Group;
using BusinessLogicLayer.Models.Warehouse;
using BusinessLogicLayer.Models.AccountAction;
using BusinessLogicLayer.Models.AppAction;

namespace BusinessLogicLayer.Models.Account
{
    public class AccountDTO : BaseEntity
    {
        //public string Id { get; set; }
        public string Username { get; set; }
        //public string Password { get; set; }
        public string Email { get; set; }

        //public bool IsDeleted { get; set; } = false;
        //public string? CreatedBy { get; set; }
        //public string? LastUpdatedBy { get; set; }
        //public string? DeletedBy { get; set; }
        //public DateTime? CreatedTime { get; set; }
        //public DateTime? LastUpdatedTime { get; set; }
        //public DateTime? DeletedTime { get; set; }
        //public ICollection<GroupDTO> AccountGroups { get; set; }

        public IEnumerable<GroupDTO>? Groups { get; set; }
        public IEnumerable<AppActionDTO>? Actions { get; set; }
        public IEnumerable<WarehouseDTO>? Warehouses { get; set; }
    }
}
