using Data.Entity;
using Data.Model.DTO.Base;
using Data.Model.Request.GoodNote;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.Inventory
{
    public class InventoryDTO : BaseDTO
    {
        public float CurrentQuantity { get; set; }
        public float? ArrangedQuantity { get; set; }
        public float? NotArrgangedQuantity { get; set; }
        public string BatchId { get; set; }
        public BatchNoteDTO? Batch { get; set; }
    }
}
