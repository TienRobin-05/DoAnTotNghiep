namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp MuiTiemVaccine là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class MuiTiemVaccine
    {
        // Thuộc tính MaMuiTiem lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int MaMuiTiem { get; set; }
        // Thuộc tính MaVaccine lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int MaVaccine { get; set; }
        // Thuộc tính SoMui lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int SoMui { get; set; }
        // Thuộc tính TenMui lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string TenMui { get; set; } = string.Empty;
        // Thuộc tính DoTuoiToiThieu lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int? DoTuoiToiThieu { get; set; }
        // Thuộc tính DoTuoiToiDa lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int? DoTuoiToiDa { get; set; }
        // Thuộc tính DoTuoiKhuyenNghi lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int? DoTuoiKhuyenNghi { get; set; }
        // Thuộc tính DonViTuoi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string DonViTuoi { get; set; } = string.Empty;
        // Thuộc tính KhoangCachNgay lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int? KhoangCachNgay { get; set; }
        // Thuộc tính GhiChu lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string GhiChu { get; set; } = string.Empty;
        // Thuộc tính TenVaccine lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string TenVaccine { get; set; } = string.Empty;
    }
}
