using Data.Entity;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.DTO.Base;
using Data.Model.Request.GoodRequest;
using Data.Model.Request.Partner;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Data.Model.Request.GoodNote
{
    public class GoodNoteDTO : BaseDTO
    {
        public GoodNoteEnum NoteType { get; set; }
        public GoodNoteStatusEnum Status { get; set; }
        public string? ShipperName { get; set; }
        public string? ReceiverName { get; set; }
        public string? Code { get; set; }
        public DateTime? Date { get; set; }
        //public string GoodRequestId { get; set; }

        //public string? GoodRequestCode { get; set; }
        public GoodRequestOfGoodNoteDTO? GoodRequest { get; set; }
        //public string? RequestedWarehouseName { get; set; }
        public IEnumerable<GoodNoteDetailDTO>? GoodNoteDetails { get; set; }

    }

    public class GoodNoteDetailDTO: BaseDTO
    {
        public float Quantity { get; set; }
        public string? Note { get; set; }
        //public string GoodNoteId { get; set; }
        //public string BatchId { get; set; }
        public BatchNoteDTO? Batch { get; set; }
    }

    public class BatchNoteDTO : BaseDTO
    {
        public string? SupplierId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public DateOnly MfgDate { get; set; }
        public DateOnly ExpDate { get; set; }
        //public string? InventoryId { get; set; }
        public string ProductId { get; set; }
        public ProductNoteDTO? Product { get; set; }
    }

    public class ProductNoteDTO 
    {
        public string? sku { get; set; }
        public string? Name { get; set; }
        public string UnitName { get; set; }
        public string BrandName { get; set; }
    }
    public class GoodNoteCreateDTO : BaseCreateDTO
    {
        [Required(ErrorMessage = "Loại phiếu không được để trống.")]
        public GoodNoteEnum NoteType { get; set; }
        [MaxLength(100, ErrorMessage = "Tên người giao hàng không được vượt quá 100 ký tự.")]
        public string? ShipperName { get; set; }
        [MaxLength(100, ErrorMessage = "Tên người nhận hàng không được vượt quá 100 ký tự.")]
        public string? ReceiverName { get; set; }
        [Required(ErrorMessage = "Mã phiếu không được để trống.")]
        [MaxLength(50, ErrorMessage = "Mã phiếu không được vượt quá 50 ký tự.")]
        public string? Code { get; set; }
        [Required(ErrorMessage = "Ngày không được để trống.")]
        public DateTime? Date { get; set; }
        [Required(ErrorMessage = "GoodRequestId không được để trống.")]
        public string GoodRequestId { get; set; }
        public IEnumerable<GoodNoteDetailCreateDTO>? GoodNoteDetails { get; set; }
    }

    public class GoodNoteDetailCreateDTO : BaseCreateDTO
    {
        public float Quantity { get; set; }
        public string? Note { get; set; }
        [JsonIgnore]
        public string? GoodNoteId { get; set; }
        public string BatchId { get; set; }
        [JsonIgnore]
        public DateTime? CreatedTime { get; set; }
    }
    public class GoodNoteUpdateDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public GoodNoteEnum? NoteType { get; set; }
        public string? ShipperName { get; set; }
        public string? ReceiverName { get; set; }
        public string? Code { get; set; }
        public DateTime? Date { get; set; }
        public IEnumerable<GoodNoteDetailCreateDTO>? GoodNoteDetails { get; set; }
    }
    public class GoodRequestOfGoodNoteDTO : BaseDTO
    {
        public string? Note { get; set; }
        public string? Code { get; set; }

        public GoodRequestEnum RequestType { get; set; }
        public GoodRequestStatusEnum Status { get; set; } = GoodRequestStatusEnum.Pending;

        //public ICollection<GoodRequestDetail> GoodRequestDetails { get; set; } = [];
        public PartnerDTO? Partner { get; set; }
        public WarehouseDTO? Warehouse { get; set; }
        public WarehouseDTO? RequestedWarehouse { get; set; }
    }
}
