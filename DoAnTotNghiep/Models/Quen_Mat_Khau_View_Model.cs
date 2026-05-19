using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class Quen_Mat_Khau_View_Model
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string email { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
        public string matKhauMoi { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu mới")]
        [Compare(nameof(matKhauMoi), ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string xacNhanMatKhauMoi { get; set; } = "";
    }
}
