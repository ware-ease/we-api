using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("ScheduleSetting")]
    public class ScheduleSetting : BaseEntity
    {
        public string DaysOfWeek {  get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        [ForeignKey("Location")]
        public string LocationId { get; set; }
        public Location Location { get; set; }
    }
}
