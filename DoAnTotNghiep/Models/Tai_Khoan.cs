using System;

namespace DoAnTotNghiep.Models
{
    public class Tai_Khoan
    {
        public int maTaiKhoan { get; set; }

        public string hoTen { get; set; } = "";

        public string email { get; set; } = "";

        public string matKhau { get; set; } = "";

        public string soDienThoai { get; set; } = "";

        public string vaiTro { get; set; } = "";

        public string trangThai { get; set; } = "";

        public DateTime? ngayTao { get; set; }
    }
}