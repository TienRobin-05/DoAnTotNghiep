using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class lichSuTiemModels
    {
        public int maLichSu { get; set; }
        public int maLichTiem { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày tiêm thực tế")]
        [DataType(DataType.Date)]
        public DateTime ngayTiemThucTe { get; set; }

        public string? ghiChu { get; set; }
        public DateTime ngayCapNhat { get; set; }
        public string? tenHoSo { get; set; }
        public string? tenVaccine { get; set; }
        public string? tenMui { get; set; }
    }
}
