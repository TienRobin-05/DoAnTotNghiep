using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class Dang_Ky_View_Model
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string hoTen { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string email { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string matKhau { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [Compare(nameof(matKhau), ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string xacNhanMatKhau { get; set; } = "";

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? soDienThoai { get; set; }
    }
}
