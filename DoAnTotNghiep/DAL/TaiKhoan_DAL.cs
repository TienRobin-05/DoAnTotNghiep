using DoAnTotNghiep.Models;
using DoAnTotNghiep.Services;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class TaiKhoan_DAL
    {
        private readonly string chuoiKetNoi;

        public TaiKhoan_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public bool KiemTraSoDienThoaiTonTai(string soDienThoai)
        {
            const string sql = "SELECT COUNT(*) FROM TaiKhoan WHERE soDienThoai = @SoDienThoai";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        public bool KiemTraEmailTonTai(string email)
        {
            const string sql = "SELECT COUNT(*) FROM TaiKhoan WHERE email = @Email";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@Email", email);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        public bool DangKy(TaiKhoan tk)
        {
            DamBaoCotDonDepTaiKhoan();
            const string sql = @"INSERT INTO TaiKhoan
(
    hoTen,
    email,
    matKhau,
    soDienThoai,
    vaiTro,
    trangThai,
    ngayTao,
    LanDangNhapCuoi,
    DaXoa,
    NgayXoaMem,
    LyDoXoa
)
VALUES
(
    @HoTen,
    @Email,
    @MatKhau,
    @SoDienThoai,
    @VaiTro,
    @TrangThai,
    @NgayTao,
    @LanDangNhapCuoi,
    0,
    NULL,
    NULL
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSoTaiKhoan(lenh, tk);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public TaiKhoan? DangNhap(string soDienThoai, string matKhau)
        {
            DamBaoCotDonDepTaiKhoan();
            const string sql = @"SELECT
    maTaiKhoan,
    hoTen,
    email,
    matKhau,
    soDienThoai,
    vaiTro,
    trangThai,
    ngayTao,
    LanDangNhapCuoi,
    DaXoa,
    NgayXoaMem,
    LyDoXoa
FROM TaiKhoan
WHERE soDienThoai = @SoDienThoai";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            if (!doc.Read())
            {
                return null;
            }

            var taiKhoan = DocTaiKhoan(doc);
            if (!MatKhauService.KiemTra(matKhau, taiKhoan.MatKhau))
            {
                return null;
            }

            if (!MatKhauService.LaHash(taiKhoan.MatKhau))
            {
                doc.Close();
                CapNhatMatKhau(taiKhoan.MaTaiKhoan, MatKhauService.TaoHash(matKhau));
            }

            return taiKhoan;
        }

        // lấy tài khoản theo mã
        public TaiKhoan? LayTaiKhoanTheoId(int maTaiKhoan)
        {
            DamBaoCotDonDepTaiKhoan();
            const string sql = @"SELECT
    maTaiKhoan,
    hoTen,
    email,
    matKhau,
    soDienThoai,
    vaiTro,
    trangThai,
    ngayTao,
    LanDangNhapCuoi,
    DaXoa,
    NgayXoaMem,
    LyDoXoa
FROM TaiKhoan
WHERE maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocTaiKhoan(doc) : null;
        }

        public List<TaiKhoan> LayTatCa()
        {
            DamBaoCotDonDepTaiKhoan();
            const string sql = @"SELECT
    maTaiKhoan,
    hoTen,
    email,
    matKhau,
    soDienThoai,
    vaiTro,
    trangThai,
    ngayTao,
    LanDangNhapCuoi,
    DaXoa,
    NgayXoaMem,
    LyDoXoa
FROM TaiKhoan
ORDER BY maTaiKhoan DESC";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<TaiKhoan>();
            while (doc.Read())
            {
                danhSach.Add(DocTaiKhoan(doc));
            }

            return danhSach;
        }

        public int DemTatCa()
        {
            const string sql = "SELECT COUNT(*) FROM TaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }

        public bool DoiTrangThai(int maTaiKhoan, bool trangThai)
        {
            const string sql = "UPDATE TaiKhoan SET trangThai = @TrangThai WHERE maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@TrangThai", trangThai);

            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public TaiKhoan? LayTaiKhoanTheoSoDienThoaiVaEmail(string soDienThoai, string email)
        {
            DamBaoCotDonDepTaiKhoan();
            const string sql = @"SELECT
    maTaiKhoan,
    hoTen,
    email,
    matKhau,
    soDienThoai,
    vaiTro,
    trangThai,
    ngayTao,
    LanDangNhapCuoi,
    DaXoa,
    NgayXoaMem,
    LyDoXoa
FROM TaiKhoan
WHERE soDienThoai = @SoDienThoai
AND email = @Email";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
            lenh.Parameters.AddWithValue("@Email", email);

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocTaiKhoan(doc) : null;
        }

        // cập nhật họ tên
        public bool CapNhatHoTen(int maTaiKhoan, string hoTen)
        {
            const string sql = @"UPDATE TaiKhoan
SET hoTen = @HoTen
WHERE maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@HoTen", hoTen);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool CapNhatThongTinCaNhan(int maTaiKhoan, string hoTen, string email)
        {
            const string sql = @"UPDATE TaiKhoan
SET hoTen = @HoTen,
    email = @Email
WHERE maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@HoTen", hoTen);
            lenh.Parameters.AddWithValue("@Email", email);

            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool DatLaiMatKhau(int maTaiKhoan, string matKhauMoi)
        {
            const string sql = @"UPDATE TaiKhoan
SET matKhau = @MatKhau
WHERE maTaiKhoan = @MaTaiKhoan
AND DaXoa = 0";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@MatKhau", MatKhauService.ChuanBiLuu(matKhauMoi));

            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public void CapNhatLanDangNhapCuoi(int maTaiKhoan)
        {
            DamBaoCotDonDepTaiKhoan();
            const string sql = "UPDATE TaiKhoan SET LanDangNhapCuoi = GETDATE() WHERE maTaiKhoan = @MaTaiKhoan AND DaXoa = 0";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        public KetQuaDonDepTaiKhoan DonDepTaiKhoanKhongHoatDong(int? maTaiKhoanDangNhap)
        {
            DamBaoCotDonDepTaiKhoan();

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            ketNoi.Open();
            using var giaoDich = ketNoi.BeginTransaction();

            try
            {
                var soXoaMem = DanhDauTaiKhoanKhongHoatDong(ketNoi, giaoDich, maTaiKhoanDangNhap);
                var soXoaCung = XoaCungTaiKhoanDaXoaMem(ketNoi, giaoDich, maTaiKhoanDangNhap);

                giaoDich.Commit();
                return new KetQuaDonDepTaiKhoan
                {
                    SoTaiKhoanXoaMem = soXoaMem,
                    SoTaiKhoanXoaCung = soXoaCung,
                    ThongBao = $"Đã xóa mềm {soXoaMem} tài khoản, xóa cứng {soXoaCung} tài khoản."
                };
            }
            catch (Exception ex)
            {
                giaoDich.Rollback();
                return new KetQuaDonDepTaiKhoan
                {
                    ThongBao = $"Dọn tài khoản thất bại: {ex.Message}"
                };
            }
        }

        public bool KiemTraEmailTonTaiChoTaiKhoanKhac(int maTaiKhoan, string email)
        {
            const string sql = @"SELECT COUNT(*)
FROM TaiKhoan
WHERE email = @Email
AND maTaiKhoan <> @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@Email", email);

            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        private void CapNhatMatKhau(int maTaiKhoan, string matKhauDaHash)
        {
            const string sql = @"UPDATE TaiKhoan
SET matKhau = @MatKhau
WHERE maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@MatKhau", matKhauDaHash);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        private static int DanhDauTaiKhoanKhongHoatDong(SqlConnection ketNoi, SqlTransaction giaoDich, int? maTaiKhoanDangNhap)
        {
            const string sql = @"
UPDATE TaiKhoan
SET DaXoa = 1,
    NgayXoaMem = GETDATE(),
    LyDoXoa = N'Tài khoản không đăng nhập quá 2 năm'
WHERE DaXoa = 0
AND ISNULL(vaiTro, N'') NOT IN (N'Admin', N'Quản trị viên')
AND (@MaTaiKhoanDangNhap IS NULL OR maTaiKhoan <> @MaTaiKhoanDangNhap)
AND LanDangNhapCuoi IS NOT NULL
AND LanDangNhapCuoi < DATEADD(YEAR, -2, GETDATE());";

            using var lenh = new SqlCommand(sql, ketNoi, giaoDich);
            lenh.Parameters.AddWithValue("@MaTaiKhoanDangNhap", (object?)maTaiKhoanDangNhap ?? DBNull.Value);
            return lenh.ExecuteNonQuery();
        }

        private static int XoaCungTaiKhoanDaXoaMem(SqlConnection ketNoi, SqlTransaction giaoDich, int? maTaiKhoanDangNhap)
        {
            const string sql = @"
DECLARE @TaiKhoanCanXoa TABLE (maTaiKhoan INT PRIMARY KEY);
DECLARE @HoSoCanXoa TABLE (maHoSo INT PRIMARY KEY);
DECLARE @LichTiemCanXoa TABLE (maLichTiem INT PRIMARY KEY);

INSERT INTO @TaiKhoanCanXoa(maTaiKhoan)
SELECT maTaiKhoan
FROM TaiKhoan
WHERE DaXoa = 1
AND NgayXoaMem IS NOT NULL
AND NgayXoaMem < DATEADD(DAY, -90, GETDATE())
AND ISNULL(vaiTro, N'') NOT IN (N'Admin', N'Quản trị viên')
AND (@MaTaiKhoanDangNhap IS NULL OR maTaiKhoan <> @MaTaiKhoanDangNhap);

INSERT INTO @HoSoCanXoa(maHoSo)
SELECT maHoSo
FROM HoSoSucKhoe
WHERE maTaiKhoan IN (SELECT maTaiKhoan FROM @TaiKhoanCanXoa);

INSERT INTO @LichTiemCanXoa(maLichTiem)
SELECT maLichTiem
FROM LichTiem
WHERE maHoSo IN (SELECT maHoSo FROM @HoSoCanXoa);

DELETE FROM LichSuTiem
WHERE maLichTiem IN (SELECT maLichTiem FROM @LichTiemCanXoa);

DELETE FROM ThongBao
WHERE maTaiKhoan IN (SELECT maTaiKhoan FROM @TaiKhoanCanXoa)
OR maLichTiem IN (SELECT maLichTiem FROM @LichTiemCanXoa);

DELETE FROM LichTiem
WHERE maLichTiem IN (SELECT maLichTiem FROM @LichTiemCanXoa);

DELETE FROM HoSoSucKhoe
WHERE maHoSo IN (SELECT maHoSo FROM @HoSoCanXoa);

DELETE FROM PushSubscription
WHERE maTaiKhoan IN (SELECT maTaiKhoan FROM @TaiKhoanCanXoa);

DELETE FROM BaiVietCamNang
WHERE maTaiKhoan IN (SELECT maTaiKhoan FROM @TaiKhoanCanXoa);

UPDATE CauHoiTuVan
SET maNguoiTraLoi = NULL
WHERE maNguoiTraLoi IN (SELECT maTaiKhoan FROM @TaiKhoanCanXoa);

DELETE FROM CauHoiTuVan
WHERE maNguoiGui IN (SELECT maTaiKhoan FROM @TaiKhoanCanXoa);

DELETE FROM TaiKhoan
WHERE maTaiKhoan IN (SELECT maTaiKhoan FROM @TaiKhoanCanXoa);

SELECT @@ROWCOUNT;";

            using var lenh = new SqlCommand(sql, ketNoi, giaoDich);
            lenh.Parameters.AddWithValue("@MaTaiKhoanDangNhap", (object?)maTaiKhoanDangNhap ?? DBNull.Value);
            return Convert.ToInt32(lenh.ExecuteScalar());
        }

        private static void GanThamSoTaiKhoan(SqlCommand lenh, TaiKhoan tk)
        {
            lenh.Parameters.AddWithValue("@HoTen", tk.HoTen);
            lenh.Parameters.AddWithValue("@Email", tk.Email);
            lenh.Parameters.AddWithValue("@MatKhau", MatKhauService.ChuanBiLuu(tk.MatKhau));
            lenh.Parameters.AddWithValue("@SoDienThoai", tk.SoDienThoai);
            lenh.Parameters.AddWithValue("@VaiTro", ChuanHoaVaiTro(tk.VaiTro));
            lenh.Parameters.AddWithValue("@TrangThai", tk.TrangThai);
            lenh.Parameters.AddWithValue("@NgayTao", tk.NgayTao);
            lenh.Parameters.AddWithValue("@LanDangNhapCuoi", (object?)tk.LanDangNhapCuoi ?? DBNull.Value);
        }

        private static TaiKhoan DocTaiKhoan(SqlDataReader doc)
        {
            return new TaiKhoan
            {
                MaTaiKhoan = Convert.ToInt32(doc["maTaiKhoan"]),
                HoTen = doc["hoTen"] == DBNull.Value ? string.Empty : doc["hoTen"].ToString() ?? string.Empty,
                Email = doc["email"] == DBNull.Value ? string.Empty : doc["email"].ToString() ?? string.Empty,
                MatKhau = doc["matKhau"] == DBNull.Value ? string.Empty : doc["matKhau"].ToString() ?? string.Empty,
                SoDienThoai = doc["soDienThoai"] == DBNull.Value ? string.Empty : doc["soDienThoai"].ToString() ?? string.Empty,
                VaiTro = ChuanHoaVaiTro(doc["vaiTro"] == DBNull.Value ? string.Empty : doc["vaiTro"].ToString()),
                TrangThai = DocTrangThai(doc["trangThai"]),
                NgayTao = Convert.ToDateTime(doc["ngayTao"]),
                LanDangNhapCuoi = doc["LanDangNhapCuoi"] == DBNull.Value ? null : Convert.ToDateTime(doc["LanDangNhapCuoi"]),
                DaXoa = Convert.ToBoolean(doc["DaXoa"]),
                NgayXoaMem = doc["NgayXoaMem"] == DBNull.Value ? null : Convert.ToDateTime(doc["NgayXoaMem"]),
                LyDoXoa = doc["LyDoXoa"] == DBNull.Value ? null : doc["LyDoXoa"].ToString()
            };
        }

        public void DamBaoCotDonDepTaiKhoan()
        {
            const string sql = @"
IF COL_LENGTH(N'dbo.TaiKhoan', N'LanDangNhapCuoi') IS NULL
BEGIN
    ALTER TABLE dbo.TaiKhoan ADD LanDangNhapCuoi DATETIME NULL;
END;

IF COL_LENGTH(N'dbo.TaiKhoan', N'DaXoa') IS NULL
BEGIN
    ALTER TABLE dbo.TaiKhoan
    ADD DaXoa BIT NOT NULL CONSTRAINT DF_TaiKhoan_DaXoa DEFAULT 0 WITH VALUES;
END;

IF COL_LENGTH(N'dbo.TaiKhoan', N'NgayXoaMem') IS NULL
BEGIN
    ALTER TABLE dbo.TaiKhoan ADD NgayXoaMem DATETIME NULL;
END;

IF COL_LENGTH(N'dbo.TaiKhoan', N'LyDoXoa') IS NULL
BEGIN
    ALTER TABLE dbo.TaiKhoan ADD LyDoXoa NVARCHAR(255) NULL;
END;

IF COL_LENGTH(N'dbo.TaiKhoan', N'PushNotificationEnabled') IS NULL
BEGIN
    ALTER TABLE dbo.TaiKhoan
    ADD PushNotificationEnabled BIT NOT NULL CONSTRAINT DF_TaiKhoan_PushNotif DEFAULT 0 WITH VALUES;
END;";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // kiểm tra trạng thái thông báo đẩy
        public bool GetPushNotificationEnabled(int maTaiKhoan)
        {
            DamBaoCotDonDepTaiKhoan();
            const string sql = "SELECT ISNULL(PushNotificationEnabled, 0) FROM TaiKhoan WHERE maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            var result = lenh.ExecuteScalar();
            return result != null && result != DBNull.Value && Convert.ToBoolean(result);
        }

        public void SetPushNotificationEnabled(int maTaiKhoan, bool enabled)
        {
            DamBaoCotDonDepTaiKhoan();
            const string sql = "UPDATE TaiKhoan SET PushNotificationEnabled = @Enabled WHERE maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@Enabled", enabled);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        private static string ChuanHoaVaiTro(string? vaiTro)
        {
            if (string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase)
                || string.Equals(vaiTro, "Quản trị viên", StringComparison.OrdinalIgnoreCase))
            {
                return "Admin";
            }

            if (string.Equals(vaiTro, "User", StringComparison.OrdinalIgnoreCase)
                || string.Equals(vaiTro, "NguoiDung", StringComparison.OrdinalIgnoreCase))
            {
                return "User";
            }

            return string.Empty;
        }

        private static bool DocTrangThai(object giaTri)
        {
            if (giaTri == DBNull.Value)
            {
                return false;
            }

            if (giaTri is bool trangThai)
            {
                return trangThai;
            }

            var chuoiTrangThai = giaTri.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(chuoiTrangThai))
            {
                return false;
            }

            return string.Equals(chuoiTrangThai, "true", StringComparison.OrdinalIgnoreCase)
                || chuoiTrangThai == "1"
                || string.Equals(chuoiTrangThai, "Active", StringComparison.OrdinalIgnoreCase)
                || string.Equals(chuoiTrangThai, "HoatDong", StringComparison.OrdinalIgnoreCase);
        }
    }
}
