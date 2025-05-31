using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.Category
{
    public class CategoryCreateDTO : BaseCreateDTO
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Tên tối đa 100 ký tự.")]
        [MinLength(5, ErrorMessage = "Tên tối thiểu 5 ký tự.")]
        public string? Name { get; set; }
        public string? Note { get; set; }
    }
}
