using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Account
{
    public class AccountUpdateDTO
    {
        [EmailAddress]
        public string? Email { get; set; }

        public ProfileCreateDTO Profile { get; set; }
    }

    public class AccountPasswordUpdateDTO
    {
        [Required]
        public string? OldPassword { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
