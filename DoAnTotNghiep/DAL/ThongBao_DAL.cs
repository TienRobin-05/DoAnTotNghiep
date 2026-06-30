using DoAnTotNghiep.Models;
using DoAnTotNghiep.Services;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class ThongBao_DAL
    {
        private readonly string chuoiKetNoi;
        private const string BieuThucNhomThongBao = @"CASE
        WHEN lt.maLichTiem IS NOT NULL
            AND lt.trangThai = N'Đã tiêm' THEN N'da-cap-nhat'
        WHEN lt.maLichTiem IS NOT NULL
            AND ISNULL(lt.trangThai, N'') <> N'Đã tiêm'
            AND CAST(lt.ngayTiemDuKien AS DATE) < CAST(GETDATE() AS DATE) THEN N'qua-han'
        WHEN (lt.maLichTiem IS NULL OR ISNULL(lt.trangThai, N'') <> N'Đã tiêm')
            AND ISNULL(tb.tieuDe, N'') LIKE N'%Quá hạn%' THEN N'qua-han'
        WHEN lt.maLichTiem IS NOT NULL
            AND ISNULL(lt.trangThai, N'') <> N'Đã tiêm'
            AND CAST(lt.ngayTiemDuKien AS DATE) <= DATEADD(DAY, 3, CAST(GETDATE() AS DATE)) THEN N'den-lich'
        WHEN (lt.maLichTiem IS NULL OR ISNULL(lt.trangThai, N'') <> N'Đã tiêm')
            AND (ISNULL(tb.tieuDe, N'') LIKE N'%đến lịch%' OR ISNULL(tb.tieuDe, N'') LIKE N'%Sắp đến lịch%') THEN N'den-lich'
        ELSE N'da-cap-nhat'
    END";
        private static readonly string[] TieuDeThongBaoNhacLich =
        {
            "Sắp đến lịch tiêm",
            "Đến lịch tiêm hôm nay",
            "Quá hạn lịch tiêm"
        };
        private const int GioGuiThongBaoNhacLich = 0;
        private const int SoNgayNhacTruoc = 3;
        private const int SoNgayGiuThongBaoNhacLich = 5;
        private readonly PushNotificationService pushNotificationService;

        public ThongBao_DAL(IConfiguration configuration, PushNotificationService pushNotificationService)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            this.pushNotificationService = pushNotificationService;
        }

        // lấy danh sách thông báo
        public List<ThongBao> LayDanhSachTheoTaiKhoan(int maTaiKhoan)
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
ORDER BY ngayGui DESC";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
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

        // Lấy đúng một trang thông báo sau khi lọc/tìm kiếm; không tải toàn bộ dữ liệu về View.
        public List<ThongBao> LayTrangTheoTaiKhoan(int maTaiKhoan, bool? daDoc, string? tuKhoa, int trang, int soDongMoiTrang)
        {
            var dieuKienDaDoc = daDoc.HasValue ? "AND tb.daDoc = @DaDoc" : string.Empty;
            var sql = $@"SELECT
    tb.maThongBao, tb.maTaiKhoan, tb.maLichTiem, tb.tieuDe, tb.noiDung, tb.ngayGui, tb.daDoc,
    lt.maHoSo, hs.hoTen AS hoTenHoSo,
    {BieuThucNhomThongBao} AS nhomThongBao
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
    COALESCE(SUM(CASE WHEN {BieuThucNhomThongBao} = N'qua-han' THEN 1 ELSE 0 END), 0) AS quaHan,
    COALESCE(SUM(CASE WHEN {BieuThucNhomThongBao} = N'den-lich' THEN 1 ELSE 0 END), 0) AS denLich,
    COALESCE(SUM(CASE WHEN {BieuThucNhomThongBao} = N'da-cap-nhat' THEN 1 ELSE 0 END), 0) AS daCapNhat
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

        // đếm tổng thông báo
        public int DemTongThongBao(int maTaiKhoan)
        {
            const string sql = "SELECT COUNT(*) FROM ThongBao WHERE maTaiKhoan = @MaTaiKhoan";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }

        // Xóa thông báo nhắc lịch của các mũi chưa tiêm thuộc hồ sơ (khi đổi ngày sinh)
        public int XoaThongBaoNhacLichTheoHoSo(int maHoSo, int maTaiKhoan)
        {
            const string sql = @"DELETE tb
FROM ThongBao tb
INNER JOIN LichTiem lt ON tb.maLichTiem = lt.maLichTiem
WHERE lt.maHoSo = @MaHoSo
AND tb.maTaiKhoan = @MaTaiKhoan
AND (lt.trangThai IS NULL OR lt.trangThai <> N'Đã tiêm')
AND tb.tieuDe IN (@TieuDeSapDen, @TieuDeHomNay, @TieuDeQuaHan)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@TieuDeSapDen", TieuDeThongBaoNhacLich[0]);
            lenh.Parameters.AddWithValue("@TieuDeHomNay", TieuDeThongBaoNhacLich[1]);
            lenh.Parameters.AddWithValue("@TieuDeQuaHan", TieuDeThongBaoNhacLich[2]);
            ketNoi.Open();
            return lenh.ExecuteNonQuery();
        }

        // đếm thông báo chưa đọc
        public int DemThongBaoChuaDoc(int maTaiKhoan)
        {
            const string sql = @"SELECT COUNT(*)
FROM ThongBao
WHERE maTaiKhoan = @MaTaiKhoan
AND daDoc = 0";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

                        ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }

        // lấy thông báo chưa đọc gần nhất
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

        public List<ThongBao> LayThongBaoMoiNhat(int maTaiKhoan, int soLuong)
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

        // lấy thông báo gần đây
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

        // lấy thông báo theo mã
        public ThongBao? LayTheoId(int maThongBao, int maTaiKhoan)
        {
            var sql = $@"SELECT
    tb.maThongBao, tb.maTaiKhoan, tb.maLichTiem, tb.tieuDe, tb.noiDung, tb.ngayGui, tb.daDoc,
    lt.maHoSo, hs.hoTen AS hoTenHoSo,
    {BieuThucNhomThongBao} AS nhomThongBao
FROM ThongBao tb
LEFT JOIN LichTiem lt ON tb.maLichTiem = lt.maLichTiem
LEFT JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo AND hs.maTaiKhoan = tb.maTaiKhoan
WHERE tb.maThongBao = @MaThongBao
AND tb.maTaiKhoan = @MaTaiKhoan";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaThongBao", maThongBao);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

                        ketNoi.Open();
                        using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocThongBao(doc) : null;
        }

        // tạo thông báo mới
        public bool Them(ThongBao tb)
        {
            const string sql = @"INSERT INTO ThongBao(maTaiKhoan, maLichTiem, tieuDe, noiDung, ngayGui, daDoc)
VALUES(@MaTaiKhoan, @MaLichTiem, @TieuDe, @NoiDung, @NgayGui, @DaDoc)";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", tb.MaTaiKhoan);
            lenh.Parameters.AddWithValue("@MaLichTiem", (object?)tb.MaLichTiem ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@TieuDe", tb.TieuDe);
            lenh.Parameters.AddWithValue("@NoiDung", string.IsNullOrWhiteSpace(tb.NoiDung) ? DBNull.Value : tb.NoiDung);
            lenh.Parameters.AddWithValue("@NgayGui", tb.NgayGui);
            lenh.Parameters.AddWithValue("@DaDoc", tb.DaDoc);

                        ketNoi.Open();
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
            foreach (var l in danhSachLich)
            {
                System.Console.WriteLine($"[ThongBao_DAL]   Lich maLichTiem={l.MaLichTiem}, hoSo={l.HoTenHoSo}, " +
                    $"vaccine={l.TenVaccine}, ngay={l.NgayTiemDuKien:yyyy-MM-dd}, " +
                    $"soNgay={(l.NgayTiemDuKien.Date - DateTime.Today).Days}");
            }

            var soThongBaoDaTao = 0;

            foreach (var lich in danhSachLich)
            {
                var tieuDe = TaoTieuDeThongBao(lich);

                if (DaCoThongBaoChoLich(maTaiKhoan, lich.MaLichTiem, tieuDe))
                {
                    continue;
                }

                var noiDung = TaoNoiDungThongBao(lich);

                var maThongBao = ThemThongBaoLichTiem(maTaiKhoan, lich.MaLichTiem, tieuDe, noiDung);
                if (maThongBao > 0)
                {
                    System.Console.WriteLine($"[ThongBao_DAL]   Tao thong bao maThongBao={maThongBao}, tieuDe={tieuDe}, " +
                        $"ngay={lich.NgayTiemDuKien:yyyy-MM-dd}, soNgay={(lich.NgayTiemDuKien.Date - DateTime.Today).Days}");
                    pushNotificationService.GuiThongBao(maTaiKhoan, maThongBao, tieuDe, noiDung);
                    soThongBaoDaTao++;
                }
            }

            return soThongBaoDaTao;
        }

        // Lấy các lịch chưa tiêm trong khoảng: hôm qua (-1 ngày) đến 3 ngày tới.
        // Các mũi quá hạn > 1 ngày (diffDays < -1) bị loại: không tạo thông báo.
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
AND CAST(lt.ngayTiemDuKien AS DATE) >= DATEADD(DAY, -1, CAST(GETDATE() AS DATE))
AND CAST(lt.ngayTiemDuKien AS DATE) <= DATEADD(DAY, @SoNgayNhacTruoc, CAST(GETDATE() AS DATE))
AND ISNULL(lt.trangThai, N'') <> @TrangThaiDaTiem
ORDER BY lt.ngayTiemDuKien, v.tenVaccine, mt.soMui";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@SoNgayNhacTruoc", SoNgayNhacTruoc);
            lenh.Parameters.AddWithValue("@TrangThaiDaTiem", "Đã tiêm");

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

        // kiểm tra đã có thông báo cho lịch chưa
        private bool DaCoThongBaoChoLich(int maTaiKhoan, int maLichTiem, string tieuDe)
        {
            const string sql = @"SELECT COUNT(*)
FROM ThongBao
WHERE maTaiKhoan = @maTaiKhoan
AND maLichTiem = @maLichTiem
AND tieuDe = @tieuDe
AND CONVERT(DATE, ngayGui) = CONVERT(DATE, @homNay)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@maLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@tieuDe", tieuDe);
            lenh.Parameters.AddWithValue("@homNay", DateTime.Today);

            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // thêm thông báo lịch tiêm
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

        // tạo nội dung thông báo
        private static string TaoTieuDeThongBao(LichTiemCanThongBao lich)
        {
            if (lich.NgayTiemDuKien.Date < DateTime.Today)
            {
                return "Quá hạn lịch tiêm";
            }

            return lich.NgayTiemDuKien.Date == DateTime.Today
                ? "Đến lịch tiêm hôm nay"
                : "Sắp đến lịch tiêm";
        }

        // tạo nội dung thông báo
        private static string TaoNoiDungThongBao(LichTiemCanThongBao lich)
        {
            var thongTinMui = $"Mũi {lich.SoMui}";

            if (lich.NgayTiemDuKien.Date < DateTime.Today)
            {
                return $"Hồ sơ {lich.HoTenHoSo} đã quá hạn {thongTinMui} - {lich.TenVaccine}. Vui lòng kiểm tra lịch tiêm và cập nhật trạng thái nếu đã tiêm.";
            }

            if (lich.NgayTiemDuKien.Date == DateTime.Today)
            {
                return $"Hôm nay, hồ sơ {lich.HoTenHoSo} đến lịch tiêm {thongTinMui} - {lich.TenVaccine}. Vui lòng kiểm tra lịch tiêm để thực hiện đúng hẹn.";
            }

            var soNgayConLai = (lich.NgayTiemDuKien.Date - DateTime.Today).Days;
            return $"Còn {soNgayConLai} ngày nữa, hồ sơ {lich.HoTenHoSo} sẽ đến lịch tiêm {thongTinMui} - {lich.TenVaccine}.";
        }

        // xóa thông báo nhắc lịch hết hiệu lực
        private void XoaThongBaoNhacLichKhongConHieuLuc(int maTaiKhoan)
        {
            const string sql = @"DELETE tb
FROM ThongBao tb
LEFT JOIN LichTiem lt ON tb.maLichTiem = lt.maLichTiem
WHERE tb.maTaiKhoan = @MaTaiKhoan
AND tb.tieuDe IN (@TieuDeSapDen, @TieuDeHomNay, @TieuDeQuaHan)
AND (
    tb.ngayGui < DATEADD(DAY, -@SoNgayGiu, GETDATE())
    OR lt.maLichTiem IS NULL
    OR lt.trangThai = @TrangThaiDaTiem
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@TieuDeSapDen", TieuDeThongBaoNhacLich[0]);
            lenh.Parameters.AddWithValue("@TieuDeHomNay", TieuDeThongBaoNhacLich[1]);
            lenh.Parameters.AddWithValue("@TieuDeQuaHan", TieuDeThongBaoNhacLich[2]);
            lenh.Parameters.AddWithValue("@SoNgayGiu", SoNgayGiuThongBaoNhacLich);
            lenh.Parameters.AddWithValue("@TrangThaiDaTiem", "Đã tiêm");

            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // đánh dấu đã đọc
        public bool DanhDauDaDoc(int maThongBao, int maTaiKhoan)
        {
            const string sql = @"UPDATE ThongBao
SET daDoc = 1
WHERE maThongBao = @MaThongBao
AND maTaiKhoan = @MaTaiKhoan";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaThongBao", maThongBao);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

                        ketNoi.Open();
                        return lenh.ExecuteNonQuery() > 0;
        }

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
                    : string.Empty,
                NhomThongBao = CoCot(doc, "nhomThongBao") && doc["nhomThongBao"] != DBNull.Value
                    ? doc["nhomThongBao"].ToString() ?? "da-cap-nhat"
                    : "da-cap-nhat"
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

        // Lấy tối đa N thông báo chưa đọc chưa từng được đẩy desktop
        // Trả về thông tin chi tiết để client gộp notification desktop theo hồ sơ
        public List<DesktopPushNotification> LayThongBaoChuaDocChoDesktopPush(int maTaiKhoan, int soLuong)
        {
            DamBaoBangDesktopPushLog();
            const string sql = @"
SELECT TOP (@SoLuong)
    tb.maThongBao, tb.tieuDe, tb.noiDung, tb.ngayGui,
    lt.maHoSo, hs.hoTen AS hoTenHoSo,
    v.tenVaccine, mt.tenMui, mt.soMui
FROM ThongBao tb
LEFT JOIN LichTiem lt ON tb.maLichTiem = lt.maLichTiem
LEFT JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo AND hs.maTaiKhoan = tb.maTaiKhoan
LEFT JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
LEFT JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE tb.maTaiKhoan = @MaTaiKhoan
AND tb.daDoc = 0
AND NOT EXISTS (
    SELECT 1
    FROM NotificationDesktopPushLogs l
    WHERE l.maTaiKhoan = tb.maTaiKhoan
    AND l.maThongBao = tb.maThongBao
)
ORDER BY tb.ngayGui DESC";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@SoLuong", soLuong);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<DesktopPushNotification>();
            while (doc.Read())
            {
                var item = new DesktopPushNotification
                {
                    Id = Convert.ToInt32(doc["maThongBao"]),
                    Title = doc["tieuDe"] == DBNull.Value ? "" : doc["tieuDe"].ToString() ?? "",
                    Message = doc["noiDung"] == DBNull.Value ? "" : doc["noiDung"].ToString() ?? "",
                    CreatedAt = Convert.ToDateTime(doc["ngayGui"]),
                    MaHoSo = doc["maHoSo"] == DBNull.Value ? null : Convert.ToInt32(doc["maHoSo"]),
                    HoTenHoSo = doc["hoTenHoSo"] == DBNull.Value ? "" : doc["hoTenHoSo"].ToString() ?? "",
                    TenVaccine = doc["tenVaccine"] == DBNull.Value ? "" : doc["tenVaccine"].ToString() ?? "",
                    TenMui = doc["tenMui"] == DBNull.Value ? "" : doc["tenMui"].ToString() ?? "",
                    SoMui = doc["soMui"] == DBNull.Value ? 0 : Convert.ToInt32(doc["soMui"])
                };
                item.LoaiThongBao = GetLoaiThongBao(item.Title);
                danhSach.Add(item);
            }
            return danhSach;
        }

        // lấy loại thông báo
        private static string GetLoaiThongBao(string tieuDe)
        {
            if (tieuDe.Contains("Quá hạn")) return "overdue";
            if (tieuDe.Contains("Đến lịch tiêm hôm nay")) return "due_today";
            if (tieuDe.Contains("Sắp đến lịch")) return "upcoming";
            return "updated";
        }

        // Đánh dấu các thông báo đã được đẩy desktop (chống trùng)
        public void MarkDesktopPushed(int maTaiKhoan, List<int> notificationIds)
        {
            if (notificationIds == null || notificationIds.Count == 0) return;
            DamBaoBangDesktopPushLog();

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            ketNoi.Open();
            foreach (var id in notificationIds)
            {
                const string sql = @"
IF NOT EXISTS (SELECT 1 FROM NotificationDesktopPushLogs WHERE maTaiKhoan = @MaTaiKhoan AND maThongBao = @MaThongBao)
    INSERT INTO NotificationDesktopPushLogs(maTaiKhoan, maThongBao) VALUES(@MaTaiKhoan, @MaThongBao)";

                using var lenh = new SqlCommand(sql, ketNoi);
                lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
                lenh.Parameters.AddWithValue("@MaThongBao", id);
                lenh.ExecuteNonQuery();
            }
        }

        // đảm bảo bảng log push tồn tại
        private void DamBaoBangDesktopPushLog()
        {
            const string sql = @"
IF OBJECT_ID(N'dbo.NotificationDesktopPushLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NotificationDesktopPushLogs (
        maLog INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_NotificationDesktopPushLogs PRIMARY KEY,
        maTaiKhoan INT NOT NULL,
        maThongBao INT NOT NULL,
        pushedAt DATETIME NOT NULL CONSTRAINT DF_NotifDesktopPush_pushedAt DEFAULT GETDATE(),
        CONSTRAINT UQ_NotifDesktopPush_UserNotif UNIQUE(maTaiKhoan, maThongBao)
    );
END;";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
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

        // Model cho desktop push notification
        public class DesktopPushNotification
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string Message { get; set; } = "";
            public DateTime CreatedAt { get; set; }
            public int? MaHoSo { get; set; }
            public string HoTenHoSo { get; set; } = "";
            public string TenVaccine { get; set; } = "";
            public string TenMui { get; set; } = "";
            public int SoMui { get; set; }
            public string LoaiThongBao { get; set; } = "";
        }
    }
}

