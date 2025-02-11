using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Data.Entity;

namespace BusinessLogicLayer.Models.Account
{
    public class AccountDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
        public ICollection<AccountGroup>? AccountGroups { get; set; }
        public ICollection<AccountPermission>? AccountPermissions { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<AccountWarehouse>? AccountWarehouses { get; set; }
    }
}
