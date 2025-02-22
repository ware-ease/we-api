using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.StockCardDetail
{
    public class CreateStockCardDetailDTO
    {
        public string? In { get; set; }
        public string? Out { get; set; }
        public string Stock { get; set; }
        public DateTime Date { get; set; }
        public string CreatedBy { get; set; }
    }
}
