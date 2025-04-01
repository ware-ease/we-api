using Data.Model.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class AccountDTO : BaseDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public AccountDTOProfile Profile { get; set; }

        public IEnumerable<AccountDTOGroup>? Groups { get; set; }
        public IEnumerable<PermissionDTO>? Permissions { get; set; }
        public IEnumerable<AccountDTOWarehouse>? Warehouses { get; set; }
    }

    public class AccountDTOGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<PermissionDTO> Permissions { get; set; }
    }

    public class AccountDTOWarehouse
    {

    }

    public class AccountDTOProfile
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool Sex { get; set; }
        public string Nationality { get; set; }
        public string AvatarUrl { get; set; }
    }
}

