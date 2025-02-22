using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.StockCardDetail
{
    public class UpdateStockCardDetailDTO
    {
        public string? In { get; set; }
        public string? Out { get; set; }
        public string Stock { get; set; }
        public DateTime Date { get; set; }
        public string LastUpdatedBy { get; set; }
        /*public string StockCardId { get; set; }
        public string ProductTypeId { get; set; }*/
    }
}
