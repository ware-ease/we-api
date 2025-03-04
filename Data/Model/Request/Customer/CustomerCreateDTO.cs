using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.Customer
{
    public class CustomerCreateDTO
    {
        [Required]
        public string? Name { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public bool Status { get; set; }
    }
}
