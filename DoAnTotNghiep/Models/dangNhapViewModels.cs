using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class dangNhapViewModels
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        // Thuộc tính email lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        // Thuộc tính matKhau lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string matKhau { get; set; } = string.Empty;
    }
}
