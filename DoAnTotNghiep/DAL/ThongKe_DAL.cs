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

        // lấy thống kê tổng quan
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

        private int Dem(string sql)
        {
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }
    }
}
