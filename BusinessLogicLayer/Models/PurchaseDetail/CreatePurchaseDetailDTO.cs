using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.PurchaseDetail
{
    public class CreatePurchaseDetailDTO
    {
        public int Quanlity { get; set; }
        public float Price { get; set; }
        public string CreatedBy { get; set; }
    }
}
