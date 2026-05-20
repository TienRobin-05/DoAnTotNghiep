using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class muiTiemVaccineModels
    {
        public int maMuiTiem { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vaccine")]
        public int maVaccine { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số mũi")]
        public int soMui { get; set; }

        public string? tenMui { get; set; }
        public int? doTuoiToiThieu { get; set; }
        public int? doTuoiToiDa { get; set; }
        public int? doTuoiKhuyenNghi { get; set; }
        public string? donViTuoi { get; set; }
        public int? khoangCachNgay { get; set; }
        public string? ghiChu { get; set; }
        public string? tenVaccine { get; set; }
    }
}
