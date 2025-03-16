using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("Location")]
    public class Location : BaseEntity
    {
        public int Level { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }

        public ICollection<ScheduleSetting> ScheduleSettings { get; set; } = [];
        public ICollection<Schedule> Schedules { get; set; } = [];
        public ICollection<LocationLog> LocationLogs { get; set; } = [];
        public ICollection<InventoryCount> InventoryCounts { get; set; } = [];

        [ForeignKey("Warehouse")]
        public string WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        [ForeignKey("Parent")]
        public string? ParentId { get; set; }
        public Location? Parent { get; set; }
    }
}
