using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class HoSoSucKhoe_DAL
    {
        private readonly string chuoiKetNoi;

        public HoSoSucKhoe_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        // Mục đích: phương thức KiemTraTaiKhoanDaCoHoSo thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool KiemTraTaiKhoanDaCoHoSo(int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = "SELECT COUNT(*) FROM HoSoSucKhoe WHERE maTaiKhoan = @MaTaiKhoan";
                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
                        ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // Mục đích: phương thức LayDanhSachTheoTaiKhoan thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<HoSoSucKhoe> LayDanhSachTheoTaiKhoan(int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT
    maHoSo,
    maTaiKhoan,
    hoTen,
    ngaySinh,
    gioiTinh,
    chieuCao,
    canNang,
    tienSuBenh,
    diUng,
    ngayTao
FROM HoSoSucKhoe
WHERE maTaiKhoan = @MaTaiKhoan
ORDER BY ngayTao DESC";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
                        ketNoi.Open();
                        using var doc = lenh.ExecuteReader();

            var danhSach = new List<HoSoSucKhoe>();
            while (doc.Read())
            {
                danhSach.Add(DocHoSo(doc));
            }

            return danhSach;
        }

        // Mục đích: phương thức LayTheoId thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public HoSoSucKhoe? LayTheoId(int maHoSo)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT
    maHoSo,
    maTaiKhoan,
    hoTen,
    ngaySinh,
    gioiTinh,
    chieuCao,
    canNang,
    tienSuBenh,
    diUng,
    ngayTao
FROM HoSoSucKhoe
WHERE maHoSo = @MaHoSo";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
                        ketNoi.Open();
                        using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocHoSo(doc) : null;
        }

        // Mục đích: phương thức LayTheoId thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public HoSoSucKhoe? LayTheoId(int maHoSo, int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT
    maHoSo,
    maTaiKhoan,
    hoTen,
    ngaySinh,
    gioiTinh,
    chieuCao,
    canNang,
    tienSuBenh,
    diUng,
    ngayTao
FROM HoSoSucKhoe
WHERE maHoSo = @MaHoSo
AND maTaiKhoan = @MaTaiKhoan";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
                        ketNoi.Open();
                        using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocHoSo(doc) : null;
        }

        // Mục đích: phương thức Them thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool Them(HoSoSucKhoe hs)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"INSERT INTO HoSoSucKhoe
