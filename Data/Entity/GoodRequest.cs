using Data.Entity.Base;
using Data.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entity
{
    [Table("GoodRequest")]
    public class GoodRequest : BaseEntity
    {
        public string? Note { get; set; }
        public string? Code { get; set; }

        public GoodRequestEnum RequestType { get; set; }
        public GoodRequestStatusEnum Status { get; set; } = GoodRequestStatusEnum.Pending;

        public ICollection<GoodNote> GoodNotes { get; set; } = [];
        public ICollection<GoodRequestDetail> GoodRequestDetails { get; set; } = [];

        [ForeignKey("Partner")]
        public string? PartnerId { get; set; }
        public Partner? Partner { get; set; }

        [ForeignKey("Warehouse")]
        public string? WarehouseId { get; set; }
        [InverseProperty("GoodRequests")]
        public Warehouse? Warehouse { get; set; }

        [ForeignKey("RequestedWarehouse")]
        public string? RequestedWarehouseId { get; set; }
        [InverseProperty("RequestedGoodRequests")]
        public Warehouse? RequestedWarehouse { get; set; }
    }
}