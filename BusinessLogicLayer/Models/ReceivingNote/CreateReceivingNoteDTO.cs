using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.ReceivingNote
{
    public class CreateReceivingNoteDTO
    {
        public DateTime Date { get; set; }
        public string CreatedBy { get; set; }
    }
}
