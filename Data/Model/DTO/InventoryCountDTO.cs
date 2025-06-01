using Data.Enum;
using Data.Model.DTO.Base;

namespace Data.Model.DTO
{
    public class InventoryCountDTO : BaseDTO
    {
        public string Id { get; set; }
        public InventoryCountStatus Status { get; set; }
        public string? Code { get; set; }
        public string? Note { get; set; }
        public DateOnly? Date { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public DateOnly? ScheduleDate { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseId { get; set; }
        public List<InventoryCountDetailDTO> InventoryCountDetails { get; set; }
    }

    public class InventoryCountDetailDTO : BaseDTO
    {
        public string Id { get; set; }
        public InventoryCountDetailStatus Status { get; set; }
        public float ExpectedQuantity { get; set; }
        public float CountedQuantity { get; set; }
        public string? Note { get; set; }
        public string? AccountId { get; set; }
        public string InventoryId { get; set; }
        public string BatchId { get; set; }
        public string BatchCode { get; set; }
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public string UnitName { get; set; }
    }

    public class InventoryCountResponesDTO : BaseDTO
    {
        public string Id { get; set; }
        public InventoryCountStatus Status { get; set; }
        public InventoryCountCheckStatus CheckStatus { get; set; }
        public string? Code { get; set; }
        public string? Note { get; set; }
        public DateOnly? Date { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public DateOnly? ScheduleDate { get; set; }
        public List<InventoryCountDetailResponseDTO> InventoryCountDetails { get; set; }
    }

    public class InventoryCountDetailResponseDTO : BaseDTO
    {
        public string Id { get; set; }
        public InventoryCountDetailStatus Status { get; set; }
        public float ExpectedQuantity { get; set; }
        public float CountedQuantity { get; set; }
        public string? Note { get; set; }
        public string? AccountId { get; set; }
        public string InventoryId { get; set; }
    }

    public class InventoryByLocationDTO : BaseDTO
    {
        public string LocationId { get; set; }
        public string? LocationName { get; set; }
        public string? LocationCode { get; set; }

        public List<CustomInventoryLocationDTO> InventoryLocations { get; set; } = [];
        public List<InventoryWithProductDTO> Inventories { get; set; } = [];
    }

    public class CustomInventoryLocationDTO : BaseDTO
    {
        public string Id { get; set; }
        public string InventoryId { get; set; }
        public string LocationId { get; set; }
        public int Quantity { get; set; }
    }

    public class InventoryWithProductDTO : BaseDTO
    {
        public string Id { get; set; }
        public float CurrentQuantity { get; set; }
        public float? ArrangedQuantity { get; set; }
        public float? NotArrgangedQuantity { get; set; }

        public string ProductId { get; set; }
    }
}
