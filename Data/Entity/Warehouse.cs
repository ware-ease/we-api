﻿using Data.Entity.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entity
{
    [Table("Warehouse")]
    public class Warehouse : BaseEntity
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public float Area { get; set; }
        [Column(TypeName = "decimal(11,8)")]
        public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(11,8)")]
        public decimal? Longitude { get; set; }
        public DateTime OperateFrom { get; set; }

        public ICollection<AccountWarehouse> AccountWarehouses { get; set; } = [];
        public ICollection<Inventory> Inventories { get; set; } = [];
        public ICollection<StockBook> StockBooks { get; set; } = [];
        public ICollection<GoodRequest> GoodRequests { get; set; } = [];
        public ICollection<GoodRequest> RequestedGoodRequests { get; set; } = [];
        public ICollection<InventoryAdjustment> InventoryAdjustments { get; set; } = [];
        public ICollection<Schedule> Schedules { get; set; } = [];
    }
}
