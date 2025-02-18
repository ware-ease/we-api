using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entity.Base;

namespace Data.Entity
{
    [Table("ProductType")]
    public class ProductType : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; }
        public ICollection<ReceivingDetail> ReceivingDetails { get; set; }
        public ICollection<SaleDetail> SaleDetails { get; set; }
        public ICollection<StockCardDetail> StockCardDetails { get; set; }
        public ICollection<IssueDetail> IssueDetails { get; set; }

        [ForeignKey("Product")]
        public string? ProductId { get; set; }
        public Product Product { get; set; }
    }
}
