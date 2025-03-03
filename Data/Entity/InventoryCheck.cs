using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table ("InventoryCheck")]
    public class InventoryCheck : BaseEntity
    {
        public bool Status { get; set; }
        public DateOnly? Date {  get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        public ICollection<InventoryCheckDetail> InventoryCheckDetails { get; set; } = [];

        [ForeignKey("Schedule")]
        public string ScheduleId { get; set; }
        public Schedule Schedule { get; set; }

        public ErrorTicket? ErrorTicket { get; set; }
    }
}
