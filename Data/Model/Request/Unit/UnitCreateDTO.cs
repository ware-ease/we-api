using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.Unit
{
    public class UnitCreateDTO : BaseCreateDTO
    {
        [Required]
        public string? Name { get; set; }
        public string? Note { get; set; }
        public UnitEnum Type { get; set; }
    }
}
