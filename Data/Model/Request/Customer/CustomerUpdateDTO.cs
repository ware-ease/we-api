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
        [Required]
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public bool Status { get; set; }
    }
}
