using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.ProductType
{
    public class ProductTypeCreateDTO : BaseCreateDTO
    {
        public string Name { get; set; }
        public string? Note { get; set; }
        public string CategoryId { get; set; }
    }
}
