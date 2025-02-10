using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.Category
{
    public class UpdateCategoryDTO
    {
        public string Name { get; set; }
        public string Note { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
