using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp dangNhapViewModels là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class dangNhapViewModels
    {
        [Required(ErrorMessage = "Vui lÃ²ng nháº­p email")]
        // Thuộc tính email lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lÃ²ng nháº­p máº­t kháº©u")]
        // Thuộc tính matKhau lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string matKhau { get; set; } = string.Empty;
    }
}
