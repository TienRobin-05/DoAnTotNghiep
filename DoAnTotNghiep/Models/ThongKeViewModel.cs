namespace DoAnTotNghiep.Models
{
    /// <summary>
    /// Lớp ThongKeViewModel là model dùng để lưu trữ dữ liệu trao đổi giữa Controller, DAL và View trong hệ thống.
    /// </summary>
    public class ThongKeViewModel
    {
        // Thuộc tính TongSoTaiKhoan lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int TongSoTaiKhoan { get; set; }
        // Thuộc tính TongSoNguoiDung lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int TongSoNguoiDung { get; set; }
        // Thuộc tính TongSoAdmin lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int TongSoAdmin { get; set; }
        // Thuộc tính TongSoHoSoSucKhoe lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int TongSoHoSoSucKhoe { get; set; }
        // Thuộc tính TongSoVaccineDangSuDung lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int TongSoVaccineDangSuDung { get; set; }
        // Thuộc tính TongSoLichTiem lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int TongSoLichTiem { get; set; }
        // Thuộc tính TongSoLichTiemChuaTiem lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int TongSoLichTiemChuaTiem { get; set; }
        // Thuộc tính TongSoLichTiemDaTiem lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int TongSoLichTiemDaTiem { get; set; }
        // Thuộc tính TongSoLichTiemQuaHan lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int TongSoLichTiemQuaHan { get; set; }
        // Thuộc tính TongSoCauHoiChuaTraLoi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int TongSoCauHoiChuaTraLoi { get; set; }
        // Thuộc tính TongSoCauHoiDaTraLoi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int TongSoCauHoiDaTraLoi { get; set; }
        // Thuộc tính TongSoBaiVietDangHienThi lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int TongSoBaiVietDangHienThi { get; set; }
    }
}
