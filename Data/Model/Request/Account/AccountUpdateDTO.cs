using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.Account
{
    public class AccountUpdateDTO
    {
    }

    public class AccountPasswordUpdateDTO
    {
        [Required]
        public string? Password { get; set; }
    }
}
