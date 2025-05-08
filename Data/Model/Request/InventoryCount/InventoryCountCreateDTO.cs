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
    public class InventoryCountCreateDTO : BaseCreateDTO
    {
        [Required(ErrorMessage = "Status không được để trống")]
        [JsonIgnore]
        public InventoryCountStatus Status { get; set; } = InventoryCountStatus.Inprogress;
        //[Required(ErrorMessage = "CheckStatus không được để trống")]
        [JsonIgnore]
        public InventoryCountCheckStatus CheckStatus { get; set; } = InventoryCountCheckStatus.Incomplete;
        [Required(ErrorMessage = "Code không được để trống")]
        public string? Code { get; set; }
        public string? Note { get; set; }
        [Required(ErrorMessage = "Date không được để trống")]
        public DateOnly? Date { get; set; }
        [Required(ErrorMessage = "StartTime không được để trống")]
        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public TimeOnly? StartTime { get; set; }
        [Required(ErrorMessage = "EndTime không được để trống")]
        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public TimeOnly? EndTime { get; set; }
        public string WarehouseId { get; set; }
        //[Required(ErrorMessage = "ScheduleId không được để trống")]
        [JsonIgnore]
        public string? ScheduleId { get; set; }
        /*[Required(ErrorMessage = "LocationId không được để trống")]
        public string LocationId { get; set; }*/
        public List<InventoryCountDetailCreateDTO> InventoryCountDetails { get; set; } = new();
    }

    public class InventoryCountDetailCreateDTO : BaseCreateDTO
    {
        //[Required(ErrorMessage = "Status không được để trống")]
        [JsonIgnore]
        public InventoryCountDetailStatus Status { get; set; } = InventoryCountDetailStatus.Uncounted;
        //[Required(ErrorMessage = "ExpectedQuantity không được để trống")]
        [JsonIgnore]
        public float ExpectedQuantity { get; set; }
        //[Required(ErrorMessage = "CountedQuantity không được để trống")]
        [JsonIgnore]
        public float? CountedQuantity { get; set; }
        public string? Note { get; set; }
        [Required(ErrorMessage = "AccountId không được để trống")]
        public string? AccountId { get; set; }
        [Required(ErrorMessage = "InventoryId không được để trống")]
        public string InventoryId { get; set; }
        public string? ErrorTicketId { get; set; }
    }
}
