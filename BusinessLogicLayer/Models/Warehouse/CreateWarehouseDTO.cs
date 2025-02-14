using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.Warehouse
{
    public class CreateWarehouseDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public int? ShelfCount { get; set; }
        public string? ParentId { get; set; }
        public string? CreatedBy { get; set; }

    }
}
