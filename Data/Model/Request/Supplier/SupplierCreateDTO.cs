using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Suppiler
{
    public class SupplierCreateDTO : BaseCreateDTO
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Tên tối đa 100 ký tự.")]
        [MinLength(2, ErrorMessage = "Tên tối thiểu 2 ký tự.")]
        public string? Name { get; set; }
        [Phone]
        [Required]
        public string? Phone { get; set; }
        [Required]
        public bool Status { get; set; } = true;
    }
}