namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp Vaccine là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class Vaccine
    {
        // Thuộc tính MaVaccine lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int MaVaccine { get; set; }
        // Thuộc tính TenVaccine lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string TenVaccine { get; set; } = string.Empty;
        // Thuộc tính NhomVaccine lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string NhomVaccine { get; set; } = string.Empty;
        // Thuộc tính DoTuoiToiThieu lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int? DoTuoiToiThieu { get; set; }
        // Thuộc tính DoTuoiToiDa lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int? DoTuoiToiDa { get; set; }
        // Thuộc tính DonViTuoi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string DonViTuoi { get; set; } = string.Empty;
        // Thuộc tính MoTa lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public string MoTa { get; set; } = string.Empty;
        // Thuộc tính LuuY lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string LuuY { get; set; } = string.Empty;
        // Thuộc tính TrangThai lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public bool TrangThai { get; set; } = true;
    }
}
