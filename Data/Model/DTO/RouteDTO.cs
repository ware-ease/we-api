using Data.Model.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class RouteDTO : BaseDTO
    {
        public string Url { get; set; }
        public string Code { get; set; }
    }
}
