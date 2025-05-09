﻿using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.InventoryAdjustment
{
    public class InventoryAdjustmentCreateDTO : BaseCreateDTO
    {
        [Required(ErrorMessage = "Date không được để trống")]
        public DateTime? Date { get; set; }
        [Required(ErrorMessage = "Reason không được để trống")]
        public string? Reason { get; set; }
        //[Required(ErrorMessage = "Loại yêu cầu không được để trống.")]
        //public GoodRequestEnum RequestType { get; set; }
        [Required(ErrorMessage = "Note không được để trống")]
        public string? Note { get; set; }
        public DocumentType? DocumentType { get; set; }
        public string? RelatedDocument { get; set; }
        [Required(ErrorMessage = "WarehouseId không được để trống")]
        public string WarehouseId { get; set; }
        public string InventoryCountId { get; set; }
        [JsonIgnore]
        public List<InventoryAdjustmentDetailCreateDTO> InventoryAdjustmentDetails { get; set; } = new();

    }

    public class InventoryAdjustmentDetailCreateDTO : BaseCreateDTO
    {
        //[Required(ErrorMessage = "NewQuantity không được để trống")]
        public float NewQuantity { get; set; }
        //[Required(ErrorMessage = "ChangeInQuantity không được để trống")]
        public float ChangeInQuantity { get; set; }
        //[Required(ErrorMessage = "Note không được để trống")]
        public string? Note { get; set; }
        public string? ProductId { get; set; }
        //[Required(ErrorMessage = "InventoryId không được để trống")]
        public string? InventoryId { get; set; }
    }
}
