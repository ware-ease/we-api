using Data.Entity;
using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Warehouse
{
    public class WarehouseDTO : BaseEntity
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public int? ShelfCount { get; set; }
    }

    public class CreateWarehouseDTO
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public int? ShelfCount { get; set; }

    }

    public class UpdateWarehouseDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public int? ShelfCount { get; set; }

    }

    #region WarehouseDTO Custom
    public class WarehouseFullInfoDTO : BaseEntity
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public int? ShelfCount { get; set; }

        public List<AreaDto> Areas { get; set; }
    }
    public class AreaDto
    {
        public string Id { get; set; }
        public string? Name { get; set; }

        public List<ShelfDto> Shelves { get; set; }
    }
    public class ShelfDto
    {
        public string Id { get; set; }
        public string? Code { get; set; }

        public List<FloorDto> Floors { get; set; }
    }
    public class FloorDto
    {
        public string Id { get; set; }
        public int Number { get; set; }

        public List<CellDto> Cells { get; set; }
    }
    public class CellDto
    {
        public string Id { get; set; }
        public int Number { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public float MaxLoad { get; set; }
    }
    #endregion

    public class CreateWarehouseStructureRequest
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public List<CreateAreaDto> Areas { get; set; }
    }

    public class CreateAreaDto
    {
        public string Name { get; set; }
        public List<CreateShelfDto> Shelves { get; set; }
    }

    public class CreateShelfDto
    {
        public string Code { get; set; }
        public List<CreateFloorDto> Floors { get; set; }
    }

    public class CreateFloorDto
    {
        public int Number { get; set; }
        public List<CreateCellDto> Cells { get; set; }
    }

    public class CreateCellDto
    {
        public int Number { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public float MaxLoad { get; set; }
    }

}
