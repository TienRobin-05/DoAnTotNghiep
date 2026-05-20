using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class hoSoSucKhoeModels
    {
        public int maHoSo { get; set; }
        public int maTaiKhoan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string hoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime ngaySinh { get; set; }

        public string? gioiTinh { get; set; }
        public double? chieuCao { get; set; }
        public double? canNang { get; set; }
        public string? tienSuBenh { get; set; }
        public string? diUng { get; set; }
        public DateTime ngayTao { get; set; }
    }
}
