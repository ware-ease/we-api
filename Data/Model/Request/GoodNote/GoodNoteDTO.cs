using Data.Enum;
using Data.Model.DTO.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Data.Model.Request.GoodNote
{
    public class GoodNoteDTO : BaseDTO
    {
        public GoodNoteEnum NoteType { get; set; }
        public string? ShipperName { get; set; }
        public string? ReceiverName { get; set; }
        public string? Code { get; set; }
        public DateTime? Date { get; set; }
        public string GoodRequestId { get; set; }
        public IEnumerable<GoodNoteDetailDTO>? GoodNoteDetails { get; set; }

    }

    public class GoodNoteDetailDTO: BaseDTO
    {
        public float Quantity { get; set; }
        public string? Note { get; set; }
        //public string GoodNoteId { get; set; }
        public string BatchId { get; set; }
        public BatchNoteDTO? Batch { get; set; }
    }

    public class BatchNoteDTO 
    {
        public string? SupplierId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public DateOnly MfgDate { get; set; }
        public DateOnly ExpDate { get; set; }
        public string? InventoryId { get; set; }
        public string ProductId { get; set; }
        public ProductNoteDTO? Product { get; set; }
    }

    public class ProductNoteDTO 
    {
        public string? Name { get; set; }
        public string UnitName { get; set; }
        public string BrandName { get; set; }
    }
    public class GoodNoteCreateDTO : BaseCreateDTO
    {
        public GoodNoteEnum NoteType { get; set; }
        public string? ShipperName { get; set; }
        public string? ReceiverName { get; set; }
        public string? Code { get; set; }
        public DateTime? Date { get; set; }
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
        public GoodNoteEnum NoteType { get; set; }
        public string? ShipperName { get; set; }
        public string? ReceiverName { get; set; }
        public string? Code { get; set; }
        public DateTime? Date { get; set; }
        //public string GoodRequestId { get; set; }
        public IEnumerable<GoodNoteDetailCreateDTO>? GoodNoteDetails { get; set; }
    }
}
