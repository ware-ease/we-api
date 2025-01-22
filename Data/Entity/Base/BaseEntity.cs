using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity.Base
{
    public abstract class BaseEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public bool IsDeleted { get; set; } = false;
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DefaultValue("GETDATE()")]
        public DateTime? CreatedTime { get; set; } = DateTime.Now;
        public DateTime? LastUpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
    }
}
