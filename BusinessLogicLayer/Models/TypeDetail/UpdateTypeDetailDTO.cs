using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.TypeDetail
{
    public class UpdateTypeDetailDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
