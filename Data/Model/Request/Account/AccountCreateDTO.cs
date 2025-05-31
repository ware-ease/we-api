using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Account
{
    public class AccountCreateDTO
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Tên đăng nhập phải từ 4 đến 50 ký tự")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        public string? Email { get; set; }

        [JsonIgnore]
        public string? CreatedBy { get; set; }
        [Required(ErrorMessage = "Thông tin hồ sơ là bắt buộc")]
        public ProfileCreateDTO Profile { get; set; }
        [Required(ErrorMessage = "Nhóm người dùng là bắt buộc")]

        public string GroupId { get; set; }
        public List<string>? WarehouseIds { get; set; }
    }

    public class ProfileCreateDTO
    {
        //[Required(ErrorMessage = "Họ là bắt buộc")]
        [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự")]
        public string? FirstName { get; set; }
        //[Required(ErrorMessage = "Tên là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự")]
        public string? LastName { get; set; }
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        //[StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        public string? Phone { get; set; }
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string? Address { get; set; }
        [Required]
        public bool Sex { get; set; }
        [StringLength(100, ErrorMessage = "Quốc tịch không được vượt quá 100 ký tự")]
        public string? Nationality { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
