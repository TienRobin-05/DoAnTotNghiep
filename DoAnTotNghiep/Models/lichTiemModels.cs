using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp lichTiemModels là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class lichTiemModels
    {
        // Thuộc tính maLichTiem lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int maLichTiem { get; set; }
        // Thuộc tính maHoSo lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int maHoSo { get; set; }
        // Thuộc tính maMuiTiem lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int maMuiTiem { get; set; }

        [Required(ErrorMessage = "Vui lÃ²ng chá»n ngÃ y tiÃªm dá»± kiáº¿n")]
        [DataType(DataType.Date)]
        // Thuộc tính ngayTiemDuKien lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public DateTime ngayTiemDuKien { get; set; }

        // Thuộc tính trangThai lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string trangThai { get; set; } = "ChÆ°a tiÃªm";
        // Thuộc tính ghiChu lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? ghiChu { get; set; }
        // Thuộc tính tenHoSo lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? tenHoSo { get; set; }
        // Thuộc tính tenVaccine lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? tenVaccine { get; set; }
        // Thuộc tính tenMui lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public string? tenMui { get; set; }
        // Thuộc tính soMui lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int? soMui { get; set; }
    }
}
