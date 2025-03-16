using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    public class GoodRequestDetail : BaseEntity
    {
        public float Quantity { get; set; }

        [ForeignKey("GoodRequest")]
        public string GoodRequestId { get; set; }
        public GoodRequest GoodRequest { get; set; }

        [ForeignKey("Product")]
        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}
