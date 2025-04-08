using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.ErrorTicket
{
    public class ErrorTicketUpdateDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public string? Reason { get; set; }
        public string? Code { get; set; }
        public string? Note { get; set; }
        public string? HandleBy { get; set; }
        public string? InventoryCountDetailId { get; set; }
    }
}
