using Data.Entity;
using Data.Entity.Base;
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
    public class CreateWarehouseDTO
    {
        [Required(ErrorMessage = "Warehouse name is required.")]
        [MaxLength(255, ErrorMessage = "Warehouse name cannot exceed 255 characters.")]
        public string? Name { get; set; }
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
        public string? Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public float Area { get; set; }
        public DateTime OperateFrom { get; set; }

        public List<LocationDto>? Locations { get; set; }
    }
    public class LocationDto
    {
        public string Id { get; set; }
        public int Level { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }

        public string? ParentId { get; set; }
    }

    public class LocationCreateDto
    {
        public int Level { get; set; }
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
        public string? Address { get; set; }
        public float Area { get; set; }
        public DateTime OperateFrom { get; set; }
        public IEnumerable<InventoryDTO>? Inventories { get; set; }
    }
}
