using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp ThongBao_DAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
    public class ThongBao_DAL
    {
        private readonly string chuoiKetNoi;

        public ThongBao_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        // Mục đích: phương thức LayDanhSachTheoTaiKhoan thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<ThongBao> LayDanhSachTheoTaiKhoan(int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT
    maThongBao,
    maTaiKhoan,
    maLichTiem,
    tieuDe,
    noiDung,
    ngayGui,
    daDoc
FROM ThongBao
WHERE maTaiKhoan = @MaTaiKhoan
ORDER BY ngayGui DESC";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<ThongBao>();
            while (doc.Read())
            {
                danhSach.Add(DocThongBao(doc));
            }

            return danhSach;
        }

        // Mục đích: phương thức DemThongBaoChuaDoc thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public int DemThongBaoChuaDoc(int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT COUNT(*)
FROM ThongBao
WHERE maTaiKhoan = @MaTaiKhoan
AND daDoc = 0";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }

        // Mục đích: phương thức LayThongBaoMoiNhat thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<ThongBao> LayThongBaoMoiNhat(int maTaiKhoan, int soLuong)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT TOP (@SoLuong)
    maThongBao,
    maTaiKhoan,
    maLichTiem,
    tieuDe,
    noiDung,
    ngayGui,
    daDoc
FROM ThongBao
WHERE maTaiKhoan = @MaTaiKhoan
ORDER BY ngayGui DESC";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@SoLuong", soLuong);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<ThongBao>();
            while (doc.Read())
            {
                danhSach.Add(DocThongBao(doc));
            }

            return danhSach;
        }

        // Mục đích: phương thức LayTheoId thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public ThongBao? LayTheoId(int maThongBao, int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT
    maThongBao,
    maTaiKhoan,
    maLichTiem,
    tieuDe,
    noiDung,
    ngayGui,
    daDoc
FROM ThongBao
WHERE maThongBao = @MaThongBao
AND maTaiKhoan = @MaTaiKhoan";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaThongBao", maThongBao);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocThongBao(doc) : null;
        }

        // Mục đích: phương thức Them thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool Them(ThongBao tb)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"INSERT INTO ThongBao(maTaiKhoan, maLichTiem, tieuDe, noiDung, ngayGui, daDoc)
