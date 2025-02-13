using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.Group
{
    public class UpdateGroupDTO
    {
        public string Name { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
    public class DeleteGroupDTO
    {
        public string? DeletedBy { get; set; }
    }
}
