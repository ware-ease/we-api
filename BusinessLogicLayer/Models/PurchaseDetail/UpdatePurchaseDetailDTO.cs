using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.PurchaseDetail
{
    public class UpdatePurchaseDetailDTO
    {
        public int Quanlity { get; set; }
        public float Price { get; set; }
        public string ReceiptId { get; set; }
        public string ProductTypeId { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
