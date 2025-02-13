using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.Profile
{
    public class ProfileUpdateDTO
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool Sex { get; set; }
        public string Nationality { get; set; }
        public string? LastUpdatedBy { get; set; }

    }
    public class DeleteProfileDTO
    {
        public string? DeletedBy { get; set; }
    }
}
