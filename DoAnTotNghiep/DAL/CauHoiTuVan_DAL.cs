using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp CauHoiTuVan_DAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
    public class CauHoiTuVan_DAL
    {
        private readonly string chuoiKetNoi;

        public CauHoiTuVan_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        // Mục đích: phương thức LayDanhSachTheoNguoiGui thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<CauHoiTuVan> LayDanhSachTheoNguoiGui(int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT
    ch.maCauHoi,
    ch.maNguoiGui,
    ch.maNguoiTraLoi,
    ch.cauHoi,
    ch.cauTraLoi,
    ch.ngayGui,
    ch.ngayTraLoi,
    ch.trangThai,
    gui.hoTen AS tenNguoiGui,
    traLoi.hoTen AS tenNguoiTraLoi
FROM CauHoiTuVan ch
INNER JOIN TaiKhoan gui ON ch.maNguoiGui = gui.maTaiKhoan
LEFT JOIN TaiKhoan traLoi ON ch.maNguoiTraLoi = traLoi.maTaiKhoan
WHERE ch.maNguoiGui = @MaTaiKhoan
ORDER BY ch.ngayGui DESC";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<CauHoiTuVan>();
            while (doc.Read())
            {
                danhSach.Add(DocCauHoi(doc));
            }

            return danhSach;
        }

        // Mục đích: phương thức LayTheoIdCuaNguoiGui thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public CauHoiTuVan? LayTheoIdCuaNguoiGui(int maCauHoi, int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT
    ch.maCauHoi,
    ch.maNguoiGui,
    ch.maNguoiTraLoi,
    ch.cauHoi,
    ch.cauTraLoi,
    ch.ngayGui,
    ch.ngayTraLoi,
    ch.trangThai,
    gui.hoTen AS tenNguoiGui,
    traLoi.hoTen AS tenNguoiTraLoi
FROM CauHoiTuVan ch
INNER JOIN TaiKhoan gui ON ch.maNguoiGui = gui.maTaiKhoan
LEFT JOIN TaiKhoan traLoi ON ch.maNguoiTraLoi = traLoi.maTaiKhoan
WHERE ch.maCauHoi = @MaCauHoi
AND ch.maNguoiGui = @MaTaiKhoan";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaCauHoi", maCauHoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocCauHoi(doc) : null;
        }

        // Mục đích: phương thức GuiCauHoi thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool GuiCauHoi(CauHoiTuVan ch)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"INSERT INTO CauHoiTuVan
(
    maNguoiGui,
    maNguoiTraLoi,
    cauHoi,
    cauTraLoi,
    ngayGui,
    ngayTraLoi,
    trangThai
)
VALUES
(
    @MaNguoiGui,
    @MaNguoiTraLoi,
    @CauHoi,
    @CauTraLoi,
    @NgayGui,
    @NgayTraLoi,
    @TrangThai
)";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaNguoiGui", ch.MaNguoiGui);
            lenh.Parameters.AddWithValue("@MaNguoiTraLoi", DBNull.Value);
            lenh.Parameters.AddWithValue("@CauHoi", ch.CauHoi);
            lenh.Parameters.AddWithValue("@CauTraLoi", DBNull.Value);
            lenh.Parameters.AddWithValue("@NgayGui", ch.NgayGui);
            lenh.Parameters.AddWithValue("@NgayTraLoi", DBNull.Value);
            lenh.Parameters.AddWithValue("@TrangThai", ch.TrangThai);

            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteNonQuery trả về số dòng bị ảnh hưởng; lớn hơn 0 nghĩa là thêm/sửa/xóa thành công.
            return lenh.ExecuteNonQuery() > 0;
        }

        // Mục đích: phương thức DocCauHoi thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static CauHoiTuVan DocCauHoi(SqlDataReader doc)
        {
            return new CauHoiTuVan
            {
                MaCauHoi = Convert.ToInt32(doc["maCauHoi"]),
                MaNguoiGui = Convert.ToInt32(doc["maNguoiGui"]),
                MaNguoiTraLoi = doc["maNguoiTraLoi"] == DBNull.Value ? null : Convert.ToInt32(doc["maNguoiTraLoi"]),
                CauHoi = doc["cauHoi"] == DBNull.Value ? string.Empty : doc["cauHoi"].ToString() ?? string.Empty,
                CauTraLoi = doc["cauTraLoi"] == DBNull.Value ? string.Empty : doc["cauTraLoi"].ToString() ?? string.Empty,
                NgayGui = Convert.ToDateTime(doc["ngayGui"]),
                NgayTraLoi = doc["ngayTraLoi"] == DBNull.Value ? null : Convert.ToDateTime(doc["ngayTraLoi"]),
                TrangThai = doc["trangThai"] == DBNull.Value ? string.Empty : doc["trangThai"].ToString() ?? string.Empty,
                TenNguoiGui = doc["tenNguoiGui"] == DBNull.Value ? string.Empty : doc["tenNguoiGui"].ToString() ?? string.Empty,
                TenNguoiTraLoi = doc["tenNguoiTraLoi"] == DBNull.Value ? string.Empty : doc["tenNguoiTraLoi"].ToString() ?? string.Empty
            };
        }
    }
}
