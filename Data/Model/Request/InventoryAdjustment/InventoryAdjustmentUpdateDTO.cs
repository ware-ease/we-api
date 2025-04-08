using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.InventoryAdjustment
{
    public class InventoryAdjustmentUpdateDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Reason { get; set; }
        public string? Note { get; set; }
        public string? RelatedDocument { get; set; }
        public string? WarehouseId { get; set; }
        public List<InventoryAdjustmentDetailUpdateDTO>? InventoryAdjustmentDetails { get; set; }
    }

    public class InventoryAdjustmentDetailUpdateDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public float? NewQuantity { get; set; }
        public float? ChangeInQuantity { get; set; }
        public string? Note { get; set; }
        /*public string? LocationId { get; set; }
        public string? InventoryId { get; set; }*/
    }
}
