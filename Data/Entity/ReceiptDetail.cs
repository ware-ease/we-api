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
    [Table("ReceiptDetail")]
    public class ReceiptDetail : BaseEntity
    {
        [Required]
        public int Quanlity { get; set; }
        [Required]
        public float Price { get; set; }

        [ForeignKey("PurchaseReceipt")]
        public string ReceiptId { get; set; }
        public PurchaseReceipt PurchaseReceipt { get; set; }

        [ForeignKey("ProductType")]
        public string ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }
    }
}
