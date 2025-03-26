using Data.Entity.Base;
using Data.Model.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class WarehouseDTO : BaseDTO
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public float? Area { get; set; }
        public DateTime? OperateFrom { get; set; }
    }
}
