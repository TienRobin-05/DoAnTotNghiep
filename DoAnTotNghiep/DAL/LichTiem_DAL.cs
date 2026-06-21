using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp LichTiem_DAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
    public class LichTiem_DAL
    {
        private readonly string chuoiKetNoi;

        public LichTiem_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        // Mục đích: phương thức LayDanhSachTheoHoSo thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<LichTiem> LayDanhSachTheoHoSo(int maHoSo, int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT lt.maLichTiem, lt.maHoSo, lt.maMuiTiem, lt.ngayTiemDuKien, lt.trangThai, lt.ghiChu,
hs.hoTen AS hoTenHoSo, v.tenVaccine, v.nhomVaccine, mt.tenMui, mt.soMui
FROM LichTiem lt
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE lt.maHoSo = @MaHoSo AND hs.maTaiKhoan = @MaTaiKhoan
ORDER BY lt.ngayTiemDuKien, v.tenVaccine, mt.soMui";

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
            var danhSach = new List<LichTiem>();
            while (doc.Read())
            {
                danhSach.Add(DocLichTiem(doc));
            }
            return danhSach;
        }

        // Lấy tất cả lịch tiêm của một tài khoản trong một lần truy vấn, tránh gọi database lặp theo từng hồ sơ.
        public List<LichTiem> LayDanhSachTheoTaiKhoan(int maTaiKhoan)
        {
            const string sql = @"SELECT lt.maLichTiem, lt.maHoSo, lt.maMuiTiem, lt.ngayTiemDuKien, lt.trangThai, lt.ghiChu,
hs.hoTen AS hoTenHoSo, v.tenVaccine, v.nhomVaccine, mt.tenMui, mt.soMui
FROM LichTiem lt
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE hs.maTaiKhoan = @MaTaiKhoan
ORDER BY lt.ngayTiemDuKien, v.tenVaccine, mt.soMui";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<LichTiem>();
            while (doc.Read())
            {
                danhSach.Add(DocLichTiem(doc));
            }

            return danhSach;
        }

        public int DemTatCa()
        {
            const string sql = "SELECT COUNT(*) FROM LichTiem";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }

        // Mục đích: phương thức KiemTraHoSoCoLichTiem thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool KiemTraHoSoCoLichTiem(int maHoSo)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = "SELECT COUNT(*) FROM LichTiem WHERE maHoSo = @MaHoSo";
            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // Mục đích: phương thức Them thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool Them(LichTiem lich)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"INSERT INTO LichTiem(maHoSo, maMuiTiem, ngayTiemDuKien, trangThai, ghiChu)
VALUES(@MaHoSo, @MaMuiTiem, @NgayTiemDuKien, @TrangThai, @GhiChu)";
            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", lich.MaHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", lich.MaMuiTiem);
            lenh.Parameters.AddWithValue("@NgayTiemDuKien", lich.NgayTiemDuKien.Date);
            lenh.Parameters.AddWithValue("@TrangThai", lich.TrangThai);
            lenh.Parameters.AddWithValue("@GhiChu", string.IsNullOrWhiteSpace(lich.GhiChu) ? DBNull.Value : lich.GhiChu);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteNonQuery trả về số dòng bị ảnh hưởng; lớn hơn 0 nghĩa là thêm/sửa/xóa thành công.
            return lenh.ExecuteNonQuery() > 0;
        }

        // Mục đích: phương thức LayTheoId thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public LichTiem? LayTheoId(int maLichTiem)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT lt.maLichTiem, lt.maHoSo, lt.maMuiTiem, lt.ngayTiemDuKien, lt.trangThai, lt.ghiChu,
hs.hoTen AS hoTenHoSo, v.tenVaccine, v.nhomVaccine, mt.tenMui, mt.soMui
FROM LichTiem lt
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE lt.maLichTiem = @MaLichTiem";
            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaLichTiem", maLichTiem);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocLichTiem(doc) : null;
        }

        // Mục đích: phương thức LayChiTietCoKiemTraChuSoHuu thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public LichTiem? LayChiTietCoKiemTraChuSoHuu(int maLichTiem, int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT lt.maLichTiem, lt.maHoSo, lt.maMuiTiem, lt.ngayTiemDuKien, lt.trangThai, lt.ghiChu,
