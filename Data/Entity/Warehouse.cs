using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("Warehouse")]
    public class Warehouse : BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public int? ShelfCount { get; set; }
        public string? ParentId { get; set; }

        public ICollection<AccountWarehouse> AccountWarehouses { get; set; }
        public ICollection<Area> Areas { get; set; }
        public ICollection<Inventory> Inventories { get; set; }
        public ICollection<StockBook> StockBooks { get; set; }

    }
}
