using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.Permission
{
    public class PermissionDTO : BaseEntity
    {
        public string Url { get; set; }
    }
}
