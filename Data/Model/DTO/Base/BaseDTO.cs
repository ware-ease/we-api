using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO.Base
{
    public class BaseDTO
    {
        public string? Id { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedTime { get; set; }
    }
}
