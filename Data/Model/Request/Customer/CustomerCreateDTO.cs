using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.Customer
{
    public class CustomerCreateDTO : BaseCreateDTO
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Tên tối đa 100 ký tự.")]
        [MinLength(2, ErrorMessage = "Tên tối thiểu 2 ký tự.")]
        public string? Name { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public bool Status { get; set; } = true;
    }
}
