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
    [Table("CellBatch")]
    public class CellBatch : BaseEntity
    {
        public int CurrentStock { get; set; }
        public ICollection<InOutDetail> InOutDetails { get; set; } = [];
        public ICollection<InventoryCheckDetail> InventoryCheckDetails { get; set; } = [];

        [ForeignKey("Cell")]
        public string CellId { get; set; }
        public Cell Cell { get; set; }

        [ForeignKey("Unit")]
        public string UnitId { get; set; }
        public Unit Unit { get; set; }
    }
}
