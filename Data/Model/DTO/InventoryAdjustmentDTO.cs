using Data.Model.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class InventoryAdjustmentDTO : BaseDTO
    {
        public DateTime? Date { get; set; }
        public string? Reason { get; set; }
        public string? Note { get; set; }
        public string? RelatedDocument { get; set; }
        public string WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public List<InventoryAdjustmentDetailDTO> inventoryAdjustmentDetails { get; set; }
    }

    public class InventoryAdjustmentDetailDTO : BaseDTO
    {
        public float NewQuantity { get; set; }
        public float ChangeInQuantity { get; set; }
        public string? Note { get; set; }
        //public string LocationLogId { get; set; }
    }
}
