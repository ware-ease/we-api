using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.Permission
{
    public class UpdatePermissionDTO
    {
        public string Url { get; set; }
        public string? LastUpdatedBy { get; set; }

    }
}
