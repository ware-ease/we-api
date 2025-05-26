using Data.Enum;
using Data.Model.DTO.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Partner
{
    public class PartnerDTO : BaseDTO
    {
        public PartnerEnum PartnerType { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public bool Status { get; set; }
    }

    public class PartnerCreateDTO : BaseCreateDTO
    {
        public PartnerEnum PartnerType { get; set; }
        [Required(ErrorMessage = "Tên đối tác là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên đối tác không được vượt quá 100 ký tự.")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? Phone { get; set; }
        [JsonIgnore]
        public bool Status { get; set; } = true;
    }

    public class PartnerUpdateDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public PartnerEnum? PartnerType { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public bool? Status { get; set; }
    }
}