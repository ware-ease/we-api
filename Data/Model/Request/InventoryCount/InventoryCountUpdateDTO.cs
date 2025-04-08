using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Data.Model.Request.Schedule.ScheduleCreateDTO;

namespace Data.Model.Request.InventoryCount
{
    public class InventoryCountUpdateDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        //[Required(ErrorMessage = "Status không được để trống")]
        [JsonIgnore]
        public InventoryCountStatus? Status { get; set; }
        //[Required(ErrorMessage = "Code không được để trống")]
        public string? Code { get; set; }
        public string? Note { get; set; }
        //[Required(ErrorMessage = "Date không được để trống")]
        public DateOnly? Date { get; set; }
        //[Required(ErrorMessage = "StartTime không được để trống")]
        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public TimeOnly? StartTime { get; set; }
        //[Required(ErrorMessage = "EndTime không được để trống")]
        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public TimeOnly? EndTime { get; set; }
        //[Required(ErrorMessage = "ScheduleId không được để trống")]
        public string? ScheduleId { get; set; }
        //[Required(ErrorMessage = "LocationId không được để trống")]
        public string? LocationId { get; set; }
        public List<InventoryCountDetailUpdateDTO>? InventoryCountDetails { get; set; } = new();
    }

    public class InventoryCountDetailUpdateDTO
    {
        //[JsonIgnore]
        public string? Id { get; set; }
        //[Required(ErrorMessage = "ExpectedQuantity không được để trống")]
        [JsonIgnore]
        public float? ExpectedQuantity { get; set; }
        //[Required(ErrorMessage = "CountedQuantity không được để trống")]
        public float? CountedQuantity { get; set; }
        public string? Note { get; set; }
        //[Required(ErrorMessage = "ProductId không được để trống")]
        public string? InventoryId { get; set; }
        public string? ErrorTicketId { get; set; }
    }
}
