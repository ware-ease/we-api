using Data.Model.DTO;
using Data.Model.DTO.Base;
using Data.Model.Request.GoodNote;
using Data.Model.Request.Inventory;
using Data.Model.Request.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryDTO = Data.Model.DTO.InventoryDTO;

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
        public int Quantity { get; set; } = 0;
        public string? Note { get; set; }

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
    public class LocationOfInventoryDTO : BaseDTO
    {
        public int Quantity { get; set; } = 0;
        public InventoryDTOv2 Inventory { get; set; }
        public LocationDTO Location { get; set; }
    }
    public class InventoryDTOv2 : BaseDTO
    {
        public float CurrentQuantity { get; set; }
        public float? ArrangedQuantity { get; set; }
        public float? NotArrgangedQuantity { get; set; }
        public string WarehouseId { get; set; }
        public WarehouseDTO Warehouse { get; set; }
        public string BatchId { get; set; }
        public BatchNoteDTO Batch { get; set; }
        //public List<InventoryLocationDTO> InventoryLocations { get; set; } = [];
    }
    public class InventoryLocationDTO : BaseDTO
    {
        public int Quantity { get; set; } 
        public LocationDTO Location { get; set; }
    }
}
