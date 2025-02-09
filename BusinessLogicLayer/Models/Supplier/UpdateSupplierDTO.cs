using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.Supplier
{
    public class UpdateSupplierDTO
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        [Required(ErrorMessage = "Trạng thái không được để trống")]
        public bool Status { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