VALUES(@MaTaiKhoan, @MaLichTiem, @TieuDe, @NoiDung, @NgayGui, @DaDoc)";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", tb.MaTaiKhoan);
            lenh.Parameters.AddWithValue("@MaLichTiem", (object?)tb.MaLichTiem ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@TieuDe", tb.TieuDe);
            lenh.Parameters.AddWithValue("@NoiDung", string.IsNullOrWhiteSpace(tb.NoiDung) ? DBNull.Value : tb.NoiDung);
            lenh.Parameters.AddWithValue("@NgayGui", tb.NgayGui);
            lenh.Parameters.AddWithValue("@DaDoc", tb.DaDoc);

            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteNonQuery trả về số dòng bị ảnh hưởng; lớn hơn 0 nghĩa là thêm/sửa/xóa thành công.
            return lenh.ExecuteNonQuery() > 0;
        }

        // Tạo thông báo cho các lịch tiêm đã đến hạn hoặc quá hạn, không tạo trùng khi người dùng mở trang nhiều lần.
        public int TaoThongBaoLichTiemDenHan(int maTaiKhoan)
        {
            var danhSachLich = LayLichTiemDenHan(maTaiKhoan);
            var soLichDenHanTheoHoSo = danhSachLich
                .GroupBy(lich => lich.MaHoSo)
                .ToDictionary(nhom => nhom.Key, nhom => nhom.Count());
            var soThongBaoDaTao = 0;

            foreach (var lich in danhSachLich)
            {
                // Chống tạo trùng bằng đúng cặp maTaiKhoan + maLichTiem theo bảng ThongBao hiện tại.
                if (DaCoThongBaoChoLich(maTaiKhoan, lich.MaLichTiem))
                {
                    continue;
                }

                var tieuDe = TaoTieuDeThongBao(lich);
                var noiDung = TaoNoiDungThongBao(lich, soLichDenHanTheoHoSo.GetValueOrDefault(lich.MaHoSo) > 1);

                if (ThemThongBaoLichTiem(maTaiKhoan, lich.MaLichTiem, tieuDe, noiDung))
                {
                    soThongBaoDaTao++;
                }
            }

            return soThongBaoDaTao;
        }

        // Lấy các lịch chưa tiêm đã đến hạn hoặc quá hạn của tài khoản hiện tại.
        private List<LichTiemCanThongBao> LayLichTiemDenHan(int maTaiKhoan)
        {
            const string sql = @"SELECT
    lt.maLichTiem,
    lt.maHoSo,
    hs.maTaiKhoan,
    hs.hoTen,
    v.tenVaccine,
    v.nhomVaccine,
    mt.soMui,
    mt.tenMui,
    mt.khoangCachNgay,
    mt.doTuoiToiThieu,
    mt.doTuoiToiDa,
    mt.doTuoiKhuyenNghi,
    mt.donViTuoi,
    lt.ngayTiemDuKien
FROM LichTiem lt
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE hs.maTaiKhoan = @MaTaiKhoan
AND CAST(lt.ngayTiemDuKien AS DATE) <= CAST(GETDATE() AS DATE)
AND lt.trangThai = @TrangThaiChuaTiem
ORDER BY lt.ngayTiemDuKien, v.tenVaccine, mt.soMui";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@TrangThaiChuaTiem", "Chưa tiêm");

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<LichTiemCanThongBao>();
            while (doc.Read())
            {
                danhSach.Add(new LichTiemCanThongBao
                {
                    MaLichTiem = Convert.ToInt32(doc["maLichTiem"]),
                    MaHoSo = Convert.ToInt32(doc["maHoSo"]),
                    HoTenHoSo = doc["hoTen"] == DBNull.Value ? string.Empty : doc["hoTen"].ToString() ?? string.Empty,
                    TenVaccine = doc["tenVaccine"] == DBNull.Value ? string.Empty : doc["tenVaccine"].ToString() ?? string.Empty,
                    NhomVaccine = doc["nhomVaccine"] == DBNull.Value ? string.Empty : doc["nhomVaccine"].ToString() ?? string.Empty,
                    SoMui = doc["soMui"] == DBNull.Value ? 0 : Convert.ToInt32(doc["soMui"]),
                    TenMui = doc["tenMui"] == DBNull.Value ? string.Empty : doc["tenMui"].ToString() ?? string.Empty,
                    KhoangCachNgay = doc["khoangCachNgay"] == DBNull.Value ? null : Convert.ToInt32(doc["khoangCachNgay"]),
                    DoTuoiToiThieu = doc["doTuoiToiThieu"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiThieu"]),
                    DoTuoiToiDa = doc["doTuoiToiDa"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiDa"]),
                    DoTuoiKhuyenNghi = doc["doTuoiKhuyenNghi"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiKhuyenNghi"]),
                    DonViTuoi = doc["donViTuoi"] == DBNull.Value ? string.Empty : doc["donViTuoi"].ToString() ?? string.Empty,
                    NgayTiemDuKien = Convert.ToDateTime(doc["ngayTiemDuKien"])
                });
            }

            return danhSach;
        }

        private bool DaCoThongBaoChoLich(int maTaiKhoan, int maLichTiem)
        {
            const string sql = @"SELECT COUNT(*)
FROM ThongBao
WHERE maTaiKhoan = @maTaiKhoan
AND maLichTiem = @maLichTiem";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@maLichTiem", maLichTiem);

            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        private bool ThemThongBaoLichTiem(int maTaiKhoan, int maLichTiem, string tieuDe, string noiDung)
        {
            const string sql = @"INSERT INTO ThongBao(maTaiKhoan, maLichTiem, tieuDe, noiDung, ngayGui, daDoc)
VALUES(@maTaiKhoan, @maLichTiem, @tieuDe, @noiDung, GETDATE(), 0)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@maLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@tieuDe", tieuDe);
            lenh.Parameters.AddWithValue("@noiDung", noiDung);

            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        private static string TaoTieuDeThongBao(LichTiemCanThongBao lich)
        {
            return lich.NgayTiemDuKien.Date == DateTime.Today
                ? "Đã đến lịch tiêm"
                : "Quá hạn lịch tiêm";
        }

        private static string TaoNoiDungThongBao(LichTiemCanThongBao lich, bool coMuiKhacDenHan)
        {
            var ngayTiem = lich.NgayTiemDuKien.ToString("dd-MM-yyyy");
            var thongTinMui = $"mũi {lich.SoMui}";
            if (!string.IsNullOrWhiteSpace(lich.TenMui))
            {
                thongTinMui += $" - {lich.TenMui}";
            }

            var thongTinKhoangCach = lich.KhoangCachNgay.HasValue && lich.KhoangCachNgay.Value > 0
                ? $" Khoảng cách khuyến nghị giữa các mũi: {lich.KhoangCachNgay.Value} ngày."
                : " Khoảng cách giữa các mũi: chưa thiết lập.";

            var noiDung = lich.NgayTiemDuKien.Date == DateTime.Today
                ? $"Hồ sơ {lich.HoTenHoSo} đã đủ điều kiện tiêm vaccine {lich.TenVaccine}, nhóm {lich.NhomVaccine}. Mũi cần tiêm: {thongTinMui}. Có thể đi tiêm từ ngày {ngayTiem}.{thongTinKhoangCach} Vui lòng kiểm tra và cập nhật trạng thái sau khi tiêm."
                : $"Hồ sơ {lich.HoTenHoSo} đã quá hạn tiêm vaccine {lich.TenVaccine}, nhóm {lich.NhomVaccine}. Mũi cần tiêm: {thongTinMui}. Lịch tiêm dự kiến từ ngày {ngayTiem}.{thongTinKhoangCach} Vui lòng đi tiêm sớm và cập nhật trạng thái sau khi tiêm.";

            if (coMuiKhacDenHan)
            {
                noiDung += " Ngoài ra, hồ sơ này còn có các mũi tiêm khác đang đến hạn. Vui lòng kiểm tra trong mục Lịch tiêm.";
            }

            return noiDung;
        }

        // Mục đích: phương thức DanhDauDaDoc thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool DanhDauDaDoc(int maThongBao, int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"UPDATE ThongBao
SET daDoc = 1
WHERE maThongBao = @MaThongBao
AND maTaiKhoan = @MaTaiKhoan";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaThongBao", maThongBao);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteNonQuery trả về số dòng bị ảnh hưởng; lớn hơn 0 nghĩa là thêm/sửa/xóa thành công.
            return lenh.ExecuteNonQuery() > 0;
        }

        // Mục đích: phương thức DocThongBao thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static ThongBao DocThongBao(SqlDataReader doc)
        {
            return new ThongBao
            {
                MaThongBao = Convert.ToInt32(doc["maThongBao"]),
                MaTaiKhoan = Convert.ToInt32(doc["maTaiKhoan"]),
                MaLichTiem = CoCot(doc, "maLichTiem") && doc["maLichTiem"] != DBNull.Value ? Convert.ToInt32(doc["maLichTiem"]) : null,
                TieuDe = doc["tieuDe"] == DBNull.Value ? string.Empty : doc["tieuDe"].ToString() ?? string.Empty,
                NoiDung = doc["noiDung"] == DBNull.Value ? string.Empty : doc["noiDung"].ToString() ?? string.Empty,
                NgayGui = Convert.ToDateTime(doc["ngayGui"]),
                DaDoc = Convert.ToBoolean(doc["daDoc"])
            };
        }

        // Kiểm tra an toàn một cột có tồn tại trong SqlDataReader hay không trước khi đọc dữ liệu.
        private static bool CoCot(SqlDataReader doc, string tenCot)
        {
            for (var i = 0; i < doc.FieldCount; i++)
            {
                if (string.Equals(doc.GetName(i), tenCot, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private class LichTiemCanThongBao
        {
            public int MaLichTiem { get; set; }
            public int MaHoSo { get; set; }
            public string HoTenHoSo { get; set; } = string.Empty;
            public string TenVaccine { get; set; } = string.Empty;
            public string NhomVaccine { get; set; } = string.Empty;
            public int SoMui { get; set; }
            public string TenMui { get; set; } = string.Empty;
            public int? KhoangCachNgay { get; set; }
            public int? DoTuoiToiThieu { get; set; }
            public int? DoTuoiToiDa { get; set; }
            public int? DoTuoiKhuyenNghi { get; set; }
            public string DonViTuoi { get; set; } = string.Empty;
            public DateTime NgayTiemDuKien { get; set; }
        }
    }
}
