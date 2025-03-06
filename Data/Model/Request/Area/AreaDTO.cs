using Data.Entity;
using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Area
{
    public class AreaDTO : BaseEntity
    {
        public string? Name { get; set; }
    }
    //DTO chỉ tạo các khu vực trong 1 kho
    public class CreateAREADto
    {
        public string Name { get; set; }
        [JsonIgnore]
        public string? WarehouseId { get; set; }

    }
}
