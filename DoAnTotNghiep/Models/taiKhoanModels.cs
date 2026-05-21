using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp taiKhoanModels là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class taiKhoanModels
    {
        // Thuộc tính maTaiKhoan lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int maTaiKhoan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        // Thuộc tính hoTen lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string hoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        // Thuộc tính email lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        // Thuộc tính matKhau lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string matKhau { get; set; } = string.Empty;

        // Thuộc tính soDienThoai lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? soDienThoai { get; set; }
        // Thuộc tính vaiTro lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public string vaiTro { get; set; } = "User";
        // Thuộc tính trangThai lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public bool trangThai { get; set; } = true;
        // Thuộc tính ngayTao lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public DateTime ngayTao { get; set; }
    }
}
