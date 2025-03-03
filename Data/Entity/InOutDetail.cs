using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entity.Base;
using Data.Enum;

namespace Data.Entity
{
    [Table("InOutDetail")]
    public class InOutDetail : BaseEntity
    {
        public string? Code { get; set; }
        public ActionEnum Action {  get; set; }
        public int? Stock {  get; set; }
        public int? Quantity {  get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("CellBatch")]
        public string? CellBatchId { get; set; }
        public CellBatch? CellBatch { get; set; }

        [ForeignKey("Batch")]
        public string BatchId { get; set; }
        public Batch Batch { get; set; }
    }
}
