namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp ThongBao là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class ThongBao
    {
        // Thuộc tính MaThongBao lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int MaThongBao { get; set; }
        // Thuộc tính MaTaiKhoan lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int MaTaiKhoan { get; set; }
        // Thuộc tính TieuDe lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string TieuDe { get; set; } = string.Empty;
        // Thuộc tính NoiDung lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string NoiDung { get; set; } = string.Empty;
        // Thuộc tính NgayGui lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public DateTime NgayGui { get; set; }
        // Thuộc tính DaDoc lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public bool DaDoc { get; set; }
        public string TrangThaiDoc => DaDoc ? "ÄÃ£ Ä‘á»c" : "ChÆ°a Ä‘á»c";
    }
}
