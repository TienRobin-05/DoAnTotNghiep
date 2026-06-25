using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class ThongKe_DAL
    {
        private readonly string chuoiKetNoi;

        public ThongKe_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        // Mục đích: phương thức LayThongKeTongQuan thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public ThongKeViewModel LayThongKeTongQuan()
        {
            return new ThongKeViewModel
            {
                TongSoTaiKhoan = Dem("SELECT COUNT(*) FROM TaiKhoan"),
                TongSoNguoiDung = Dem("SELECT COUNT(*) FROM TaiKhoan WHERE vaiTro IN (N'User', N'NguoiDung')"),
                TongSoAdmin = Dem("SELECT COUNT(*) FROM TaiKhoan WHERE vaiTro = N'Admin'"),
                TongSoHoSoSucKhoe = Dem("SELECT COUNT(*) FROM HoSoSucKhoe"),
                TongSoVaccineDangSuDung = Dem("SELECT COUNT(*) FROM Vaccine WHERE trangThai = 1"),
                TongSoLichTiem = Dem("SELECT COUNT(*) FROM LichTiem"),
                TongSoLichTiemChuaTiem = Dem("SELECT COUNT(*) FROM LichTiem WHERE ISNULL(trangThai, N'') <> N'Đã tiêm'"),
                TongSoLichTiemDaTiem = Dem("SELECT COUNT(*) FROM LichTiem WHERE trangThai = N'Đã tiêm'"),
                TongSoLichTiemQuaHan = Dem("SELECT COUNT(*) FROM LichTiem WHERE ISNULL(trangThai, N'') <> N'Đã tiêm' AND CONVERT(date, ngayTiemDuKien) < CONVERT(date, GETDATE())"),
                TongSoCauHoiChuaTraLoi = Dem("SELECT COUNT(*) FROM CauHoiTuVan WHERE trangThai = N'Chưa trả lời'"),
                TongSoCauHoiDaTraLoi = Dem("SELECT COUNT(*) FROM CauHoiTuVan WHERE trangThai = N'Đã trả lời'"),
                TongSoBaiVietDangHienThi = Dem("SELECT COUNT(*) FROM BaiVietCamNang WHERE trangThai = 1")
            };
        }

        // Mục đích: phương thức Dem thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private int Dem(string sql)
        {
            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }
    }
}
