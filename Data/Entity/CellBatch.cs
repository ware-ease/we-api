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
        public string Name { get; set; }
        public string Unit { get; set; }
        public DateTime Date { get; set; }
        public ICollection<InOutDetail> InOutDetails { get; set; }
        public ICollection<InventoryCheckDetail> InventoryCheckDetails { get; set; }

        [ForeignKey("Cell")]
        public string CellId { get; set; }
        public Cell Cell { get; set; }
    }
}
