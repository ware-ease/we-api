using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Account
{
    public class AccountCreateDTO
    {
        [Required]
        public string? Username { get; set; }

        [EmailAddress]
        [Required]
        public string? Email { get; set; }

        [JsonIgnore]
        public string? CreatedBy { get; set; }

        public ProfileCreateDTO Profile { get; set; }

        public string GroupId { get; set; }
        public List<string>? WarehouseIds { get; set; }
    }

    public class ProfileCreateDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool Sex { get; set; }
        public string? Nationality { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
