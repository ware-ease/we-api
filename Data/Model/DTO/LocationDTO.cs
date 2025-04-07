using Data.Entity;
using Data.Model.DTO.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class LocationDTO : BaseDTO
    {
        public int Level { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string WarehouseId { get; set; }
        public string? ParentId { get; set; }
    }
}
