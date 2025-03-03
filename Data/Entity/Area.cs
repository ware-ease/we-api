using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("Area")]
    public class Area : BaseEntity
    {
        public string? Name { get; set; }

        public ICollection<Shelf> Shelves { get; set; } = [];
        public ICollection<Schedule> Schedules { get; set; } = [];
        public ICollection<ScheduleSetting> ScheduleSettings { get; set; } = [];

        [ForeignKey("Warehouse")]
        public string WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
