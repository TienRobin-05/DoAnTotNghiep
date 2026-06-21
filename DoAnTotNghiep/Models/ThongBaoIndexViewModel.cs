namespace DoAnTotNghiep.Models
{
    /// <summary>Dữ liệu trình bày trang thông báo và các số đếm lấy trực tiếp từ database.</summary>
    public class ThongBaoIndexViewModel
    {
        public List<ThongBao> DanhSach { get; set; } = new();
        public string TrangThai { get; set; } = "tat-ca";
        public string TuKhoa { get; set; } = string.Empty;
        public int TrangHienTai { get; set; } = 1;
        public int SoDongMoiTrang { get; set; } = 12;
        public int TongSoTrang { get; set; }
        public int TongKetQua { get; set; }
        public int TongThongBaoTaiKhoan { get; set; }
        public int TongTatCa { get; set; }
        public int TongChuaDoc { get; set; }
        public int TongDaDoc { get; set; }
        public int TongQuaHan { get; set; }
        public int TongDenLich { get; set; }
        public int TongDaCapNhat { get; set; }
    }
}
