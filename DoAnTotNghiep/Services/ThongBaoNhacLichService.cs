using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DoAnTotNghiep.Services
{
    /// <summary>
    /// Lớp ThongBaoNhacLichService chứa nghiệp vụ tạo thông báo nhắc lịch tiêm theo đúng bảng ThongBao.
    /// </summary>
    public class ThongBaoNhacLichService
    {
        private readonly string chuoiKetNoi;
        private static bool? daCoCotMaLichTiem;

        public ThongBaoNhacLichService(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        // Kiểm tra các lịch tiêm sắp đến hạn của tài khoản và tạo thông báo nếu chưa có.
        public void KiemTraVaTaoThongBaoNhacLich(int maTaiKhoan)
        {
            // Database thật hiện tại có thể chưa có cột maLichTiem trong bảng ThongBao.
            // Khi thiếu cột này, không tạo thông báo liên kết lịch tiêm để tránh làm trang /NguoiDung bị lỗi SQL.
            if (!ThongBaoCoCotMaLichTiem())
            {
                return;
            }

            var danhSachLichCanNhac = LayLichTiemCanNhac(maTaiKhoan);

            foreach (var lich in danhSachLichCanNhac)
            {
                if (DaCoThongBaoChoLich(maTaiKhoan, lich.MaLichTiem))
                {
                    continue;
                }

                TaoThongBaoNhacLich(maTaiKhoan, lich);
            }
        }

        // Kiểm tra schema thật của bảng ThongBao trước khi dùng cột maLichTiem.
        private bool ThongBaoCoCotMaLichTiem()
        {
            if (daCoCotMaLichTiem.HasValue)
            {
                return daCoCotMaLichTiem.Value;
            }

            const string sql = @"SELECT COUNT(*)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'ThongBao'
AND COLUMN_NAME = 'maLichTiem'";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);

            ketNoi.Open();
            daCoCotMaLichTiem = Convert.ToInt32(lenh.ExecuteScalar()) > 0;
            return daCoCotMaLichTiem.Value;
        }

        // Lấy các lịch tiêm chưa hoàn thành, có ngày dự kiến từ hôm nay đến 7 ngày tới.
        private List<LichTiemCanNhac> LayLichTiemCanNhac(int maTaiKhoan)
        {
            const string sql = @"SELECT
    lt.maLichTiem,
    lt.ngayTiemDuKien,
    hs.hoTen AS hoTenHoSo,
    v.tenVaccine,
    mt.tenMui
FROM LichTiem lt
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE hs.maTaiKhoan = @MaTaiKhoan
AND lt.trangThai <> @TrangThaiDaTiem
AND lt.ngayTiemDuKien >= @TuNgay
AND lt.ngayTiemDuKien < @DenNgay
ORDER BY lt.ngayTiemDuKien, v.tenVaccine, mt.soMui";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@TrangThaiDaTiem", "Đã tiêm");
            lenh.Parameters.AddWithValue("@TuNgay", DateTime.Today);
            lenh.Parameters.AddWithValue("@DenNgay", DateTime.Today.AddDays(8));

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<LichTiemCanNhac>();
            while (doc.Read())
            {
                danhSach.Add(new LichTiemCanNhac
                {
                    MaLichTiem = Convert.ToInt32(doc["maLichTiem"]),
                    NgayTiemDuKien = Convert.ToDateTime(doc["ngayTiemDuKien"]),
                    HoTenHoSo = doc["hoTenHoSo"] == DBNull.Value ? string.Empty : doc["hoTenHoSo"].ToString() ?? string.Empty,
                    TenVaccine = doc["tenVaccine"] == DBNull.Value ? string.Empty : doc["tenVaccine"].ToString() ?? string.Empty,
                    TenMui = doc["tenMui"] == DBNull.Value ? string.Empty : doc["tenMui"].ToString() ?? string.Empty
                });
            }

            return danhSach;
        }

        // Kiểm tra để không tạo trùng thông báo cho cùng một lịch tiêm.
        private bool DaCoThongBaoChoLich(int maTaiKhoan, int maLichTiem)
        {
            const string sql = @"SELECT COUNT(*)
FROM ThongBao
WHERE maTaiKhoan = @MaTaiKhoan
AND maLichTiem = @MaLichTiem";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@MaLichTiem", maLichTiem);

            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // Tạo thông báo nhắc lịch tiêm bằng đúng cột maLichTiem trong bảng ThongBao.
        private void TaoThongBaoNhacLich(int maTaiKhoan, LichTiemCanNhac lich)
        {
            const string sql = @"INSERT INTO ThongBao(maTaiKhoan, maLichTiem, tieuDe, noiDung, ngayGui, daDoc)
VALUES(@MaTaiKhoan, @MaLichTiem, @TieuDe, @NoiDung, @NgayGui, @DaDoc)";

            var tieuDe = "Nhắc lịch tiêm vaccine";
            var noiDung = $"Hồ sơ {lich.HoTenHoSo} có lịch tiêm {lich.TenVaccine} - {lich.TenMui} vào ngày {lich.NgayTiemDuKien:dd/MM/yyyy}.";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@MaLichTiem", lich.MaLichTiem);
            lenh.Parameters.AddWithValue("@TieuDe", tieuDe);
            lenh.Parameters.AddWithValue("@NoiDung", noiDung);
            lenh.Parameters.AddWithValue("@NgayGui", DateTime.Now);
            lenh.Parameters.AddWithValue("@DaDoc", false);

            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        private class LichTiemCanNhac
        {
            public int MaLichTiem { get; set; }
            public DateTime NgayTiemDuKien { get; set; }
            public string HoTenHoSo { get; set; } = string.Empty;
            public string TenVaccine { get; set; } = string.Empty;
            public string TenMui { get; set; } = string.Empty;
        }
    }
}
