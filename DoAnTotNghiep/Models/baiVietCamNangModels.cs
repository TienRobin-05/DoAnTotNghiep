using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class baiVietCamNangModels
    {
        public int maBaiViet { get; set; }
        public int maTaiKhoan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        public string tieuDe { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string noiDung { get; set; } = string.Empty;

        public DateTime ngayTao { get; set; }
        public bool trangThai { get; set; } = true;
        public string? tenTacGia { get; set; }
    }
}
