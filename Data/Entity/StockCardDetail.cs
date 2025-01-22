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
        [Required]
        public string In {  get; set; }
        [Required]
        public string Out { get; set; }
        [Required]
        public string Stock {  get; set; }
        [Required]
        public DateTime Date { get; set; }

        [ForeignKey("StockCard")]
        public string StockCardId { get; set; }
        public StockCard StockCard { get; set; }

        [ForeignKey("ProductType")]
        public string ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }
    }
}
