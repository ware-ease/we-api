using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.Account
{
    public class AccountUpdateDTO
    {
        public string? Email { get; set; }
        public string? LastUpdatedBy { get; set; }

    }
    public class UpdateUsernameDTO
    {
        public string NewUsername { get; set; }
        public string? LastUpdatedBy { get; set; }

    }

    public class UpdatePasswordDTO
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string? LastUpdatedBy { get; set; }

    }
    public class DeleteAccountDTO
    {
        public string? DeletedBy { get; set; }
    }

}
