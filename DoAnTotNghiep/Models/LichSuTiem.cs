namespace DoAnTotNghiep.Models
{
    public class LichSuTiem
    {
        // Thuộc tính MaLichSu lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int MaLichSu { get; set; }
        // Thuộc tính MaLichTiem lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int MaLichTiem { get; set; }
        // Thuộc tính NgayTiemThucTe lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public DateTime NgayTiemThucTe { get; set; }
        // Thuộc tính GhiChu lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string GhiChu { get; set; } = string.Empty;
        // Thuộc tính NgayCapNhat lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public DateTime NgayCapNhat { get; set; }

        // Thuộc tính MaHoSo lưu một phần thông tin của đối tượng model. Dữ liệu này được dùng khi đọc/ghi database, truyền sang Controller hoặc hiển thị ra View.
        public int MaHoSo { get; set; }
        // Thuộc tính HoTenHoSo lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string HoTenHoSo { get; set; } = string.Empty;
        // Thuộc tính TenVaccine lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string TenVaccine { get; set; } = string.Empty;
        // Thuộc tính TenMui lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public string TenMui { get; set; } = string.Empty;
        // Thuộc tính SoMui lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public int SoMui { get; set; }
        // Thuộc tính NgayTiemDuKien lưu thông tin tương ứng của đối tượng model. Controller dùng dữ liệu này để truyền sang View, còn DAL dùng để đọc hoặc ghi đúng cột dữ liệu trong database.
        public DateTime NgayTiemDuKien { get; set; }

        public string hoTenHoSo
        {
            get => HoTenHoSo;
            set => HoTenHoSo = value;
        }

        public string tenVaccine
        {
            get => TenVaccine;
            set => TenVaccine = value;
        }

        public string tenMui
        {
            get => TenMui;
            set => TenMui = value;
        }

        public int soMui
        {
            get => SoMui;
            set => SoMui = value;
        }

        public DateTime ngayTiemDuKien
        {
            get => NgayTiemDuKien;
            set => NgayTiemDuKien = value;
        }
    }
}
