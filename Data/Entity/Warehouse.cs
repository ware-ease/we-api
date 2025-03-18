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
        public string? Name { get; set; }
        public string? Address { get; set; }
        public float Area { get; set; }
        public DateTime OperateFrom { get; set; }

        public ICollection<AccountWarehouse> AccountWarehouses { get; set; } = [];
        public ICollection<Location> Locations { get; set; } = [];
        public ICollection<ReceivingNote> ReceivingNotes { get; set; }
        public ICollection<IssueNote> IssueNotes { get; set; } = [];
        public ICollection<Inventory> Inventories { get; set; } = [];
        public ICollection<StockBook> StockBooks { get; set; } = [];
        public ICollection<GoodRequest> GoodRequests { get; set; } = [];
        public ICollection<GoodRequest> RequestedGoodRequests { get; set; } = [];
        public ICollection<InventoryAdjustment> InventoryAdjustments { get; set; } = [];
    }
}
