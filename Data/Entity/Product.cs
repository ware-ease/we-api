using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entity.Base;
using Data.Enum;

namespace Data.Entity
{
    [Table("Product")]
    public class Product : BaseEntity
    {
        public string? Name { get; set; }
        public string? Sku {  get; set; }
        public ProductQuantityTypeEnum QuantityType { get; set; } = ProductQuantityTypeEnum.Countable;
        public string? imageUrl { get; set; }
        public ICollection<Batch> Batches { get; set; } = [];
        public ICollection<StockBook> StockBooks { get; set; } = [];
        public ICollection<GoodRequestDetail> GoodRequestDetails { get; set; } = [];
        //public ICollection<InventoryCountDetail> InventoryCountDetails { get; set; } = [];

        [ForeignKey("ProductType")]
        public string ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }

        [ForeignKey("Unit")]
        public string UnitId { get; set; }
        public Unit Unit { get; set; }

        [ForeignKey("Brand")]
        public string BrandId { get; set; }
        public Brand Brand { get; set; }
    }
}
