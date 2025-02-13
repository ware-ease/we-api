using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.ReceivingNote
{
    public class UpdateReceivingNoteDTO
    {
        public DateTime Date { get; set; }
        public string SupplierId { get; set; }
        public string PurchaseId { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
