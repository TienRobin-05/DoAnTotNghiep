using DoAnTotNghiep.Models;
using DoAnTotNghiep.Services;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp TaiKhoan_DAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
    public class TaiKhoan_DAL
    {
        private readonly string chuoiKetNoi;

        public TaiKhoan_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        // Mục đích: phương thức KiemTraSoDienThoaiTonTai thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool KiemTraSoDienThoaiTonTai(string soDienThoai)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = "SELECT COUNT(*) FROM TaiKhoan WHERE soDienThoai = @SoDienThoai";
            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // Mục đích: phương thức KiemTraEmailTonTai thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool KiemTraEmailTonTai(string email)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = "SELECT COUNT(*) FROM TaiKhoan WHERE email = @Email";
            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@Email", email);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // Mục đích: phương thức DangKy thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool DangKy(TaiKhoan tk)
        {
            DamBaoCotDonDepTaiKhoan();
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
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

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSoTaiKhoan(lenh, tk);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteNonQuery trả về số dòng bị ảnh hưởng; lớn hơn 0 nghĩa là thêm/sửa/xóa thành công.
            return lenh.ExecuteNonQuery() > 0;
        }

        // Mục đích: phương thức DangNhap thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public TaiKhoan? DangNhap(string soDienThoai, string matKhau)
        {
            DamBaoCotDonDepTaiKhoan();
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
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

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
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

        // Mục đích: phương thức LayTaiKhoanTheoId thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public TaiKhoan? LayTaiKhoanTheoId(int maTaiKhoan)
        {
            DamBaoCotDonDepTaiKhoan();
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
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

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocTaiKhoan(doc) : null;
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

        // Mục đích: phương thức CapNhatHoTen thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool CapNhatHoTen(int maTaiKhoan, string hoTen)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"UPDATE TaiKhoan
SET hoTen = @HoTen
WHERE maTaiKhoan = @MaTaiKhoan";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@HoTen", hoTen);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteNonQuery trả về số dòng bị ảnh hưởng; lớn hơn 0 nghĩa là thêm/sửa/xóa thành công.
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

        // Mục đích: phương thức GanThamSoTaiKhoan thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
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

        // Mục đích: phương thức DocTaiKhoan thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
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
END;";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        private static string ChuanHoaVaiTro(string? vaiTro)
        {
            if (string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase))
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
