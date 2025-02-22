using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.StockCard
{
    public class CreateStockCardDTO
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public DateTime Date { get; set; }
        public string CreatedBy { get; set; }
    }
}
