using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.Brand
{
    public class BrandCreateDTO : BaseCreateDTO
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Tên tối đa 50 ký tự.")]
        [MinLength(3, ErrorMessage = "Tên tối thiểu 3 ký tự.")]
        public string? Name { get; set; }
    }
}
