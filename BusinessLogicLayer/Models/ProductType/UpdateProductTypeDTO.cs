using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.ProductType
{
    public class UpdateProductTypeDTO
    {
        public string Name { get; set; }
        public string ProductId { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
