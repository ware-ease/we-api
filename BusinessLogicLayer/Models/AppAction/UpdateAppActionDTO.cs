using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.AppAction
{
    public class UpdateAppActionDTO
    {
        public string Code { get; set; }
        public string PermissionId { get; set; }

        public string? LastUpdatedBy { get; set; }
    }
    public class DeleteAppActionDTO
    {
        public string? DeletedBy { get; set; }
    }
}
