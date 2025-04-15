using Data.Entity;
using Data.Enum;
using Data.Model.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class ProductDTO : BaseDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? imageUrl { get; set; }
        //public string Barcode { get; set; }
        public string Sku { get; set; }
        public string ProductType { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public string Unit { get; set; }
    }
}
