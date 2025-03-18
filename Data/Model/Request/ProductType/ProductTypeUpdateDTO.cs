using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.ProductType
{
    public class ProductTypeUpdateDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string CategoryId { get; set; }
    }
}
