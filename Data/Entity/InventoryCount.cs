using Data.Entity.Base;
using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table ("InventoryCount")]
    public class InventoryCount : BaseEntity
    {
        public InventoryCountStatus Status { get; set; }
        public string? Code { get; set; }
        public string? Note { get; set; }
        public DateOnly? Date {  get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        public ICollection<InventoryCountDetail> InventoryCheckDetails { get; set; } = [];

        /*[ForeignKey("Schedule")]
        public string? ScheduleId { get; set; }
        public Schedule? Schedule { get; set; }*/

        /*[ForeignKey("Location")]
        public string LocationId { get; set; }
        public Location Location { get; set; } //level 0 only*/
    }
}
