using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entity.Base;

namespace Data.Entity
{
    [Table("Shelf")]
    public class Shelf : BaseEntity
    {
        public string? Code { get; set; }
        public ICollection<Floor> Floors { get; set; } = [];

        [ForeignKey("Area")]
        public string AreaId { get; set; }
        public Area Area { get; set; }
    }
}
