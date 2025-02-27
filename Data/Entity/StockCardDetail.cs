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
    [Table("StockCardDetail")]
    public class StockCardDetail : BaseEntity
    {
        public string Code { get; set; }
        public string Action {  get; set; }
        public string Stock {  get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("StockCard")]
        public string StockCardId { get; set; }
        public StockCard StockCard { get; set; }

        [ForeignKey("Batch")]
        public string BatchId { get; set; }
        public Batch Batch { get; set; }
    }
}
