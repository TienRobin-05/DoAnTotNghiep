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
            var danhSachMuiTiemPhuHop = danhSachMuiTiem
                .Where(muiTiem => KiemTraMuiTiemPhuHopVoiTuoi(hoSo.NgaySinh, muiTiem))
                .ToList();
            var ketQua = new KetQuaTaoLichTiem
            {
                SoMuiTiemVaccine = danhSachMuiTiem.Count,
                SoMuiTiemPhuHop = danhSachMuiTiemPhuHop.Count,
                MaMuiTiemPhuHop = danhSachMuiTiemPhuHop.Select(muiTiem => muiTiem.MaMuiTiem).ToList()
            };

            foreach (var muiTiem in danhSachMuiTiemPhuHop)
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
    mt.doTuoiToiDa,
    mt.doTuoiKhuyenNghi,
    mt.donViTuoi
FROM MuiTiemVaccine mt
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE v.trangThai = 1
ORDER BY v.tenVaccine, mt.soMui";

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
                    DoTuoiToiDa = doc["doTuoiToiDa"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiDa"]),
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
            lenh.Parameters.AddWithValue("@GhiChu", "Mũi tiêm được khuyến nghị theo độ tuổi");

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

        // Kiểm tra mũi tiêm có phù hợp với tuổi hiện tại của hồ sơ theo đúng đơn vị ngày/tháng/năm.
        private static bool KiemTraMuiTiemPhuHopVoiTuoi(DateTime ngaySinh, MuiTiemTaoLich muiTiem)
        {
            var donVi = ChuanHoaDonViTuoi(muiTiem.DonViTuoi);
            var tuoiHienTai = TinhTuoiTheoDonVi(ngaySinh, donVi);
            var tuoiToiThieu = muiTiem.DoTuoiToiThieu ?? 0;
            var tuoiToiDa = muiTiem.DoTuoiToiDa ?? 0;

            if (tuoiHienTai < tuoiToiThieu)
            {
                return false;
            }

            // Tuổi tối đa null hoặc 0 được hiểu là không giới hạn.
            return tuoiToiDa <= 0 || tuoiHienTai <= tuoiToiDa;
        }

        // Tính tuổi hiện tại của hồ sơ theo đơn vị tương ứng để lọc mũi tiêm phù hợp.
        private static int TinhTuoiTheoDonVi(DateTime ngaySinh, string donViTuoi)
        {
            var ngayHienTai = DateTime.Today;
            var ngaySinhDate = ngaySinh.Date;
            if (ngaySinhDate > ngayHienTai)
            {
                return 0;
            }

            return donViTuoi switch
            {
                "năm" => TinhSoNamTuoi(ngaySinhDate, ngayHienTai),
                "tháng" => TinhSoThangTuoi(ngaySinhDate, ngayHienTai),
                _ => Math.Max(0, (ngayHienTai - ngaySinhDate).Days)
            };
        }

        private static int TinhSoThangTuoi(DateTime ngaySinh, DateTime ngayHienTai)
        {
            var soThang = ((ngayHienTai.Year - ngaySinh.Year) * 12) + ngayHienTai.Month - ngaySinh.Month;
            if (ngayHienTai.Day < ngaySinh.Day)
            {
                soThang--;
            }

            return Math.Max(0, soThang);
        }

        private static int TinhSoNamTuoi(DateTime ngaySinh, DateTime ngayHienTai)
        {
            var soNam = ngayHienTai.Year - ngaySinh.Year;
            if (ngayHienTai.Date < ngaySinh.Date.AddYears(soNam))
            {
                soNam--;
            }

            return Math.Max(0, soNam);
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
            public int? DoTuoiToiDa { get; set; }
            public int? DoTuoiKhuyenNghi { get; set; }
            public string DonViTuoi { get; set; } = string.Empty;
        }

        /// <summary>
        /// Kết quả trả về sau khi tự động tạo lịch tiêm để Controller hiển thị thông báo phù hợp.
        /// </summary>
        public class KetQuaTaoLichTiem
        {
            public int SoMuiTiemVaccine { get; set; }
            public int SoMuiTiemPhuHop { get; set; }
            public int SoLichTiemDaTao { get; set; }
            public List<int> MaMuiTiemPhuHop { get; set; } = new();
        }
    }
}
