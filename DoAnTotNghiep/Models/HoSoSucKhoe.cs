namespace DoAnTotNghiep.Models
{
    public class HoSoSucKhoe
    {
        public int MaHoSo { get; set; }
        public int MaTaiKhoan { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; } = string.Empty;
        public double? ChieuCao { get; set; }
        public double? CanNang { get; set; }
        public string TienSuBenh { get; set; } = string.Empty;
        public string DiUng { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
    }
}
