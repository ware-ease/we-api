using Data.Model.DTO.Base;
using Data.Model.Request.Inventory;
using Data.Model.Request.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.InventoryLocation
{
    public class InventoryInLocationDTO : BaseDTO
    {
        public int Quantity { get; set; } = 0;
        public InventoryDTO Inventory { get; set; }
    }

    public class CreateInventoryLocationDTO : BaseCreateDTO
    {
        [Required(ErrorMessage = "InventoryId is required.")]
        public string InventoryId { get; set; }

        [Required(ErrorMessage = "LocationId is required.")]
        public string LocationId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; } = 0;
    }
    public class LocationInventoryDTO : BaseDTO
    {
        public int Level { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? ParentId { get; set; }
        public string WarehouseId { get; set; }
        public ICollection<InventoryInLocationDTO>? InventoryItems { get; set; } 
    }

}
