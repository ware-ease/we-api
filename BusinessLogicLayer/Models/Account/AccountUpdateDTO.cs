using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.Account
{
    public class AccountUpdateDTO
    {
        public string? Password { get; set; } 
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }
}
