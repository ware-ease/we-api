using Data.Enum;
using Data.Model.DTO.Base;
using System.Text.Json.Serialization;

namespace Data.Model.Request.GoodRequest
{
    public class GoodRequestDTO : BaseDTO
    {
        public string? Note { get; set; }

        public GoodRequestEnum RequestType { get; set; }

        public string? PartnerId { get; set; }
        public string? WarehouseId { get; set; }
        public string? RequestedWarehouseId { get; set; }
        public IEnumerable<GoodRequestDetailDTO>? GoodRequestDetails { get; set; }

    }

    public class GoodRequestCreateDTO : BaseCreateDTO
    {
        public string? Note { get; set; }

        public GoodRequestEnum RequestType { get; set; }

        public string? PartnerId
        {
            get => PartnerId;
            set => PartnerId = string.IsNullOrEmpty(value) ? null : value;
        }
        public string? WarehouseId
        {
            get => WarehouseId;
            set => WarehouseId = string.IsNullOrEmpty(value) ? null : value;
        }
        public string? RequestedWarehouseId
        {
            get => RequestedWarehouseId;
            set => RequestedWarehouseId = string.IsNullOrEmpty(value) ? null : value;
        }
        public List<GoodRequestDetailDTO>? GoodRequestDetails { get; set; }
    }
    public class GoodRequestUpdateDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public string? Note { get; set; }
        public string? PartnerId
        {
            get => PartnerId;
            set => PartnerId = string.IsNullOrEmpty(value) ? null : value;
        }
        public string? WarehouseId
        {
            get => WarehouseId;
            set => WarehouseId = string.IsNullOrEmpty(value) ? null : value;
        }
        public string? RequestedWarehouseId
        {
            get => RequestedWarehouseId;
            set => RequestedWarehouseId = string.IsNullOrEmpty(value) ? null : value;
        }
        public List<GoodRequestDetailDTO>? GoodRequestDetails { get; set; }

    }

    public class GoodRequestDetailDTO : BaseCreateDTO
    {
        public float Quantity { get; set; }

        public string? ProductId
        {
            get => ProductId;
            set => ProductId = string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
