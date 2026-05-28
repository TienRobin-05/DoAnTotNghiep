using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DoAnTotNghiep.Services
{
    /// <summary>
    /// Lớp TaoLichTiemService chứa nghiệp vụ dùng chung, giúp Controller/DAL tách riêng phần xử lý phức tạp khỏi luồng request chính.
    /// </summary>
    public class TaoLichTiemService
    {
        private readonly string chuoiKetNoi;

        public TaoLichTiemService(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        // Mục đích: phương thức TaoLichTiemChoHoSo xử lý nghiệp vụ trung gian để Controller có thể tái sử dụng mà không lặp code.
        // Dữ liệu đầu vào: các model, mã định danh hoặc dữ liệu nghiệp vụ cần thiết cho quá trình xử lý.
        // Xử lý chính: phối hợp các bước tính toán, kiểm tra điều kiện và gọi DAL khi cần truy xuất dữ liệu.
        // Kết quả trả về: kết quả nghiệp vụ sau khi xử lý, có thể là dữ liệu, trạng thái thành công hoặc không trả về giá trị nếu chỉ thực hiện tác vụ.
        public KetQuaTaoLichTiem TaoLichTiemChoHoSo(int maHoSo)
        {
            var hoSo = LayHoSoTheoMa(maHoSo);
            if (hoSo == null)
            {
                return new KetQuaTaoLichTiem();
            }

            var danhSachMuiTiem = LayDanhSachMuiTiemVaccine();
            var ketQua = new KetQuaTaoLichTiem
            {
                SoMuiTiemVaccine = danhSachMuiTiem.Count
            };

            foreach (var muiTiem in danhSachMuiTiem)
            {
                // Không tạo trùng lịch tiêm cho cùng một hồ sơ và cùng một mũi tiêm.
                if (KiemTraLichTiemDaTonTai(maHoSo, muiTiem.MaMuiTiem))
                {
                    continue;
                }

                // Ưu tiên độ tuổi khuyến nghị, nếu chưa có thì dùng độ tuổi tối thiểu.
                var doTuoiTinhLich = muiTiem.DoTuoiKhuyenNghi ?? muiTiem.DoTuoiToiThieu;
                var ngayTiemDuKien = doTuoiTinhLich.HasValue
                    ? TinhNgayTiemDuKien(hoSo.NgaySinh, doTuoiTinhLich.Value, muiTiem.DonViTuoi)
                    : DateTime.Today;

                if (ThemLichTiemNeuChuaTonTai(maHoSo, muiTiem.MaMuiTiem, ngayTiemDuKien))
                {
                    ketQua.SoLichTiemDaTao++;
                }
            }

            return ketQua;
        }

        // Mục đích: phương thức LayHoSoTheoMa xử lý nghiệp vụ trung gian để Controller có thể tái sử dụng mà không lặp code.
        // Dữ liệu đầu vào: các model, mã định danh hoặc dữ liệu nghiệp vụ cần thiết cho quá trình xử lý.
        // Xử lý chính: phối hợp các bước tính toán, kiểm tra điều kiện và gọi DAL khi cần truy xuất dữ liệu.
        // Kết quả trả về: kết quả nghiệp vụ sau khi xử lý, có thể là dữ liệu, trạng thái thành công hoặc không trả về giá trị nếu chỉ thực hiện tác vụ.
        private HoSoTaoLich? LayHoSoTheoMa(int maHoSo)
        {
            const string sql = @"SELECT maHoSo, ngaySinh
FROM HoSoSucKhoe
WHERE maHoSo = @MaHoSo";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            if (!doc.Read())
            {
                return null;
            }

            return new HoSoTaoLich
            {
                MaHoSo = Convert.ToInt32(doc["maHoSo"]),
                NgaySinh = Convert.ToDateTime(doc["ngaySinh"])
            };
        }

        // Mục đích: phương thức LayDanhSachMuiTiemVaccine lấy toàn bộ mũi tiêm làm nguồn tự động tạo lịch tiêm.
        // Dữ liệu đầu vào: các model, mã định danh hoặc dữ liệu nghiệp vụ cần thiết cho quá trình xử lý.
        // Xử lý chính: phối hợp các bước tính toán, kiểm tra điều kiện và gọi DAL khi cần truy xuất dữ liệu.
        // Kết quả trả về: kết quả nghiệp vụ sau khi xử lý, có thể là dữ liệu, trạng thái thành công hoặc không trả về giá trị nếu chỉ thực hiện tác vụ.
        private List<MuiTiemTaoLich> LayDanhSachMuiTiemVaccine()
        {
            const string sql = @"SELECT
    mt.maMuiTiem,
    mt.doTuoiToiThieu,
    mt.doTuoiKhuyenNghi,
    mt.donViTuoi
FROM MuiTiemVaccine mt
ORDER BY mt.maVaccine, mt.soMui";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<MuiTiemTaoLich>();
            while (doc.Read())
            {
                danhSach.Add(new MuiTiemTaoLich
                {
                    MaMuiTiem = Convert.ToInt32(doc["maMuiTiem"]),
                    DoTuoiToiThieu = doc["doTuoiToiThieu"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiThieu"]),
                    DoTuoiKhuyenNghi = doc["doTuoiKhuyenNghi"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiKhuyenNghi"]),
                    DonViTuoi = doc["donViTuoi"] == DBNull.Value ? string.Empty : doc["donViTuoi"].ToString() ?? string.Empty
                });
            }

            return danhSach;
        }

        // Mục đích: phương thức KiemTraLichTiemDaTonTai xử lý nghiệp vụ trung gian để Controller có thể tái sử dụng mà không lặp code.
        // Dữ liệu đầu vào: các model, mã định danh hoặc dữ liệu nghiệp vụ cần thiết cho quá trình xử lý.
        // Xử lý chính: phối hợp các bước tính toán, kiểm tra điều kiện và gọi DAL khi cần truy xuất dữ liệu.
        // Kết quả trả về: kết quả nghiệp vụ sau khi xử lý, có thể là dữ liệu, trạng thái thành công hoặc không trả về giá trị nếu chỉ thực hiện tác vụ.
        private bool KiemTraLichTiemDaTonTai(int maHoSo, int maMuiTiem)
        {
            const string sql = @"SELECT COUNT(*)
FROM LichTiem
WHERE maHoSo = @MaHoSo
AND maMuiTiem = @MaMuiTiem";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);

            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // Mục đích: phương thức ThemLichTiemNeuChuaTonTai chỉ insert khi chưa có cặp maHoSo + maMuiTiem trong LichTiem.
        // Dữ liệu đầu vào: các model, mã định danh hoặc dữ liệu nghiệp vụ cần thiết cho quá trình xử lý.
        // Xử lý chính: phối hợp các bước tính toán, kiểm tra điều kiện và gọi DAL khi cần truy xuất dữ liệu.
        // Kết quả trả về: kết quả nghiệp vụ sau khi xử lý, có thể là dữ liệu, trạng thái thành công hoặc không trả về giá trị nếu chỉ thực hiện tác vụ.
        private bool ThemLichTiemNeuChuaTonTai(int maHoSo, int maMuiTiem, DateTime ngayTiemDuKien)
        {
            const string sql = @"INSERT INTO LichTiem(maHoSo, maMuiTiem, ngayTiemDuKien, trangThai, ghiChu)
SELECT @MaHoSo, @MaMuiTiem, @NgayTiemDuKien, @TrangThai, @GhiChu
WHERE NOT EXISTS (
    SELECT 1
    FROM LichTiem WITH (UPDLOCK, HOLDLOCK)
    WHERE maHoSo = @MaHoSo
    AND maMuiTiem = @MaMuiTiem
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);
            lenh.Parameters.AddWithValue("@NgayTiemDuKien", ngayTiemDuKien.Date);
            lenh.Parameters.AddWithValue("@TrangThai", "Chưa tiêm");
            lenh.Parameters.AddWithValue("@GhiChu", "Tự động tạo lịch tiêm");

            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        // Mục đích: phương thức TinhNgayTiemDuKien xử lý nghiệp vụ trung gian để Controller có thể tái sử dụng mà không lặp code.
        // Dữ liệu đầu vào: các model, mã định danh hoặc dữ liệu nghiệp vụ cần thiết cho quá trình xử lý.
        // Xử lý chính: phối hợp các bước tính toán, kiểm tra điều kiện và gọi DAL khi cần truy xuất dữ liệu.
        // Kết quả trả về: kết quả nghiệp vụ sau khi xử lý, có thể là dữ liệu, trạng thái thành công hoặc không trả về giá trị nếu chỉ thực hiện tác vụ.
        private static DateTime TinhNgayTiemDuKien(DateTime ngaySinh, int doTuoi, string donViTuoi)
        {
            var donVi = ChuanHoaDonViTuoi(donViTuoi);

            try
            {
                return donVi switch
                {
                    "năm" => ngaySinh.AddYears(doTuoi),
                    "tháng" => ngaySinh.AddMonths(doTuoi),
                    "ngày" => ngaySinh.AddDays(doTuoi),
                    _ => DateTime.Today
                };
            }
            catch (ArgumentOutOfRangeException)
            {
                // Nếu dữ liệu tuổi trong database không hợp lệ, dùng ngày hiện tại để tránh lỗi khi xem lịch.
                return DateTime.Today;
            }
        }

        // Mục đích: phương thức ChuanHoaDonViTuoi xử lý nghiệp vụ trung gian để Controller có thể tái sử dụng mà không lặp code.
        // Dữ liệu đầu vào: các model, mã định danh hoặc dữ liệu nghiệp vụ cần thiết cho quá trình xử lý.
        // Xử lý chính: phối hợp các bước tính toán, kiểm tra điều kiện và gọi DAL khi cần truy xuất dữ liệu.
        // Kết quả trả về: kết quả nghiệp vụ sau khi xử lý, có thể là dữ liệu, trạng thái thành công hoặc không trả về giá trị nếu chỉ thực hiện tác vụ.
        private static string ChuanHoaDonViTuoi(string donViTuoi)
        {
            var donVi = donViTuoi.Trim().ToLower();

            if (donVi == "nam")
            {
                return "năm";
            }

            if (donVi == "thang")
            {
                return "tháng";
            }

            return string.IsNullOrWhiteSpace(donVi) ? "ngày" : donVi;
        }

        /// <summary>
        /// Lớp HoSoTaoLich chứa nghiệp vụ dùng chung, giúp Controller/DAL tách riêng phần xử lý phức tạp khỏi luồng request chính.
        /// </summary>
        private class HoSoTaoLich
        {
            public int MaHoSo { get; set; }
            public DateTime NgaySinh { get; set; }
        }

        /// <summary>
        /// Lớp MuiTiemTaoLich chứa nghiệp vụ dùng chung, giúp Controller/DAL tách riêng phần xử lý phức tạp khỏi luồng request chính.
        /// </summary>
        private class MuiTiemTaoLich
        {
            public int MaMuiTiem { get; set; }
            public int? DoTuoiToiThieu { get; set; }
            public int? DoTuoiKhuyenNghi { get; set; }
            public string DonViTuoi { get; set; } = string.Empty;
        }

        /// <summary>
        /// Kết quả trả về sau khi tự động tạo lịch tiêm để Controller hiển thị thông báo phù hợp.
        /// </summary>
        public class KetQuaTaoLichTiem
        {
            public int SoMuiTiemVaccine { get; set; }
            public int SoLichTiemDaTao { get; set; }
        }
    }
}
