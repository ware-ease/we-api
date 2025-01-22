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
    [Table("StockCard")]
    public class StockCard : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Unit { get; set; }
        [Required]
        public DateTime Date { get; set; }

        [ForeignKey("Cell")]
        public string CellId { get; set; }
        public Cell Cell { get; set; }
    }
}
