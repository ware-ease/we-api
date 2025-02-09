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
    [Table("Floor")]
    public class Floor : BaseEntity
    {
        public int Number { get; set; }

        [ForeignKey("Shelf")]
        public string ShelfId { get; set; }
        public Shelf Shelf { get; set; }
    }
}
