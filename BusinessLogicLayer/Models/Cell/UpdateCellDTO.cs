using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.Cell
{
    public class UpdateCellDTO
    {
        public int Number { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public float MaxLoad { get; set; }
        public string FloorId { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
