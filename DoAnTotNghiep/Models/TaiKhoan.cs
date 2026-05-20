namespace DoAnTotNghiep.Models
{
    public class TaiKhoan
    {
        public int MaTaiKhoan { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
