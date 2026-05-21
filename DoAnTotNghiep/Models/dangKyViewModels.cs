using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp dangKyViewModels là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class dangKyViewModels
    {
        [Required(ErrorMessage = "Vui lÃ²ng nháº­p há» tÃªn")]
        // Thuộc tính hoTen lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public string hoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lÃ²ng nháº­p email")]
        [EmailAddress(ErrorMessage = "Email khÃ´ng há»£p lá»‡")]
        // Thuộc tính email lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lÃ²ng nháº­p máº­t kháº©u")]
        // Thuộc tính matKhau lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public string matKhau { get; set; } = string.Empty;

        // Thuộc tính soDienThoai lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? soDienThoai { get; set; }
    }
}
