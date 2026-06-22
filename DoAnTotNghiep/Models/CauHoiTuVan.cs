namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp CauHoiTuVan là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class CauHoiTuVan
    {
        // Thuộc tính MaCauHoi lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int MaCauHoi { get; set; }
        // Thuộc tính MaNguoiGui lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int MaNguoiGui { get; set; }
        // Thuộc tính MaNguoiTraLoi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int? MaNguoiTraLoi { get; set; }
        public int? MaVaccine { get; set; }
        // Thuộc tính CauHoi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string CauHoi { get; set; } = string.Empty;
        // Thuộc tính CauTraLoi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string CauTraLoi { get; set; } = string.Empty;
        // Thuộc tính NgayGui lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public DateTime NgayGui { get; set; }
        // Thuộc tính NgayTraLoi lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public DateTime? NgayTraLoi { get; set; }
        // Thuộc tính TrangThai lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string TrangThai { get; set; } = string.Empty;

        // Thuộc tính TenNguoiGui lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string TenNguoiGui { get; set; } = string.Empty;
        // Thuộc tính TenNguoiTraLoi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string TenNguoiTraLoi { get; set; } = string.Empty;
        public string TenVaccine { get; set; } = string.Empty;
        public string NhomVaccine { get; set; } = string.Empty;
        public string DoTuoiVaccine { get; set; } = string.Empty;
    }
}
