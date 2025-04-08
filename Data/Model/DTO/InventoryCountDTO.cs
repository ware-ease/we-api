using Data.Model.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class InventoryCountDTO : BaseDTO
    {
        public string Id { get; set; }
        public bool Status { get; set; }
        public string? Code { get; set; }
        public string? Note { get; set; }
        public DateOnly? Date { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public DateOnly? ScheduleDate { get; set; }
        public string LocationName { get; set; }
        public string LocationId { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseId { get; set; }
        public List<InventoryCountDetailDTO> InventoryCountDetailDTO { get; set; }
    }

    public class InventoryCountDetailDTO : BaseDTO
    {
        public string Id { get; set; }
        public float ExpectedQuantity { get; set; }
        public float CountedQuantity { get; set; }
        public string? Note { get; set; }
        public string ProductName { get; set; }
    }
}
