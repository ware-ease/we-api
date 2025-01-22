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
        [Required]
        public int Number { get; set; }
        [Required]
        public float Length { get; set; }
        [Required]
        public float Height { get; set; }
        [Required]
        public float MaxLoad { get; set; }
        public ICollection<StockCard> StockCards { get; set; }
        public ICollection<Product> Products { get; set; }

        [ForeignKey("Floor")]
        public string FloorId { get; set; }
        public Floor Floor { get; set; }
    }
}
