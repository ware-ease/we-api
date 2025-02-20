using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.Shelf
{
    public class UpdateShelfDTO
    {
        public int Number { get; set; }
        public int FloorNumber { get; set; }
        public string WarehouseId { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
