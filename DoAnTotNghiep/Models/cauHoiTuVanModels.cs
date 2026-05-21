using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp cauHoiTuVanModels là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class cauHoiTuVanModels
    {
        // Thuộc tính maCauHoi lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int maCauHoi { get; set; }
        // Thuộc tính maNguoiGui lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int maNguoiGui { get; set; }
        // Thuộc tính maNguoiTraLoi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int? maNguoiTraLoi { get; set; }

        [Required(ErrorMessage = "Vui lÃ²ng nháº­p cÃ¢u há»i")]
        // Thuộc tính cauHoi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string cauHoi { get; set; } = string.Empty;

        // Thuộc tính cauTraLoi lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public string? cauTraLoi { get; set; }
        // Thuộc tính ngayGui lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public DateTime ngayGui { get; set; }
        // Thuộc tính ngayTraLoi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public DateTime? ngayTraLoi { get; set; }
        // Thuộc tính trangThai lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string trangThai { get; set; } = "Chá» tráº£ lá»i";
        // Thuộc tính tenNguoiGui lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? tenNguoiGui { get; set; }
        // Thuộc tính tenNguoiTraLoi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? tenNguoiTraLoi { get; set; }
    }
}
