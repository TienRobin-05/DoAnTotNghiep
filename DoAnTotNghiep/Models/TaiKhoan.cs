namespace DoAnTotNghiep.Models
{
    public class TaiKhoan
    {
        // Thuộc tính MaTaiKhoan lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int MaTaiKhoan { get; set; }
        // Thuộc tính HoTen lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string HoTen { get; set; } = string.Empty;
        // Thuộc tính Email lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string Email { get; set; } = string.Empty;
        // Thuộc tính MatKhau lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string MatKhau { get; set; } = string.Empty;
        // Thuộc tính SoDienThoai lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string SoDienThoai { get; set; } = string.Empty;
        // Thuộc tính VaiTro lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string VaiTro { get; set; } = string.Empty;
        // Thuộc tính TrangThai lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public bool TrangThai { get; set; }
        // Thuộc tính NgayTao lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public DateTime NgayTao { get; set; }
        public DateTime? LanDangNhapCuoi { get; set; }
        public bool DaXoa { get; set; }
        public DateTime? NgayXoaMem { get; set; }
        public string? LyDoXoa { get; set; }
    }

    public class KetQuaDonDepTaiKhoan
    {
        public int SoTaiKhoanXoaMem { get; set; }
        public int SoTaiKhoanXoaCung { get; set; }
        public string ThongBao { get; set; } = string.Empty;
    }
}
