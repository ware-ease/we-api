using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("Schedule")]
    public class Schedule : BaseEntity
    {
        public DateOnly? Date { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        [ForeignKey("Location")]
        public string LocationId { get; set; }
        public Location Location { get; set; }

        public InventoryCheck? InventoryCheck { get; set; }
    }
}
