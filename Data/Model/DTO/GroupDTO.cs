using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class GroupDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<PermissionDTO> Permissions { get; set; }
        public List<GroupDTOAccount> Accounts { get; set; }
    }

    public class GroupDTOAccount
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
