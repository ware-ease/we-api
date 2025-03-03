using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entity.Base;

namespace Data.Entity
{
    [Table("Product")]
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string BarCode  { get; set; }
        public string Sku {  get; set; }
        public ICollection<Batch> Batches { get; set; }
        public ICollection<StockBook> StockBooks { get; set; }

        [ForeignKey("Category")]
        public string CategoryId { get; set; }
        public Category Category { get; set; }

        [ForeignKey("Inventory")]
        public string InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        [ForeignKey("Unit")]
        public string UnitId { get; set; }
        public Unit Unit { get; set; }

        [ForeignKey("Brand")]
        public string BrandId { get; set; }
        public Brand Brand { get; set; }
    }
}
