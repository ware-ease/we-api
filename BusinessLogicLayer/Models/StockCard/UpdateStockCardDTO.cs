using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.StockCard
{
    public class UpdateStockCardDTO
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public DateTime Date { get; set; }
        public string CellId { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
