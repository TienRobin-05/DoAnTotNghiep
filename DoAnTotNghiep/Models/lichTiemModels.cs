using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class lichTiemModels
    {
        public int maLichTiem { get; set; }
        public int maHoSo { get; set; }
        public int maMuiTiem { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày tiêm dự kiến")]
        [DataType(DataType.Date)]
        public DateTime ngayTiemDuKien { get; set; }

        public string trangThai { get; set; } = "Chưa tiêm";
        public string? ghiChu { get; set; }
        public string? tenHoSo { get; set; }
        public string? tenVaccine { get; set; }
        public string? tenMui { get; set; }
        public int? soMui { get; set; }
    }
}
