using Data.Entity.Base;
using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("Partner")]
    public class Partner : BaseEntity
    {
        public PartnerEnum PartnerType { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public bool Status { get; set; }
        public ICollection<GoodRequest> GoodRequests { get; set; } = [];
    }
}
