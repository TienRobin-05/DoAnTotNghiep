using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp MuiTiemVaccine_DAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
    public class MuiTiemVaccine_DAL
    {
        private readonly string chuoiKetNoi;

        public MuiTiemVaccine_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        // Mục đích: phương thức LayDanhSach thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<MuiTiemVaccine> LayDanhSach()
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT
    mt.maMuiTiem,
    mt.maVaccine,
    mt.soMui,
    mt.tenMui,
    mt.doTuoiToiThieu,
    mt.doTuoiToiDa,
    mt.doTuoiKhuyenNghi,
    mt.donViTuoi,
    mt.khoangCachNgay,
    mt.ghiChu,
    v.tenVaccine
FROM MuiTiemVaccine mt
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
ORDER BY v.tenVaccine, mt.soMui";

            return DocDanhSach(sql);
        }

        // Hàm đặt tên theo nghiệp vụ: lấy toàn bộ mũi tiêm kèm tên vaccine để Admin quản lý dữ liệu cố định.
        public List<MuiTiemVaccine> LayTatCaMuiTiemKemVaccine()
        {
            return LayDanhSach();
        }

        // Hàm đặt tên theo nghiệp vụ: lấy mũi tiêm của một vaccine cụ thể.
        public List<MuiTiemVaccine> LayMuiTiemTheoVaccine(int maVaccine)
        {
            return LayDanhSachTheoVaccine(maVaccine);
        }

        // Mục đích: phương thức LayDanhSachTheoVaccine thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<MuiTiemVaccine> LayDanhSachTheoVaccine(int maVaccine)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT
    mt.maMuiTiem,
    mt.maVaccine,
    mt.soMui,
    mt.tenMui,
    mt.doTuoiToiThieu,
    mt.doTuoiToiDa,
    mt.doTuoiKhuyenNghi,
    mt.donViTuoi,
    mt.khoangCachNgay,
    mt.ghiChu,
    v.tenVaccine
FROM MuiTiemVaccine mt
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE mt.maVaccine = @MaVaccine
ORDER BY mt.soMui";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaVaccine", maVaccine);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            return DocDanhSachTuReader(doc);
        }

        // Mục đích: phương thức LayTheoId thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public MuiTiemVaccine? LayTheoId(int maMuiTiem)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT
    mt.maMuiTiem,
    mt.maVaccine,
    mt.soMui,
    mt.tenMui,
    mt.doTuoiToiThieu,
    mt.doTuoiToiDa,
    mt.doTuoiKhuyenNghi,
    mt.donViTuoi,
    mt.khoangCachNgay,
    mt.ghiChu,
    v.tenVaccine
FROM MuiTiemVaccine mt
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE mt.maMuiTiem = @MaMuiTiem";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocMuiTiem(doc) : null;
        }

        // Mục đích: phương thức Them thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool Them(MuiTiemVaccine mt)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"INSERT INTO MuiTiemVaccine
(
    maVaccine,
    soMui,
    tenMui,
    doTuoiToiThieu,
    doTuoiToiDa,
    doTuoiKhuyenNghi,
    donViTuoi,
    khoangCachNgay,
    ghiChu
)
VALUES
(
    @MaVaccine,
    @SoMui,
    @TenMui,
    @DoTuoiToiThieu,
    @DoTuoiToiDa,
    @DoTuoiKhuyenNghi,
    @DonViTuoi,
    @KhoangCachNgay,
    @GhiChu
)";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSo(lenh, mt);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteNonQuery trả về số dòng bị ảnh hưởng; lớn hơn 0 nghĩa là thêm/sửa/xóa thành công.
            return lenh.ExecuteNonQuery() > 0;
        }

        // Hàm đặt tên theo nghiệp vụ: thêm mũi tiêm cố định cho vaccine.
        public bool ThemMuiTiemVaccine(MuiTiemVaccine mt)
        {
            return Them(mt);
        }

        // Mục đích: phương thức CapNhat thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool CapNhat(MuiTiemVaccine mt)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"UPDATE MuiTiemVaccine
SET maVaccine = @MaVaccine,
    soMui = @SoMui,
    tenMui = @TenMui,
    doTuoiToiThieu = @DoTuoiToiThieu,
    doTuoiToiDa = @DoTuoiToiDa,
    doTuoiKhuyenNghi = @DoTuoiKhuyenNghi,
    donViTuoi = @DonViTuoi,
    khoangCachNgay = @KhoangCachNgay,
    ghiChu = @GhiChu
