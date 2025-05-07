using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Data.Model.Request.Schedule
{
    public class ScheduleCreateDTO : BaseCreateDTO
    {
        [Required(ErrorMessage = "Date không được để trống")]
        public DateOnly? Date { get; set; }
        //[Required(ErrorMessage = "Name không được để trống")]
        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public TimeOnly? StartTime { get; set; }
        //[Required(ErrorMessage = "Name không được để trống")]
        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public TimeOnly? EndTime { get; set; }
        [Required(ErrorMessage = "WarehouseId không được để trống")]
        public string WarehouseId { get; set; }

        public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
        {
            private readonly string _format = "HH:mm";

            public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var timeString = reader.GetString();
                if (TimeOnly.TryParseExact(timeString, _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
                {
                    return time;
                }
                throw new JsonException($"Không thể chuyển đổi \"{timeString}\" thành TimeOnly.");
            }

            public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString(_format));
            }
        }
    }
}
