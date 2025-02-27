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
        public string Number { get; set; }
        public int FloorNumber { get; set; }
        public ICollection<Shelf> Shelves { get; set; }

        [ForeignKey("Warehouse")]
        public string WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        [ForeignKey("Schedule")]
        public string ScheduleId { get; set; }
        public Schedule Schedule { get; set; }

        [ForeignKey("ScheduleSetting")]
        public string ScheduleSettingId { get; set; }
        public ScheduleSetting ScheduleSetting { get; set; }
    }
}
