using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class AppActionDTO : BaseEntity
    {
        public string Code { get; set; }
        public string PermissionId { get; set; }

    }
}
