using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.PurchaseReceipt
{
    public class CreatePurchaseReceiptDTO
    {
        public DateTime? Date {  get; set; }
        public string CreatedBy { get; set; }
    }
}
