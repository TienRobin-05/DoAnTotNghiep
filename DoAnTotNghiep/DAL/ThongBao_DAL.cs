using DoAnTotNghiep.Models;
using DoAnTotNghiep.Services;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp ThongBao_DAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
    public class ThongBao_DAL
    {
        private readonly string chuoiKetNoi;
        private static readonly string[] TieuDeThongBaoNhacLich =
        {
            "Sắp đến lịch tiêm",
            "Hôm nay là lịch tiêm",
            "Đã đến lịch tiêm",
            "Quá hạn lịch tiêm"
        };
        private const int GioGuiThongBaoNhacLich = 0;
        private const int SoNgayNhacTruoc = 2;
        private const int SoNgayGiuThongBaoNhacLich = 3;
        private readonly PushNotificationService pushNotificationService;

        public ThongBao_DAL(IConfiguration configuration, PushNotificationService pushNotificationService)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            this.pushNotificationService = pushNotificationService;
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

        // Lấy đúng một trang thông báo sau khi lọc/tìm kiếm; không tải toàn bộ dữ liệu về View.
        public List<ThongBao> LayTrangTheoTaiKhoan(int maTaiKhoan, bool? daDoc, string? tuKhoa, int trang, int soDongMoiTrang)
        {
            var dieuKienDaDoc = daDoc.HasValue ? "AND tb.daDoc = @DaDoc" : string.Empty;
            var sql = $@"SELECT
    tb.maThongBao, tb.maTaiKhoan, tb.maLichTiem, tb.tieuDe, tb.noiDung, tb.ngayGui, tb.daDoc,
    lt.maHoSo, hs.hoTen AS hoTenHoSo
FROM ThongBao tb
LEFT JOIN LichTiem lt ON tb.maLichTiem = lt.maLichTiem
LEFT JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo AND hs.maTaiKhoan = tb.maTaiKhoan
WHERE tb.maTaiKhoan = @MaTaiKhoan
{dieuKienDaDoc}
AND (
    @TuKhoa = N''
    OR tb.tieuDe COLLATE Latin1_General_100_CI_AI LIKE N'%' + @TuKhoa + N'%'
    OR tb.noiDung COLLATE Latin1_General_100_CI_AI LIKE N'%' + @TuKhoa + N'%'
    OR ISNULL(hs.hoTen, N'') COLLATE Latin1_General_100_CI_AI LIKE N'%' + @TuKhoa + N'%'
)
ORDER BY tb.ngayGui DESC, tb.maThongBao DESC
OFFSET @SoDongBoQua ROWS FETCH NEXT @SoDongMoiTrang ROWS ONLY";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSoBoLoc(lenh, maTaiKhoan, daDoc, tuKhoa);
            lenh.Parameters.AddWithValue("@SoDongBoQua", (Math.Max(1, trang) - 1) * soDongMoiTrang);
            lenh.Parameters.AddWithValue("@SoDongMoiTrang", soDongMoiTrang);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<ThongBao>();
            while (doc.Read()) danhSach.Add(DocThongBao(doc));
            return danhSach;
        }

        // Đếm ba tab theo từ khóa hiện tại nhưng không phụ thuộc tab đang chọn.
        public (int TatCa, int ChuaDoc, int DaDoc) DemTheoTrangThai(int maTaiKhoan, string? tuKhoa)
        {
            const string sql = @"SELECT
    COUNT(*) AS tatCa,
    COALESCE(SUM(CASE WHEN tb.daDoc = 0 THEN 1 ELSE 0 END), 0) AS chuaDoc,
    COALESCE(SUM(CASE WHEN tb.daDoc = 1 THEN 1 ELSE 0 END), 0) AS daDoc
FROM ThongBao tb
LEFT JOIN LichTiem lt ON tb.maLichTiem = lt.maLichTiem
LEFT JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo AND hs.maTaiKhoan = tb.maTaiKhoan
WHERE tb.maTaiKhoan = @MaTaiKhoan
AND (
    @TuKhoa = N''
    OR tb.tieuDe COLLATE Latin1_General_100_CI_AI LIKE N'%' + @TuKhoa + N'%'
    OR tb.noiDung COLLATE Latin1_General_100_CI_AI LIKE N'%' + @TuKhoa + N'%'
    OR ISNULL(hs.hoTen, N'') COLLATE Latin1_General_100_CI_AI LIKE N'%' + @TuKhoa + N'%'
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSoBoLoc(lenh, maTaiKhoan, null, tuKhoa);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            if (!doc.Read()) return (0, 0, 0);
            return (Convert.ToInt32(doc["tatCa"]), Convert.ToInt32(doc["chuaDoc"]), Convert.ToInt32(doc["daDoc"]));
        }

        // Đếm ba nhóm nội dung theo cả trạng thái và từ khóa đang chọn.
        public (int QuaHan, int DenLich, int DaCapNhat) DemTheoNhom(int maTaiKhoan, bool? daDoc, string? tuKhoa)
        {
            var dieuKienDaDoc = daDoc.HasValue ? "AND tb.daDoc = @DaDoc" : string.Empty;
            var sql = $@"SELECT
    COALESCE(SUM(CASE WHEN ISNULL(tb.tieuDe, N'') LIKE N'%Quá hạn%' THEN 1 ELSE 0 END), 0) AS quaHan,
    COALESCE(SUM(CASE WHEN ISNULL(tb.tieuDe, N'') NOT LIKE N'%Quá hạn%'
        AND (ISNULL(tb.tieuDe, N'') LIKE N'%đến lịch%' OR ISNULL(tb.tieuDe, N'') LIKE N'%Hôm nay%lịch tiêm%') THEN 1 ELSE 0 END), 0) AS denLich,
    COALESCE(SUM(CASE WHEN ISNULL(tb.tieuDe, N'') NOT LIKE N'%Quá hạn%'
        AND ISNULL(tb.tieuDe, N'') NOT LIKE N'%đến lịch%'
        AND ISNULL(tb.tieuDe, N'') NOT LIKE N'%Hôm nay%lịch tiêm%' THEN 1 ELSE 0 END), 0) AS daCapNhat
FROM ThongBao tb
LEFT JOIN LichTiem lt ON tb.maLichTiem = lt.maLichTiem
LEFT JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo AND hs.maTaiKhoan = tb.maTaiKhoan
WHERE tb.maTaiKhoan = @MaTaiKhoan
{dieuKienDaDoc}
AND (
    @TuKhoa = N''
    OR tb.tieuDe COLLATE Latin1_General_100_CI_AI LIKE N'%' + @TuKhoa + N'%'
    OR tb.noiDung COLLATE Latin1_General_100_CI_AI LIKE N'%' + @TuKhoa + N'%'
    OR ISNULL(hs.hoTen, N'') COLLATE Latin1_General_100_CI_AI LIKE N'%' + @TuKhoa + N'%'
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSoBoLoc(lenh, maTaiKhoan, daDoc, tuKhoa);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            if (!doc.Read()) return (0, 0, 0);
            return (Convert.ToInt32(doc["quaHan"]), Convert.ToInt32(doc["denLich"]), Convert.ToInt32(doc["daCapNhat"]));
        }

        public int DemTongThongBao(int maTaiKhoan)
        {
            const string sql = "SELECT COUNT(*) FROM ThongBao WHERE maTaiKhoan = @MaTaiKhoan";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
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

        public List<ThongBao> LayThongBaoChuaDocMoiNhat(int maTaiKhoan, int soLuong)
        {
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
AND daDoc = 0
ORDER BY ngayGui DESC";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@SoLuong", soLuong);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<ThongBao>();
            while (doc.Read())
            {
                danhSach.Add(DocThongBao(doc));
            }

            return danhSach;
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

        public List<ThongBao> LayThongBaoGanDay(int maTaiKhoan, int soNgay)
        {
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
AND ngayGui >= DATEADD(DAY, -@SoNgay, GETDATE())
ORDER BY ngayGui DESC";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@SoNgay", soNgay);

            ketNoi.Open();
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
    tb.maThongBao, tb.maTaiKhoan, tb.maLichTiem, tb.tieuDe, tb.noiDung, tb.ngayGui, tb.daDoc,
    lt.maHoSo, hs.hoTen AS hoTenHoSo
FROM ThongBao tb
LEFT JOIN LichTiem lt ON tb.maLichTiem = lt.maLichTiem
LEFT JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo AND hs.maTaiKhoan = tb.maTaiKhoan
WHERE tb.maThongBao = @MaThongBao
AND tb.maTaiKhoan = @MaTaiKhoan";

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

        // Chỉ cập nhật thông báo chưa đọc thuộc đúng tài khoản hiện tại.
        public int DanhDauTatCaDaDoc(int maTaiKhoan)
        {
            const string sql = @"UPDATE ThongBao
SET daDoc = 1
WHERE maTaiKhoan = @MaTaiKhoan
AND daDoc = 0";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            return lenh.ExecuteNonQuery();
        }

        // Tạo thông báo cho các lịch tiêm đã đến hạn hoặc quá hạn, không tạo trùng khi người dùng mở trang nhiều lần.
        public int TaoThongBaoLichTiemDenHan(int maTaiKhoan)
        {
            XoaThongBaoNhacLichKhongConHieuLuc(maTaiKhoan);

            if (DateTime.Now.TimeOfDay < TimeSpan.FromHours(GioGuiThongBaoNhacLich))
            {
                return 0;
            }

            var danhSachLich = LayLichTiemDenHan(maTaiKhoan);
            var soLichDenHanTheoHoSo = danhSachLich
                .GroupBy(lich => lich.MaHoSo)
                .ToDictionary(nhom => nhom.Key, nhom => nhom.Count());
            var soThongBaoDaTao = 0;

            foreach (var lich in danhSachLich)
            {
                var tieuDe = TaoTieuDeThongBao(lich);

                if (DaCoThongBaoChoLich(maTaiKhoan, lich.MaLichTiem, tieuDe))
                {
                    continue;
                }

                var noiDung = TaoNoiDungThongBao(lich, soLichDenHanTheoHoSo.GetValueOrDefault(lich.MaHoSo) > 1);

                var maThongBao = ThemThongBaoLichTiem(maTaiKhoan, lich.MaLichTiem, tieuDe, noiDung);
                if (maThongBao > 0)
                {
                    pushNotificationService.GuiThongBao(maTaiKhoan, maThongBao, tieuDe, noiDung);
                    soThongBaoDaTao++;
                }
            }

            return soThongBaoDaTao;
        }

        // Lấy các lịch chưa tiêm sắp đến hạn, đúng ngày hoặc quá hạn của tài khoản hiện tại.
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
AND CAST(lt.ngayTiemDuKien AS DATE) <= DATEADD(DAY, @SoNgayNhacTruoc, CAST(GETDATE() AS DATE))
AND lt.trangThai = @TrangThaiChuaTiem
ORDER BY lt.ngayTiemDuKien, v.tenVaccine, mt.soMui";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@SoNgayNhacTruoc", SoNgayNhacTruoc);
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

        private bool DaCoThongBaoChoLich(int maTaiKhoan, int maLichTiem, string tieuDe)
        {
            const string sql = @"SELECT COUNT(*)
FROM ThongBao
WHERE maTaiKhoan = @maTaiKhoan
AND maLichTiem = @maLichTiem
AND tieuDe = @tieuDe";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@maLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@tieuDe", tieuDe);

            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        private int ThemThongBaoLichTiem(int maTaiKhoan, int maLichTiem, string tieuDe, string noiDung)
        {
            const string sql = @"INSERT INTO ThongBao(maTaiKhoan, maLichTiem, tieuDe, noiDung, ngayGui, daDoc)
OUTPUT INSERTED.maThongBao
VALUES(@maTaiKhoan, @maLichTiem, @tieuDe, @noiDung, @ngayGui, 0)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@maLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@tieuDe", tieuDe);
            lenh.Parameters.AddWithValue("@noiDung", noiDung);
            lenh.Parameters.AddWithValue("@ngayGui", DateTime.Today.AddHours(GioGuiThongBaoNhacLich));

            ketNoi.Open();
            var ketQua = lenh.ExecuteScalar();
            return ketQua == null || ketQua == DBNull.Value ? 0 : Convert.ToInt32(ketQua);
        }

        private static string TaoTieuDeThongBao(LichTiemCanThongBao lich)
        {
            if (lich.NgayTiemDuKien.Date < DateTime.Today)
            {
                return "Quá hạn lịch tiêm";
            }

            return lich.NgayTiemDuKien.Date == DateTime.Today
                ? "Hôm nay là lịch tiêm"
                : "Sắp đến lịch tiêm";
        }

        private static string TaoNoiDungThongBao(LichTiemCanThongBao lich, bool coMuiKhacDenHan)
        {
            var ngayTiem = lich.NgayTiemDuKien.ToString("dd-MM-yyyy");
            var soNgayConLai = (lich.NgayTiemDuKien.Date - DateTime.Today).Days;
            var thongTinMui = $"mũi {lich.SoMui}";
            if (!string.IsNullOrWhiteSpace(lich.TenMui))
            {
                thongTinMui += $" - {lich.TenMui}";
            }

            string noiDung;
            if (lich.NgayTiemDuKien.Date < DateTime.Today)
            {
                noiDung = $"{lich.HoTenHoSo} đã quá hạn {thongTinMui} {lich.TenVaccine}. Hãy cập nhật khi đã tiêm.";
            }
            else if (lich.NgayTiemDuKien.Date == DateTime.Today)
            {
                noiDung = $"Hôm nay {lich.HoTenHoSo} đến lịch tiêm {thongTinMui} {lich.TenVaccine}. Đã tiêm chưa?";
            }
            else
            {
                noiDung = $"Còn {soNgayConLai} ngày đến lịch tiêm của {lich.HoTenHoSo}: {thongTinMui} {lich.TenVaccine}.";
            }

            if (coMuiKhacDenHan)
            {
                noiDung += " Có thêm mũi khác cần xem.";
            }

            return noiDung;
        }

        private void XoaThongBaoNhacLichKhongConHieuLuc(int maTaiKhoan)
        {
            const string sql = @"DELETE tb
FROM ThongBao tb
LEFT JOIN LichTiem lt ON tb.maLichTiem = lt.maLichTiem
WHERE tb.maTaiKhoan = @MaTaiKhoan
AND tb.tieuDe IN (@TieuDeSapDen, @TieuDeHomNay, @TieuDeDenHanCu, @TieuDeQuaHan)
AND (
    tb.ngayGui < DATEADD(DAY, -@SoNgayGiu, GETDATE())
    OR lt.maLichTiem IS NULL
    OR lt.trangThai <> @TrangThaiChuaTiem
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@TieuDeSapDen", TieuDeThongBaoNhacLich[0]);
            lenh.Parameters.AddWithValue("@TieuDeHomNay", TieuDeThongBaoNhacLich[1]);
            lenh.Parameters.AddWithValue("@TieuDeDenHanCu", TieuDeThongBaoNhacLich[2]);
            lenh.Parameters.AddWithValue("@TieuDeQuaHan", TieuDeThongBaoNhacLich[3]);
            lenh.Parameters.AddWithValue("@SoNgayGiu", SoNgayGiuThongBaoNhacLich);
            lenh.Parameters.AddWithValue("@TrangThaiChuaTiem", "Chưa tiêm");

            ketNoi.Open();
            lenh.ExecuteNonQuery();
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
                DaDoc = Convert.ToBoolean(doc["daDoc"]),
                MaHoSo = CoCot(doc, "maHoSo") && doc["maHoSo"] != DBNull.Value ? Convert.ToInt32(doc["maHoSo"]) : null,
                HoTenHoSo = CoCot(doc, "hoTenHoSo") && doc["hoTenHoSo"] != DBNull.Value
                    ? doc["hoTenHoSo"].ToString() ?? string.Empty
                    : string.Empty
            };
        }

        private static void GanThamSoBoLoc(SqlCommand lenh, int maTaiKhoan, bool? daDoc, string? tuKhoa)
        {
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@TuKhoa", string.IsNullOrWhiteSpace(tuKhoa) ? string.Empty : tuKhoa.Trim());
            if (daDoc.HasValue)
            {
                lenh.Parameters.AddWithValue("@DaDoc", daDoc.Value);
            }
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


