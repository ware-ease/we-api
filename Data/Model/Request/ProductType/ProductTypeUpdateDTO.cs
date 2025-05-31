using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [MaxLength(100, ErrorMessage = "Tên tối đa 100 ký tự.")]
        public string? Name { get; set; }
        [MaxLength(200, ErrorMessage = "Note tối đa 200 ký tự.")]
        public string? Note { get; set; }
        public string? CategoryId { get; set; }
    }
}
