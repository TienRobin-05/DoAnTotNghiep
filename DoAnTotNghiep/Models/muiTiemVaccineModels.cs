using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp muiTiemVaccineModels là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class muiTiemVaccineModels
    {
        // Thuộc tính maMuiTiem lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int maMuiTiem { get; set; }

        [Required(ErrorMessage = "Vui lÃ²ng chá»n vaccine")]
        // Thuộc tính maVaccine lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int maVaccine { get; set; }

        [Required(ErrorMessage = "Vui lÃ²ng nháº­p sá»‘ mÅ©i")]
        // Thuộc tính soMui lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int soMui { get; set; }

        // Thuộc tính tenMui lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? tenMui { get; set; }
        // Thuộc tính doTuoiToiThieu lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int? doTuoiToiThieu { get; set; }
        // Thuộc tính doTuoiToiDa lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int? doTuoiToiDa { get; set; }
        // Thuộc tính doTuoiKhuyenNghi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int? doTuoiKhuyenNghi { get; set; }
        // Thuộc tính donViTuoi lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public string? donViTuoi { get; set; }
        // Thuộc tính khoangCachNgay lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int? khoangCachNgay { get; set; }
        // Thuộc tính ghiChu lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? ghiChu { get; set; }
        // Thuộc tính tenVaccine lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? tenVaccine { get; set; }
    }
}
