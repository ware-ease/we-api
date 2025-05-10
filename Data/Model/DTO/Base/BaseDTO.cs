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
        //public BaseDTOExtended? BaseDTOExtended { get; set; }
        public string? CreatedByAvatarUrl { get; set; }
        public string? CreatedByFullName { get; set; }
        public string? CreatedByGroup { get; set; }

    }
    public class BaseDTOExtended
    {
        public string? CreatedByAvatarUrl { get; set; }
        public string? CreatedByFullName { get; set; }
        public string? CreatedByGroup { get; set; }
    }
}
