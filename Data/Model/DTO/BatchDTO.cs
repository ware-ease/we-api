using Data.Model.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class BatchDTO : BaseDTO
    {
        //public string? SupplierName { get; set; }
        //public string ProductName { get; set; }
        public string ProductId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public DateOnly InboundDate { get; set; }
        public DateOnly MfgDate { get; set; }
        public DateOnly ExpDate { get; set; }
        public ProductDTO Product { get; set; }
        //public string? InventoryId { get; set; }
    }
}
