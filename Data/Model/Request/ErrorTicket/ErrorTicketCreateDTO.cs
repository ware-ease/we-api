using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Request.ErrorTicket
{
    public class ErrorTicketCreateDTO : BaseCreateDTO
    {
        [Required(ErrorMessage = "Reason không được để trống")]
        public string? Reason { get; set; }
        [Required(ErrorMessage = "Code không được để trống")]
        public string? Code { get; set; }
        [Required(ErrorMessage = "Note không được để trống")]
        public string? Note { get; set; }
        [Required(ErrorMessage = "HandleBy không được để trống")]
        public string? HandleBy { get; set; }
        [Required(ErrorMessage = "InventoryCountDetailId không được để trống")]
        public string? InventoryCountDetailId { get; set; }
    }
}