WHERE maMuiTiem = @MaMuiTiem";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaMuiTiem", mt.MaMuiTiem);
            GanThamSo(lenh, mt);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteNonQuery trả về số dòng bị ảnh hưởng; lớn hơn 0 nghĩa là thêm/sửa/xóa thành công.
            return lenh.ExecuteNonQuery() > 0;
        }

        // Hàm đặt tên theo nghiệp vụ: sửa thông tin mũi tiêm cố định của vaccine.
        public bool SuaMuiTiemVaccine(MuiTiemVaccine mt)
        {
            return CapNhat(mt);
        }

        // Mục đích: phương thức Xoa thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool Xoa(int maMuiTiem)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = "DELETE FROM MuiTiemVaccine WHERE maMuiTiem = @MaMuiTiem";
            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteNonQuery trả về số dòng bị ảnh hưởng; lớn hơn 0 nghĩa là thêm/sửa/xóa thành công.
            return lenh.ExecuteNonQuery() > 0;
        }

        // Hàm đặt tên theo nghiệp vụ: xóa mũi tiêm cố định khỏi bảng MuiTiemVaccine.
        public bool XoaMuiTiemVaccine(int maMuiTiem)
        {
            return Xoa(maMuiTiem);
        }

        // Mục đích: phương thức KiemTraTrungSoMui thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool KiemTraTrungSoMui(int maVaccine, int soMui, int? maMuiTiemBoQua)
        {
            var sql = @"SELECT COUNT(*)
FROM MuiTiemVaccine
WHERE maVaccine = @MaVaccine
AND soMui = @SoMui";

            if (maMuiTiemBoQua.HasValue)
            {
                sql += " AND maMuiTiem <> @MaMuiTiemBoQua";
            }

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaVaccine", maVaccine);
            lenh.Parameters.AddWithValue("@SoMui", soMui);
            if (maMuiTiemBoQua.HasValue)
            {
                lenh.Parameters.AddWithValue("@MaMuiTiemBoQua", maMuiTiemBoQua.Value);
            }

            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // Mục đích: phương thức DocDanhSach thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private List<MuiTiemVaccine> DocDanhSach(string sql)
        {
            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            return DocDanhSachTuReader(doc);
        }

        // Mục đích: phương thức DocDanhSachTuReader thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static List<MuiTiemVaccine> DocDanhSachTuReader(SqlDataReader doc)
        {
            var danhSach = new List<MuiTiemVaccine>();
            while (doc.Read())
            {
                danhSach.Add(DocMuiTiem(doc));
            }

            return danhSach;
        }

        // Mục đích: phương thức GanThamSo thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static void GanThamSo(SqlCommand lenh, MuiTiemVaccine mt)
        {
            lenh.Parameters.AddWithValue("@MaVaccine", mt.MaVaccine);
            lenh.Parameters.AddWithValue("@SoMui", mt.SoMui);
            lenh.Parameters.AddWithValue("@TenMui", string.IsNullOrWhiteSpace(mt.TenMui) ? DBNull.Value : mt.TenMui);
            lenh.Parameters.AddWithValue("@DoTuoiToiThieu", (object?)mt.DoTuoiToiThieu ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@DoTuoiToiDa", (object?)mt.DoTuoiToiDa ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@DoTuoiKhuyenNghi", (object?)mt.DoTuoiKhuyenNghi ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@DonViTuoi", string.IsNullOrWhiteSpace(mt.DonViTuoi) ? DBNull.Value : mt.DonViTuoi);
            lenh.Parameters.AddWithValue("@KhoangCachNgay", (object?)mt.KhoangCachNgay ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@GhiChu", string.IsNullOrWhiteSpace(mt.GhiChu) ? DBNull.Value : mt.GhiChu);
        }

        // Mục đích: phương thức DocMuiTiem thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static MuiTiemVaccine DocMuiTiem(SqlDataReader doc)
        {
            return new MuiTiemVaccine
            {
                MaMuiTiem = Convert.ToInt32(doc["maMuiTiem"]),
                MaVaccine = Convert.ToInt32(doc["maVaccine"]),
                SoMui = Convert.ToInt32(doc["soMui"]),
                TenMui = doc["tenMui"] == DBNull.Value ? string.Empty : doc["tenMui"].ToString() ?? string.Empty,
                DoTuoiToiThieu = doc["doTuoiToiThieu"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiThieu"]),
                DoTuoiToiDa = doc["doTuoiToiDa"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiDa"]),
                DoTuoiKhuyenNghi = doc["doTuoiKhuyenNghi"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiKhuyenNghi"]),
                DonViTuoi = doc["donViTuoi"] == DBNull.Value ? string.Empty : doc["donViTuoi"].ToString() ?? string.Empty,
                KhoangCachNgay = doc["khoangCachNgay"] == DBNull.Value ? null : Convert.ToInt32(doc["khoangCachNgay"]),
                GhiChu = doc["ghiChu"] == DBNull.Value ? string.Empty : doc["ghiChu"].ToString() ?? string.Empty,
                TenVaccine = doc["tenVaccine"] == DBNull.Value ? string.Empty : doc["tenVaccine"].ToString() ?? string.Empty
            };
        }
    }
}
