using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("SaleDetail")]
    public class SaleDetail : BaseEntity
    {
        [Required]
        public int Quanlity { get; set; }
        [Required]
        public float Price { get; set; }

        [ForeignKey("SaleReceipt")]
        public string ReceiptId { get; set; }
        public SaleReceipt SaleReceipt { get; set; }

        [ForeignKey("ProductType")]
        public string ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }
    }
}
