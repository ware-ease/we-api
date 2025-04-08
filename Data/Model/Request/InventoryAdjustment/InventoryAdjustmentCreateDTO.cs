using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.InventoryAdjustment
{
    public class InventoryAdjustmentCreateDTO : BaseCreateDTO
    {
        [Required(ErrorMessage = "Date không được để trống")]
        public DateTime? Date { get; set; }
        [Required(ErrorMessage = "Reason không được để trống")]
        public string? Reason { get; set; }
        [Required(ErrorMessage = "Note không được để trống")]
        public string? Note { get; set; }
        public string? RelatedDocument { get; set; }
        [Required(ErrorMessage = "WarehouseId không được để trống")]
        public string WarehouseId { get; set; }
        public List<InventoryAdjustmentDetailCreateDTO> InventoryAdjustmentDetails { get; set; }

    }

    public class InventoryAdjustmentDetailCreateDTO : BaseCreateDTO
    {
        [Required(ErrorMessage = "NewQuantity không được để trống")]
        public float NewQuantity { get; set; }
        [Required(ErrorMessage = "ChangeInQuantity không được để trống")]
        public float ChangeInQuantity { get; set; }
        [Required(ErrorMessage = "Note không được để trống")]
        public string? Note { get; set; }
        [Required(ErrorMessage = "LocationId không được để trống")]
        public string? LocationId { get; set; }
        [Required(ErrorMessage = "InventoryId không được để trống")]
        public string? InventoryId { get; set; }
    }
}
