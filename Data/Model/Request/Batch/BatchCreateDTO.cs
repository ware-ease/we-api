using Data.Model.DTO.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.Batch
{
    public class BatchCreateDTO : BaseCreateDTO
    {
        //[Required(ErrorMessage = "SupplierId không được để trống")]
        //public string? SupplierId { get; set; }
        [Required(ErrorMessage = "Id sản phẩm không được để trống")]
        public string ProductId { get; set; }
        [Required(ErrorMessage = "Mã lô không được để trống")]
        public string? Code { get; set; }
        [Required(ErrorMessage = "Tên lô không được để trống")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "InboundDate không được để trống")]
        public DateOnly? InboundDate { get; set; }
        //[Required(ErrorMessage = "MfgDate không được để trống")]
        //[DataType(DataType.Date)]
        //public DateOnly MfgDate { get; set; }
        //[Required(ErrorMessage = "ExpDate không được để trống")]
        //[DataType(DataType.Date)]
        //public DateOnly ExpDate { get; set; }
        //public string? InventoryId { get; set; }
    }
}
