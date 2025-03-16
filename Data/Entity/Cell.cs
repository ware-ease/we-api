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
    [Table("Cell")]
    public class Cell : BaseEntity
    {
        public int Number { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public float MaxLoad { get; set; }

        [ForeignKey("Floor")]
        public string FloorId { get; set; }
        public Floor Floor { get; set; }
    }
}
