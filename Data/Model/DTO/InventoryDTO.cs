using Data.Model.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class InventoryDTO : BaseDTO
    {
        public float CurrentQuantity { get; set; }
        public string WarehouseName { get; set; }
        public string BatchName { get; set; }
        public string BatchCode { get; set; }
    }
}
