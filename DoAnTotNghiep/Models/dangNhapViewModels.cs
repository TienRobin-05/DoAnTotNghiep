using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class dangNhapViewModels
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string matKhau { get; set; } = string.Empty;
    }
}
