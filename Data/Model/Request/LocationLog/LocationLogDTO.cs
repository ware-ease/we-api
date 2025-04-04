using Data.Model.DTO.Base;
using Data.Model.Request.InventoryLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.LocationLog
{
    public class LocationLogDTO : BaseDTO
    {
        public float NewQuantity { get; set; }
        public float ChangeInQuantity { get; set; }
        public string InventoryLocationId { get; set; }
        public string LocationId { get; set; }
        //public InventoryInLocationDTO InventoryLocation { get; set; }
    }
}
