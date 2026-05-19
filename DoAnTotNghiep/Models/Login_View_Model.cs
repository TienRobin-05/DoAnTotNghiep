using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class Login_View_Model
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string email { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string matKhau { get; set; } = "";
    }
}
