using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp hoSoSucKhoeModels là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class hoSoSucKhoeModels
    {
        // Thuộc tính maHoSo lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int maHoSo { get; set; }
        // Thuộc tính maTaiKhoan lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int maTaiKhoan { get; set; }

        [Required(ErrorMessage = "Vui lÃ²ng nháº­p há» tÃªn")]
        // Thuộc tính hoTen lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string hoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lÃ²ng chá»n ngÃ y sinh")]
        [DataType(DataType.Date)]
        // Thuộc tính ngaySinh lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public DateTime ngaySinh { get; set; }

        // Thuộc tính gioiTinh lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? gioiTinh { get; set; }
        // Thuộc tính chieuCao lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public double? chieuCao { get; set; }
        // Thuộc tính canNang lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public double? canNang { get; set; }
        // Thuộc tính tienSuBenh lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? tienSuBenh { get; set; }
        // Thuộc tính diUng lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public string? diUng { get; set; }
        // Thuộc tính ngayTao lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public DateTime ngayTao { get; set; }
    }
}
