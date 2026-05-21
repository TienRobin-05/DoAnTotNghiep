using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp baiVietCamNangModels là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class baiVietCamNangModels
    {
        // Thuộc tính maBaiViet lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int maBaiViet { get; set; }
        // Thuộc tính maTaiKhoan lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int maTaiKhoan { get; set; }

        [Required(ErrorMessage = "Vui lÃ²ng nháº­p tiÃªu Ä‘á»")]
        // Thuộc tính tieuDe lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string tieuDe { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lÃ²ng nháº­p ná»™i dung")]
        // Thuộc tính noiDung lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public string noiDung { get; set; } = string.Empty;

        // Thuộc tính ngayTao lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public DateTime ngayTao { get; set; }
        // Thuộc tính trangThai lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public bool trangThai { get; set; } = true;
        // Thuộc tính tenTacGia lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? tenTacGia { get; set; }
    }
}
