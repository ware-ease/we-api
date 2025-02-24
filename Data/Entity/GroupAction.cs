using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    [Table("GroupAction")]
    public class GroupAction : BaseEntity
    {
        [ForeignKey("Group")]
        public string GroupId { get; set; }
        public Group Group { get; set; }

        [ForeignKey("Action")]
        public string ActionId { get; set; }
        public AppAction Action { get; set; }
    }
}