(
    maTaiKhoan,
    hoTen,
    ngaySinh,
    gioiTinh,
    chieuCao,
    canNang,
    tienSuBenh,
    diUng,
    ngayTao
)
VALUES
(
    @MaTaiKhoan,
    @HoTen,
    @NgaySinh,
    @GioiTinh,
    @ChieuCao,
    @CanNang,
    @TienSuBenh,
    @DiUng,
    @NgayTao
)";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSoHoSo(lenh, hs);
                        ketNoi.Open();
                        return lenh.ExecuteNonQuery() > 0;
        }

        // Thêm hồ sơ và trả về maHoSo mới để controller có thể tạo lịch tiêm/thông báo ngay sau khi lưu.
        public int ThemVaLayId(HoSoSucKhoe hs)
        {
            const string sql = @"INSERT INTO HoSoSucKhoe
(
    maTaiKhoan,
    hoTen,
    ngaySinh,
    gioiTinh,
    chieuCao,
    canNang,
    tienSuBenh,
    diUng,
    ngayTao
)
VALUES
(
    @MaTaiKhoan,
    @HoTen,
    @NgaySinh,
    @GioiTinh,
    @ChieuCao,
    @CanNang,
    @TienSuBenh,
    @DiUng,
    @NgayTao
);
SELECT CAST(SCOPE_IDENTITY() AS int);";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSoHoSo(lenh, hs);

            ketNoi.Open();
            var maHoSoMoi = lenh.ExecuteScalar();
            return maHoSoMoi == null || maHoSoMoi == DBNull.Value ? 0 : Convert.ToInt32(maHoSoMoi);
        }

        // Mục đích: phương thức CapNhat thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool CapNhat(HoSoSucKhoe hs)
        {
            const string sql = @"UPDATE HoSoSucKhoe
SET hoTen = @HoTen,
    ngaySinh = @NgaySinh,
    gioiTinh = @GioiTinh,
    chieuCao = @ChieuCao,
    canNang = @CanNang,
    tienSuBenh = @TienSuBenh,
    diUng = @DiUng
WHERE maHoSo = @MaHoSo
AND maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", hs.MaHoSo);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", hs.MaTaiKhoan);
            lenh.Parameters.AddWithValue("@HoTen", hs.HoTen);
            lenh.Parameters.AddWithValue("@NgaySinh", hs.NgaySinh.Date);
            lenh.Parameters.AddWithValue("@GioiTinh", (object?)hs.GioiTinh ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@ChieuCao", (object?)hs.ChieuCao ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@CanNang", (object?)hs.CanNang ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@TienSuBenh", (object?)hs.TienSuBenh ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@DiUng", (object?)hs.DiUng ?? DBNull.Value);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool CapNhatNgaySinhVaDanhDau(int maHoSo, int maTaiKhoan, DateTime ngaySinhMoi)
        {
            DamBaoCotHoSoSucKhoe();
            const string sql = @"UPDATE HoSoSucKhoe
SET ngaySinh = @NgaySinh,
    birthDateChangedAt = GETDATE(),
    birthDateWarningDismissedAt = NULL
WHERE maHoSo = @MaHoSo
AND maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@NgaySinh", ngaySinhMoi.Date);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool TatCanhBaoDoiNgaySinh(int maHoSo, int maTaiKhoan)
        {
            DamBaoCotHoSoSucKhoe();
            const string sql = @"UPDATE HoSoSucKhoe
SET birthDateWarningDismissedAt = GETDATE()
WHERE maHoSo = @MaHoSo
AND maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool KiemTraHoSoCoCanhBaoDoiNgaySinh(int maHoSo, int maTaiKhoan)
        {
            DamBaoCotHoSoSucKhoe();
            const string sql = @"SELECT CASE
WHEN birthDateChangedAt IS NOT NULL
AND (birthDateWarningDismissedAt IS NULL OR birthDateWarningDismissedAt < birthDateChangedAt)
THEN 1 ELSE 0 END
FROM HoSoSucKhoe
WHERE maHoSo = @MaHoSo
AND maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // Mục đích: phương thức Xoa thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool Xoa(int maHoSo, int maTaiKhoan)
        {
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            ketNoi.Open();
            using var giaoDich = ketNoi.BeginTransaction();

            try
            {
                // Xóa lịch sử tiêm của các lịch tiêm thuộc đúng hồ sơ và đúng tài khoản đang đăng nhập.
                ThucThiLenhXoa(ketNoi, giaoDich, @"DELETE lst
FROM LichSuTiem lst
INNER JOIN LichTiem lt ON lst.maLichTiem = lt.maLichTiem
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
WHERE hs.maHoSo = @MaHoSo
AND hs.maTaiKhoan = @MaTaiKhoan", maHoSo, maTaiKhoan);

                // Xóa thông báo theo đúng khóa maLichTiem hiện có trong bảng ThongBao.
                ThucThiLenhXoa(ketNoi, giaoDich, @"DELETE tb
FROM ThongBao tb
INNER JOIN LichTiem lt ON tb.maLichTiem = lt.maLichTiem
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
WHERE hs.maHoSo = @MaHoSo
AND hs.maTaiKhoan = @MaTaiKhoan", maHoSo, maTaiKhoan);

                // Xóa lịch tiêm sau khi đã xóa các bảng con tham chiếu đến LichTiem.
                ThucThiLenhXoa(ketNoi, giaoDich, @"DELETE lt
FROM LichTiem lt
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
WHERE hs.maHoSo = @MaHoSo
AND hs.maTaiKhoan = @MaTaiKhoan", maHoSo, maTaiKhoan);

                // Cuối cùng mới xóa hồ sơ sức khỏe, vẫn kiểm tra đúng chủ sở hữu bằng maTaiKhoan.
                var soDongHoSoDaXoa = ThucThiLenhXoa(ketNoi, giaoDich, @"DELETE FROM HoSoSucKhoe
WHERE maHoSo = @MaHoSo
AND maTaiKhoan = @MaTaiKhoan", maHoSo, maTaiKhoan);

                giaoDich.Commit();
                return soDongHoSoDaXoa > 0;
            }
            catch
            {
                try
                {
                    giaoDich.Rollback();
                }
                catch
                {
                    // Nếu rollback cũng lỗi thì trả false để Controller hiển thị thông báo thất bại thay vì crash.
                }

                return false;
            }
        }

        // Thực thi một câu DELETE trong cùng transaction và luôn truyền tham số, không nối chuỗi SQL trực tiếp.
        private static int ThucThiLenhXoa(SqlConnection ketNoi, SqlTransaction giaoDich, string sql, int maHoSo, int maTaiKhoan)
        {
            using var lenh = new SqlCommand(sql, ketNoi, giaoDich);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            return lenh.ExecuteNonQuery();
        }

        // Mục đích: phương thức LayHoSoDauTienTheoTaiKhoan thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public HoSoSucKhoe? LayHoSoDauTienTheoTaiKhoan(int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT TOP 1
    maHoSo,
    maTaiKhoan,
    hoTen,
    ngaySinh,
    gioiTinh,
    chieuCao,
    canNang,
    tienSuBenh,
    diUng,
    ngayTao
FROM HoSoSucKhoe
WHERE maTaiKhoan = @MaTaiKhoan
ORDER BY ngayTao ASC";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
                        ketNoi.Open();
                        using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocHoSo(doc) : null;
        }

        // Mục đích: phương thức GanThamSoHoSo thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static void GanThamSoHoSo(SqlCommand lenh, HoSoSucKhoe hs)
        {
            lenh.Parameters.AddWithValue("@MaTaiKhoan", hs.MaTaiKhoan);
            lenh.Parameters.AddWithValue("@HoTen", hs.HoTen);
            lenh.Parameters.AddWithValue("@NgaySinh", hs.NgaySinh.Date);
            lenh.Parameters.AddWithValue("@GioiTinh", (object?)hs.GioiTinh ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@ChieuCao", (object?)hs.ChieuCao ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@CanNang", (object?)hs.CanNang ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@TienSuBenh", (object?)hs.TienSuBenh ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@DiUng", (object?)hs.DiUng ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@NgayTao", hs.NgayTao);
        }

        // Mục đích: phương thức DocHoSo thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static HoSoSucKhoe DocHoSo(SqlDataReader doc)
        {
            return new HoSoSucKhoe
            {
                MaHoSo = Convert.ToInt32(doc["maHoSo"]),
                MaTaiKhoan = Convert.ToInt32(doc["maTaiKhoan"]),
                HoTen = doc["hoTen"] == DBNull.Value ? string.Empty : doc["hoTen"].ToString() ?? string.Empty,
                NgaySinh = Convert.ToDateTime(doc["ngaySinh"]),
                GioiTinh = doc["gioiTinh"] == DBNull.Value ? string.Empty : doc["gioiTinh"].ToString() ?? string.Empty,
                ChieuCao = doc["chieuCao"] == DBNull.Value ? null : Convert.ToDouble(doc["chieuCao"]),
                CanNang = doc["canNang"] == DBNull.Value ? null : Convert.ToDouble(doc["canNang"]),
                TienSuBenh = doc["tienSuBenh"] == DBNull.Value ? string.Empty : doc["tienSuBenh"].ToString() ?? string.Empty,
                DiUng = doc["diUng"] == DBNull.Value ? string.Empty : doc["diUng"].ToString() ?? string.Empty,
                NgayTao = Convert.ToDateTime(doc["ngayTao"])
            };
        }
        private void DamBaoCotHoSoSucKhoe()
        {
            const string sql = @"
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.HoSoSucKhoe') AND name = 'birthDateChangedAt')
    ALTER TABLE dbo.HoSoSucKhoe ADD birthDateChangedAt DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.HoSoSucKhoe') AND name = 'birthDateWarningDismissedAt')
    ALTER TABLE dbo.HoSoSucKhoe ADD birthDateWarningDismissedAt DATETIME NULL;";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }
    }
}
