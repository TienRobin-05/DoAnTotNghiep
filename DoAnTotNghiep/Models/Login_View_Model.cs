using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class Login_View_Model
    {
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string soDienThoai { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string matKhau { get; set; } = "";
    }
}
