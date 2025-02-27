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
        public DateOnly? Day {  get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        [ForeignKey("Area")]
        public string AreaId { get; set; }
        public Area Area { get; set; }

    }
}
