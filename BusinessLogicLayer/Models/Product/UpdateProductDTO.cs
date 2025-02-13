using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.Product
{
    public class UpdateProductDTO
    {
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
