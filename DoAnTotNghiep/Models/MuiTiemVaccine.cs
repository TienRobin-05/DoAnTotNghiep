namespace DoAnTotNghiep.Models
{
    public class MuiTiemVaccine
    {
        public int MaMuiTiem { get; set; }
        public int MaVaccine { get; set; }
        public int SoMui { get; set; }
        public string TenMui { get; set; } = string.Empty;
        public int? DoTuoiToiThieu { get; set; }
        public int? DoTuoiToiDa { get; set; }
        public int? DoTuoiKhuyenNghi { get; set; }
        public string DonViTuoi { get; set; } = string.Empty;
        public int? KhoangCachNgay { get; set; }
        public string GhiChu { get; set; } = string.Empty;
        public string TenVaccine { get; set; } = string.Empty;
    }
}
