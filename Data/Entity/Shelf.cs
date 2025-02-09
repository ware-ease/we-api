using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entity.Base;

namespace Data.Entity
{
    [Table("Shelf")]
    public class Shelf : BaseEntity
    {
        public int Number { get; set; }
        public int FloorNumber { get; set; }
        public ICollection<Floor> Floors { get; set; }

        [ForeignKey("Warehouse")]
        public string WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
