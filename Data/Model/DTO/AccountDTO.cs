using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class AccountDTO
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
