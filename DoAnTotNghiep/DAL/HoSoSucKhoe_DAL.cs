using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp HoSoSucKhoe_DAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
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
            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
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

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
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

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
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

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
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

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSoHoSo(lenh, hs);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteNonQuery trả về số dòng bị ảnh hưởng; lớn hơn 0 nghĩa là thêm/sửa/xóa thành công.
            return lenh.ExecuteNonQuery() > 0;
        }

        // Mục đích: phương thức CapNhat thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool CapNhat(HoSoSucKhoe hs)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
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

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
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
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteNonQuery trả về số dòng bị ảnh hưởng; lớn hơn 0 nghĩa là thêm/sửa/xóa thành công.
            return lenh.ExecuteNonQuery() > 0;
        }

        // Mục đích: phương thức Xoa thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool Xoa(int maHoSo, int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"DELETE FROM HoSoSucKhoe
WHERE maHoSo = @MaHoSo
AND maTaiKhoan = @MaTaiKhoan";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteNonQuery trả về số dòng bị ảnh hưởng; lớn hơn 0 nghĩa là thêm/sửa/xóa thành công.
            return lenh.ExecuteNonQuery() > 0;
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

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
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
    }
}
