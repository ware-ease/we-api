using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("Profile")]
    public class Profile : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone {  get; set; }
        public string? Address { get; set; }
        public bool Sex { get; set; }
        public string? Nationality { get; set; }
        public string? AvatarUrl { get; set; }

        [ForeignKey("Account")]
        public string AccountId { get; set; }
        public Account Account { get; set; }
    }
}
