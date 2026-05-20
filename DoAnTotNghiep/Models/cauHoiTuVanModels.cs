using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class cauHoiTuVanModels
    {
        public int maCauHoi { get; set; }
        public int maNguoiGui { get; set; }
        public int? maNguoiTraLoi { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập câu hỏi")]
        public string cauHoi { get; set; } = string.Empty;

        public string? cauTraLoi { get; set; }
        public DateTime ngayGui { get; set; }
        public DateTime? ngayTraLoi { get; set; }
        public string trangThai { get; set; } = "Chờ trả lời";
        public string? tenNguoiGui { get; set; }
        public string? tenNguoiTraLoi { get; set; }
    }
}
