using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.AppAction
{
    public class CreateAppActionDTO
    {
        public string Code { get; set; }
        public string PermissionId { get; set; }

        public string? CreatedBy { get; set; }


    }
}
