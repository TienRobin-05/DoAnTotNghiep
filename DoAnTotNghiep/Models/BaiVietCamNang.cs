namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp BaiVietCamNang là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class BaiVietCamNang
    {
        // Thuộc tính MaBaiViet lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int MaBaiViet { get; set; }
        // Thuộc tính MaTaiKhoan lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int MaTaiKhoan { get; set; }
        // Thuộc tính TieuDe lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string TieuDe { get; set; } = string.Empty;
        // Thuộc tính NoiDung lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string NoiDung { get; set; } = string.Empty;
        public string LoaiBaiViet { get; set; } = "Cẩm nang";
        public string AnhDaiDien { get; set; } = string.Empty;
        // Thuộc tính NgayTao lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public DateTime NgayTao { get; set; }
        // Thuộc tính TrangThai lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public bool TrangThai { get; set; } = true;

        // Thuộc tính TenTacGia lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public string TenTacGia { get; set; } = string.Empty;
    }
}
