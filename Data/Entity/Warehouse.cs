using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("Warehouse")]
    public class Warehouse : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public float Length { get; set; }
        [Required]
        public float Width { get; set; }
        [Required]
        public int ShelfCount { get; set; }
        public string? ParentId { get; set; }

        public ICollection<AccountWarehouse> AccountWarehouses { get; set; }
        public ICollection<Shelf> Shelves { get; set; }

    }
}
