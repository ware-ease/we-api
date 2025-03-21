using Data.Enum;
using Data.Model.DTO.Base;
using System.Text.Json.Serialization;

namespace Data.Model.Request.GoodRequest
{
    public class GoodRequestDTO : BaseDTO
    {
        public string? Note { get; set; }
        public string? Code { get; set; }
        public GoodRequestEnum RequestType { get; set; }

        public string? PartnerId { get; set; }
        public string? PartnerName { get; set; }
        public string? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public string? RequestedWarehouseId { get; set; }
        public string? RequestedWarehouseName { get; set; }
        public IEnumerable<GoodRequestDetailInfoDTO>? GoodRequestDetails { get; set; }

    }

    public class GoodRequestCreateDTO : BaseCreateDTO
    {
        public string? Note { get; set; }
        public string? Code { get; set; }
        public GoodRequestEnum RequestType { get; set; }

        private string? _partnerId;
        public string? PartnerId
        {
            get => _partnerId;
            set => _partnerId = string.IsNullOrEmpty(value) ? null : value;
        }

        private string? _warehouseId;
        public string? WarehouseId
        {
            get => _warehouseId;
            set => _warehouseId = string.IsNullOrEmpty(value) ? null : value;
        }

        private string? _requestedWarehouseId;
        public string? RequestedWarehouseId
        {
            get => _requestedWarehouseId;
            set => _requestedWarehouseId = string.IsNullOrEmpty(value) ? null : value;
        }

        public List<GoodRequestDetailDTO>? GoodRequestDetails { get; set; }
    }

    public class GoodRequestUpdateDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public string? Note { get; set; }
        public string? Code { get; set; }

        private string? _partnerId;
        public string? PartnerId
        {
            get => _partnerId;
            set => _partnerId = string.IsNullOrEmpty(value) ? null : value;
        }

        private string? _warehouseId;
        public string? WarehouseId
        {
            get => _warehouseId;
            set => _warehouseId = string.IsNullOrEmpty(value) ? null : value;
        }

        private string? _requestedWarehouseId;
        public string? RequestedWarehouseId
        {
            get => _requestedWarehouseId;
            set => _requestedWarehouseId = string.IsNullOrEmpty(value) ? null : value;
        }

        public List<GoodRequestDetailDTO>? GoodRequestDetails { get; set; }
    }

    public class GoodRequestDetailDTO : BaseCreateDTO
    {
        public float Quantity { get; set; }

        private string? _productId;
        public string? ProductId
        {
            get => _productId;
            set => _productId = string.IsNullOrEmpty(value) ? null : value;
        }
    }
    public class GoodRequestDetailInfoDTO : BaseCreateDTO
    {
        public float Quantity { get; set; }

        private string? _productId;
        public string? ProductId
        {
            get => _productId;
            set => _productId = string.IsNullOrEmpty(value) ? null : value;
        }
        public string? ProductName { get; set; }
        public string? UnitName { get; set; }
        public string? BrandName { get; set; }
    }
}