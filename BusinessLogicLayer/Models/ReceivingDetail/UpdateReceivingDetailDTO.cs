using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.ReceivingDetail
{
    public class UpdateReceivingDetailDTO
    {
        public int Quanlity { get; set; }
        public float Price { get; set; }
        public string NoteId { get; set; }
        public string ProductTypeId { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
