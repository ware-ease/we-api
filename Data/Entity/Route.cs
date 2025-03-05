using Data.Entity.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entity
{
    [Table("Route")]
    public class Route : BaseEntity
    {
        public string? Url { get; set; }
        public string? Code { get; set; }
        public ICollection<Permission> Permissions { get; set; } = [];
    }
}
