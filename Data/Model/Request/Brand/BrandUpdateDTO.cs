using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Brand
{
    public class BrandUpdateDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
