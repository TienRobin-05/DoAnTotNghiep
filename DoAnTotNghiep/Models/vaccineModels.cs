using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp vaccineModels là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class vaccineModels
    {
        // Thuộc tính maVaccine lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int maVaccine { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên vaccine")]
        // Thuộc tính tenVaccine lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string tenVaccine { get; set; } = string.Empty;

        // Thuộc tính nhomVaccine lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? nhomVaccine { get; set; }
        // Thuộc tính doTuoiToiThieu lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int? doTuoiToiThieu { get; set; }
        // Thuộc tính doTuoiToiDa lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int? doTuoiToiDa { get; set; }
        // Thuộc tính donViTuoi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? donViTuoi { get; set; }
        // Thuộc tính moTa lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? moTa { get; set; }
        // Thuộc tính luuY lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string? luuY { get; set; }
        // Thuộc tính trangThai lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public bool trangThai { get; set; } = true;
    }
}
