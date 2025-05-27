using Data.Entity;
using Data.Entity.Base;
using Data.Enum;
using Data.Model.DTO.Base;
using Data.Model.Request.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Warehouse
{
    public class CreateWarehouseDTO : BaseCreateDTO
    {
        [Required(ErrorMessage = "Warehouse name is required.")]
        [MaxLength(255, ErrorMessage = "Warehouse name cannot exceed 255 characters.")]
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        [Required(ErrorMessage = "Warehouse Latitude is required.")]
        public decimal? Latitude { get; set; }
        [Required(ErrorMessage = "Warehouse Longtitude is required.")]
        public decimal? Longitude { get; set; }
        [Range(1, float.MaxValue, ErrorMessage = "Area must be greater than 0.")]
        public float Area { get; set; }
        [Required(ErrorMessage = "OperateFrom date is required.")]
        [DataType(DataType.Date)]
        public DateTime OperateFrom { get; set; }

    }

    public class UpdateWarehouseDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public float? Area { get; set; }
        public DateTime? OperateFrom { get; set; }

    }

    #region WarehouseDTO Custom
    public class WarehouseFullInfoDTO : BaseDTO
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public float Area { get; set; }
        public DateTime OperateFrom { get; set; }

        //public List<LocationDto>? Locations { get; set; }
    }
    public class LocationDto : BaseDTO
    {
        //public string Id { get; set; }
        public int Level { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }

        public string? ParentId { get; set; }
    }

    public class LocationCreateDto
    {
        [JsonIgnore]
        public int Level { get; set; } = 0; // Default level is 0
        public string? Name { get; set; }
        public string? Code { get; set; }

        public string? ParentId { get; set; }  
    }

    public class CreateWarehouseStructureRequest
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public List<LocationCreateDto> Locations { get; set; }
    }
    #endregion

    public class WarehouseInventoryDTO : BaseDTO
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public float Area { get; set; }
        public DateTime OperateFrom { get; set; }
        public IEnumerable<InventoryDTO>? Inventories { get; set; }
    }
    public class ProductWithQuantityDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? imageUrl { get; set; }
        public string Sku { get; set; }
        public bool IsBatchManaged { get; set; }
        public string ProductTypeName { get; set; }
        public string CategoryName { get; set; }

        public string UnitName { get; set; }
        public UnitEnum UnitType { get; set; }
        public string BrandName { get; set; }

        public double TotalQuantity { get; set; }
    }

}
