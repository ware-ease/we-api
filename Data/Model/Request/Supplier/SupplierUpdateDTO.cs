using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Supplier
{
    public class SupplierUpdateDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public string? Name { get; set; }
        [Phone]
        public string? Phone { get; set; }
        [JsonIgnore]
        public bool? Status { get; set; }
    }
}
