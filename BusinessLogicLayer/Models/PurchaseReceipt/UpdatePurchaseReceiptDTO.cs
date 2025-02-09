using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.PurchaseReceipt
{
    public class UpdatePurchaseReceiptDTO
    {
        public DateTime? Date { get; set; }
        public string SupplierId { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
