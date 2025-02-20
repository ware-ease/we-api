using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.Floor
{
    public class UpdateFloorDTO
    {
        public int Number { get; set; }
        public string ShelfId { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
