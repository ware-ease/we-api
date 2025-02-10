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
    public class ProductTypeTypeDetail : BaseEntity
    {
        [ForeignKey("TypeDetail")]
        public string TypeDetailId { get; set; }
        public TypeDetail TypeDetail { get; set; }

        [ForeignKey("ProductType")]
        public string? ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }
    }
}
