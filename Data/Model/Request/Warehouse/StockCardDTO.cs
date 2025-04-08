using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.Warehouse
{
    public class StockCardDTO
    {
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? UnitName { get; set; }
        public string? WarehouseName { get; set; }
        public List<StockCardDetailDTO> Details { get; set; }
    }

    public class StockCardDetailDTO
    {
        public DateTime? Date { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public float? Import { get; set; }
        public float? Export { get; set; }
        public float? Stock { get; set; }
        public string? Note { get; set; }
    }

}
