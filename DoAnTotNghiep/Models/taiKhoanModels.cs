using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class taiKhoanModels
    {
        public int maTaiKhoan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string hoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string matKhau { get; set; } = string.Empty;

        public string? soDienThoai { get; set; }
        public string vaiTro { get; set; } = "User";
        public bool trangThai { get; set; } = true;
        public DateTime ngayTao { get; set; }
    }
}
