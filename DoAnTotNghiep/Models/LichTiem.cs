namespace DoAnTotNghiep.Models
{
    public class LichTiem
    {
        public int MaLichTiem { get; set; }
        public int MaHoSo { get; set; }
        public int MaMuiTiem { get; set; }
        public DateTime NgayTiemDuKien { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
        public string TenVaccine { get; set; } = string.Empty;
        public string TenMui { get; set; } = string.Empty;
        public int SoMui { get; set; }
        public string HoTenHoSo { get; set; } = string.Empty;
    }
}
