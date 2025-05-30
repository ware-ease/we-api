using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Customer
{
    public class CustomerUpdateDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        [MaxLength(100, ErrorMessage = "Tên tối đa 100 ký tự.")]
        [MinLength(2, ErrorMessage = "Tên tối thiểu 2 ký tự.")]
        public string? Name { get; set; }
        [Phone]
        public string? Phone { get; set; }
        [JsonIgnore]
        public bool? Status { get; set; }
    }
}
