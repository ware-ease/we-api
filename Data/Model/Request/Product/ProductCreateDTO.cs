using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.Product
{
    public class ProductCreateDTO : BaseCreateDTO
    {
        [Required(ErrorMessage = "Name không được để trống")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Sku không được để trống")]
        public string? Sku { get; set; }
        public string? imageUrl { get; set; }
        [Required(ErrorMessage = "ProductTypeId không được để trống")]
        public string ProductTypeId { get; set; }
        [Required(ErrorMessage = "BrandId không được để trống")]
        public string BrandId { get; set; }
        [Required(ErrorMessage = "UnitId không được để trống")]
        public string UnitId { get; set; }
    }
}