hs.hoTen AS hoTenHoSo, hs.ngaySinh AS ngaySinhHoSo, v.tenVaccine, v.nhomVaccine, mt.tenMui, mt.soMui
FROM LichTiem lt
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE lt.maLichTiem = @MaLichTiem
AND hs.maTaiKhoan = @MaTaiKhoan";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocLichTiem(doc) : null;
        }

        // Mục đích: phương thức CapNhatDaTiem thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool CapNhatDaTiem(int maLichTiem)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = "UPDATE LichTiem SET trangThai = @TrangThai WHERE maLichTiem = @MaLichTiem";

            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@TrangThai", "Đã tiêm");

            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteNonQuery trả về số dòng bị ảnh hưởng; lớn hơn 0 nghĩa là thêm/sửa/xóa thành công.
            return lenh.ExecuteNonQuery() > 0;
        }

        // Mục đích: phương thức CapNhatTrangThai thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool CapNhatTrangThai(int maLichTiem, string trangThai)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = "UPDATE LichTiem SET trangThai = @TrangThai WHERE maLichTiem = @MaLichTiem";
            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@TrangThai", trangThai);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteNonQuery trả về số dòng bị ảnh hưởng; lớn hơn 0 nghĩa là thêm/sửa/xóa thành công.
            return lenh.ExecuteNonQuery() > 0;
        }

        // Mục đích: phương thức KiemTraLichTonTai thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool KiemTraLichTonTai(int maHoSo, int maMuiTiem)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = "SELECT COUNT(*) FROM LichTiem WHERE maHoSo = @MaHoSo AND maMuiTiem = @MaMuiTiem";
            // Tạo kết nối đến SQL Server bằng chuỗi kết nối đã lấy từ appsettings.json.
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // Mục đích: phương thức DocLichTiem thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static LichTiem DocLichTiem(SqlDataReader doc)
        {
            return new LichTiem
            {
                MaLichTiem = Convert.ToInt32(doc["maLichTiem"]),
                MaHoSo = Convert.ToInt32(doc["maHoSo"]),
                MaMuiTiem = Convert.ToInt32(doc["maMuiTiem"]),
                NgayTiemDuKien = Convert.ToDateTime(doc["ngayTiemDuKien"]),
                TrangThai = doc["trangThai"] == DBNull.Value ? string.Empty : doc["trangThai"].ToString() ?? string.Empty,
                GhiChu = ChuanHoaGhiChuHienThi(doc["ghiChu"] == DBNull.Value ? string.Empty : doc["ghiChu"].ToString() ?? string.Empty),
                HoTenHoSo = doc["hoTenHoSo"] == DBNull.Value ? string.Empty : doc["hoTenHoSo"].ToString() ?? string.Empty,
                TenVaccine = doc["tenVaccine"] == DBNull.Value ? string.Empty : doc["tenVaccine"].ToString() ?? string.Empty,
                NhomVaccine = CoCot(doc, "nhomVaccine") && doc["nhomVaccine"] != DBNull.Value ? doc["nhomVaccine"].ToString() ?? string.Empty : string.Empty,
                TenMui = doc["tenMui"] == DBNull.Value ? string.Empty : doc["tenMui"].ToString() ?? string.Empty,
                SoMui = Convert.ToInt32(doc["soMui"]),
                NgaySinhHoSo = CoCot(doc, "ngaySinhHoSo") && doc["ngaySinhHoSo"] != DBNull.Value
                    ? Convert.ToDateTime(doc["ngaySinhHoSo"])
                    : DateTime.MinValue
            };
        }

        // Mục đích: phương thức CoCot thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
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

        // Chuẩn hóa ghi chú cũ/null để người dùng không thấy nội dung kỹ thuật khi xem lịch tiêm.
        private static string ChuanHoaGhiChuHienThi(string ghiChu)
        {
            if (string.IsNullOrWhiteSpace(ghiChu))
            {
                return "Theo lịch tiêm khuyến nghị";
            }

            var noiDungCu = ghiChu.Trim();
            if (string.Equals(noiDungCu, "Tự động tạo lịch tiêm", StringComparison.OrdinalIgnoreCase)
                || string.Equals(noiDungCu, "Tự động tạo lịch tiêm theo độ tuổi", StringComparison.OrdinalIgnoreCase))
            {
                return "Mũi tiêm được khuyến nghị theo độ tuổi";
            }

            return ghiChu;
        }
    }
}
