using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Batch
{
    public class BatchUpdateDTO
    {
        [JsonIgnore]
        public string Id { get; set; }
        public string? SupplierId { get; set; }
        public string ProductId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        [Required(ErrorMessage = "MfgDate không được để trống")]
        [DataType(DataType.Date)]
        public DateOnly MfgDate { get; set; }
        [Required(ErrorMessage = "ExpDate không được để trống")]
        [DataType(DataType.Date)]
        public DateOnly ExpDate { get; set; }
        public string? InventoryId { get; set; }
    }
}
