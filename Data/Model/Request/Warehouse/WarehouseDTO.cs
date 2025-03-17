using Data.Entity;
using Data.Entity.Base;
using Data.Model.DTO.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Warehouse
{
    public class CreateWarehouseDTO
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }

    }

    public class UpdateWarehouseDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }

    }

    #region WarehouseDTO Custom
    public class WarehouseFullInfoDTO : BaseDTO
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }

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
}
