using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.TypeDetail
{
    public class CreateTypeDetailDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CreatedBy { get; set; }
    }
}
