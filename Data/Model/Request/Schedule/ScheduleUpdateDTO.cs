using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Data.Model.Request.Schedule.ScheduleCreateDTO;

namespace Data.Model.Request.Schedule
{
    public class ScheduleUpdateDTO
    {
        [JsonIgnore]
        public string Id { get; set; }
        public DateOnly? Date { get; set; }
        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public TimeOnly? StartTime { get; set; }
        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public TimeOnly? EndTime { get; set; }
        public string WarehouseId { get; set; }
    }
}
