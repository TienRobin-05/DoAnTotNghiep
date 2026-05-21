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
        public void TaoLichTiemChoHoSo(int maHoSo)
        {
            var hoSo = LayHoSoTheoMa(maHoSo);
            if (hoSo == null)
            {
                return;
            }

            var danhSachMuiTiem = LayDanhSachMuiTiemCuaVaccineDangSuDung();

            foreach (var muiTiem in danhSachMuiTiem)
            {
                // KhÃ´ng táº¡o trÃ¹ng lá»‹ch tiÃªm cho cÃ¹ng má»™t há»“ sÆ¡ vÃ  cÃ¹ng má»™t mÅ©i tiÃªm.
                if (KiemTraLichTiemDaTonTai(maHoSo, muiTiem.MaMuiTiem))
                {
                    continue;
                }

                // Æ¯u tiÃªn Ä‘á»™ tuá»•i khuyáº¿n nghá»‹, náº¿u chÆ°a cÃ³ thÃ¬ dÃ¹ng Ä‘á»™ tuá»•i tá»‘i thiá»ƒu.
                var doTuoiTinhLich = muiTiem.DoTuoiKhuyenNghi ?? muiTiem.DoTuoiToiThieu ?? 0;
                var ngayTiemDuKien = TinhNgayTiemDuKien(hoSo.NgaySinh, doTuoiTinhLich, muiTiem.DonViTuoi);

                ThemLichTiem(maHoSo, muiTiem.MaMuiTiem, ngayTiemDuKien);
            }
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

        // Mục đích: phương thức LayDanhSachMuiTiemCuaVaccineDangSuDung xử lý nghiệp vụ trung gian để Controller có thể tái sử dụng mà không lặp code.
        // Dữ liệu đầu vào: các model, mã định danh hoặc dữ liệu nghiệp vụ cần thiết cho quá trình xử lý.
        // Xử lý chính: phối hợp các bước tính toán, kiểm tra điều kiện và gọi DAL khi cần truy xuất dữ liệu.
        // Kết quả trả về: kết quả nghiệp vụ sau khi xử lý, có thể là dữ liệu, trạng thái thành công hoặc không trả về giá trị nếu chỉ thực hiện tác vụ.
        private List<MuiTiemTaoLich> LayDanhSachMuiTiemCuaVaccineDangSuDung()
        {
            const string sql = @"SELECT
    mt.maMuiTiem,
    mt.doTuoiToiThieu,
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

        // Mục đích: phương thức ThemLichTiem xử lý nghiệp vụ trung gian để Controller có thể tái sử dụng mà không lặp code.
        // Dữ liệu đầu vào: các model, mã định danh hoặc dữ liệu nghiệp vụ cần thiết cho quá trình xử lý.
        // Xử lý chính: phối hợp các bước tính toán, kiểm tra điều kiện và gọi DAL khi cần truy xuất dữ liệu.
        // Kết quả trả về: kết quả nghiệp vụ sau khi xử lý, có thể là dữ liệu, trạng thái thành công hoặc không trả về giá trị nếu chỉ thực hiện tác vụ.
        private void ThemLichTiem(int maHoSo, int maMuiTiem, DateTime ngayTiemDuKien)
        {
            const string sql = @"INSERT INTO LichTiem(maHoSo, maMuiTiem, ngayTiemDuKien, trangThai, ghiChu)
VALUES(@MaHoSo, @MaMuiTiem, @NgayTiemDuKien, @TrangThai, @GhiChu)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);
            lenh.Parameters.AddWithValue("@NgayTiemDuKien", ngayTiemDuKien.Date);
            lenh.Parameters.AddWithValue("@TrangThai", "ChÆ°a tiÃªm");
            lenh.Parameters.AddWithValue("@GhiChu", string.Empty);

            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // Mục đích: phương thức TinhNgayTiemDuKien xử lý nghiệp vụ trung gian để Controller có thể tái sử dụng mà không lặp code.
        // Dữ liệu đầu vào: các model, mã định danh hoặc dữ liệu nghiệp vụ cần thiết cho quá trình xử lý.
        // Xử lý chính: phối hợp các bước tính toán, kiểm tra điều kiện và gọi DAL khi cần truy xuất dữ liệu.
        // Kết quả trả về: kết quả nghiệp vụ sau khi xử lý, có thể là dữ liệu, trạng thái thành công hoặc không trả về giá trị nếu chỉ thực hiện tác vụ.
        private static DateTime TinhNgayTiemDuKien(DateTime ngaySinh, int doTuoi, string donViTuoi)
        {
            var donVi = ChuanHoaDonViTuoi(donViTuoi);

            return donVi switch
            {
                "nÄƒm" => ngaySinh.AddYears(doTuoi),
                "thÃ¡ng" => ngaySinh.AddMonths(doTuoi),
                _ => ngaySinh.AddDays(doTuoi)
            };
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
                return "nÄƒm";
            }

            if (donVi == "thang")
            {
                return "thÃ¡ng";
            }

            return string.IsNullOrWhiteSpace(donVi) ? "ngÃ y" : donVi;
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
    }
}
