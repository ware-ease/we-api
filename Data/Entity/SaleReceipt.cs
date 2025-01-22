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
    [Table("SaleReceipt")]
    public class SaleReceipt : BaseEntity
    {
        [Required]
        public DateTime Date { get; set; }
    }
}
