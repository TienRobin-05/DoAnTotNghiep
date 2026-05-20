namespace DoAnTotNghiep.Models
{
    public class thongBaoModels
    {
        public int maThongBao { get; set; }
        public int maTaiKhoan { get; set; }
        public int? maLichTiem { get; set; }
        public string tieuDe { get; set; } = string.Empty;
        public string? noiDung { get; set; }
        public DateTime ngayGui { get; set; }
        public bool daDoc { get; set; }
    }
}
