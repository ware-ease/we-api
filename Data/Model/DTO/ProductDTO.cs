using Data.Entity;
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
        public string Barcode { get; set; }
        public string Sku { get; set; }
        public CategoryDTO Category { get; set; }
        public BrandDTO Brand { get; set; }
        public UnitDTO Unit { get; set; }
    }
}
