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
    [Table("PurchaseDetail")]
    public class PurchaseDetail : BaseEntity
    {
        public int Quanlity { get; set; }
        public float Price { get; set; }

        [ForeignKey("PurchaseReceipt")]
        public string ReceiptId { get; set; }
        public PurchaseReceipt PurchaseReceipt { get; set; }

        [ForeignKey("ProductType")]
        public string ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }
    }
}
