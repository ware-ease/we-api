using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Unit
{
    public class UnitUpdateDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        [MaxLength(50, ErrorMessage = "Tên tối đa 50 ký tự.")]
        [MinLength(2, ErrorMessage = "Tên tối thiểu 2 ký tự.")]
        public string? Name { get; set; }
        [MaxLength(50, ErrorMessage = "Tên tối đa 50 ký tự.")]
        [MinLength(2, ErrorMessage = "Tên tối thiểu 2 ký tự.")]
        public string? Note { get; set; }
        public UnitEnum Type { get; set; }
    }
}
