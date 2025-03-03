using Data.Entity.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entity
{
    [Table("Permission")]
    public class Permission : BaseEntity
    {
        public string Url { get; set; }
        public string Code { get; set; }
        public ICollection<AppAction> Actions { get; set; }
    }
}
