using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class LichSuTiem_DAL
    {
        private readonly string chuoiKetNoi;

        public LichSuTiem_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        // Mục đích: phương thức Them thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool Them(LichSuTiem lichSu)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"INSERT INTO LichSuTiem(maLichTiem, ngayTiemThucTe, ghiChu, ngayCapNhat)
VALUES(@MaLichTiem, @NgayTiemThucTe, @GhiChu, @NgayCapNhat)";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaLichTiem", lichSu.MaLichTiem);
            lenh.Parameters.AddWithValue("@NgayTiemThucTe", lichSu.NgayTiemThucTe.Date);
            lenh.Parameters.AddWithValue("@GhiChu", string.IsNullOrWhiteSpace(lichSu.GhiChu) ? DBNull.Value : lichSu.GhiChu);
            lenh.Parameters.AddWithValue("@NgayCapNhat", lichSu.NgayCapNhat);

                        ketNoi.Open();
                        return lenh.ExecuteNonQuery() > 0;
        }

        // Mục đích: phương thức KiemTraDaCoLichSu thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool KiemTraDaCoLichSu(int maLichTiem)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = "SELECT COUNT(*) FROM LichSuTiem WHERE maLichTiem = @MaLichTiem";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaLichTiem", maLichTiem);

                        ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        public bool CapNhat(LichSuTiem lichSu)
        {
            const string sql = @"UPDATE LichSuTiem
SET ngayTiemThucTe = @NgayTiemThucTe,
    ghiChu = @GhiChu,
    ngayCapNhat = @NgayCapNhat
WHERE maLichTiem = @MaLichTiem";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaLichTiem", lichSu.MaLichTiem);
            lenh.Parameters.AddWithValue("@NgayTiemThucTe", lichSu.NgayTiemThucTe.Date);
            lenh.Parameters.AddWithValue("@GhiChu", string.IsNullOrWhiteSpace(lichSu.GhiChu) ? DBNull.Value : lichSu.GhiChu);
            lenh.Parameters.AddWithValue("@NgayCapNhat", lichSu.NgayCapNhat);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public LichSuTiem? LayTheoMaLichTiem(int maLichTiem)
        {
            const string sql = @"SELECT TOP 1
    maLichSu, maLichTiem, ngayTiemThucTe, ghiChu, ngayCapNhat
FROM LichSuTiem
WHERE maLichTiem = @MaLichTiem";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaLichTiem", maLichTiem);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            if (!doc.Read()) return null;
            return new LichSuTiem
            {
                MaLichSu = Convert.ToInt32(doc["maLichSu"]),
                MaLichTiem = Convert.ToInt32(doc["maLichTiem"]),
                NgayTiemThucTe = Convert.ToDateTime(doc["ngayTiemThucTe"]),
                GhiChu = doc["ghiChu"] == DBNull.Value ? string.Empty : doc["ghiChu"].ToString() ?? string.Empty,
                NgayCapNhat = Convert.ToDateTime(doc["ngayCapNhat"])
            };
        }

        // Lấy ngày tiêm thực tế của mũi trước trong cùng vaccine (theo số thứ tự mũi)
        public DateTime? LayNgayTiemThucTeMuiTruoc(int maHoSo, int maMuiTiemHienTai)
        {
            const string sql = @"SELECT TOP 1 lst.ngayTiemThucTe
FROM LichSuTiem lst
INNER JOIN LichTiem lt ON lst.maLichTiem = lt.maLichTiem
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
WHERE lt.maHoSo = @MaHoSo
AND mt.maVaccine = (SELECT maVaccine FROM MuiTiemVaccine WHERE maMuiTiem = @MaMuiTiem)
AND mt.soMui < (SELECT soMui FROM MuiTiemVaccine WHERE maMuiTiem = @MaMuiTiem)
ORDER BY mt.soMui DESC";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiemHienTai);
            ketNoi.Open();
            var result = lenh.ExecuteScalar();
            return result == null || result == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(result);
        }

        // Lấy ngày tiêm thực tế của mũi sau trong cùng vaccine (theo số thứ tự mũi)
        public DateTime? LayNgayTiemThucTeMuiSau(int maHoSo, int maMuiTiemHienTai)
        {
            const string sql = @"SELECT TOP 1 lst.ngayTiemThucTe
FROM LichSuTiem lst
INNER JOIN LichTiem lt ON lst.maLichTiem = lt.maLichTiem
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
WHERE lt.maHoSo = @MaHoSo
AND mt.maVaccine = (SELECT maVaccine FROM MuiTiemVaccine WHERE maMuiTiem = @MaMuiTiem)
AND mt.soMui > (SELECT soMui FROM MuiTiemVaccine WHERE maMuiTiem = @MaMuiTiem)
ORDER BY mt.soMui ASC";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiemHienTai);
            ketNoi.Open();
            var result = lenh.ExecuteScalar();
            return result == null || result == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(result);
        }

        // Lấy tất cả ngày tiêm thực tế của các lịch tiêm thuộc một hồ sơ
        public Dictionary<int, DateTime> LayNgayTiemThucTeTheoHoSo(int maHoSo)
        {
            var result = new Dictionary<int, DateTime>();
            const string sql = @"SELECT lst.maLichTiem, lst.ngayTiemThucTe
FROM LichSuTiem lst
INNER JOIN LichTiem lt ON lst.maLichTiem = lt.maLichTiem
WHERE lt.maHoSo = @MaHoSo";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            while (doc.Read())
            {
                var maLichTiem = Convert.ToInt32(doc["maLichTiem"]);
                var ngayTiem = Convert.ToDateTime(doc["ngayTiemThucTe"]);
                result[maLichTiem] = ngayTiem;
            }
            return result;
        }

        // Mục đích: phương thức LayDanhSachTheoHoSo thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<LichSuTiem> LayDanhSachTheoHoSo(int maHoSo, int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"SELECT
    lst.maLichSu,
    lst.maLichTiem,
    lst.ngayTiemThucTe,
    lst.ghiChu,
    lst.ngayCapNhat,
    lt.maHoSo,
    lt.ngayTiemDuKien,
    hs.hoTen AS hoTenHoSo,
    v.tenVaccine,
    mt.tenMui,
    mt.soMui
FROM LichSuTiem lst
INNER JOIN LichTiem lt ON lst.maLichTiem = lt.maLichTiem
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE lt.maHoSo = @MaHoSo
AND hs.maTaiKhoan = @MaTaiKhoan
ORDER BY lst.ngayTiemThucTe DESC, v.tenVaccine, mt.soMui";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

                        ketNoi.Open();
                        using var doc = lenh.ExecuteReader();

            var danhSach = new List<LichSuTiem>();
            while (doc.Read())
            {
                danhSach.Add(new LichSuTiem
                {
                    MaLichSu = Convert.ToInt32(doc["maLichSu"]),
                    MaLichTiem = Convert.ToInt32(doc["maLichTiem"]),
                    NgayTiemThucTe = Convert.ToDateTime(doc["ngayTiemThucTe"]),
                    GhiChu = doc["ghiChu"] == DBNull.Value ? string.Empty : doc["ghiChu"].ToString() ?? string.Empty,
                    NgayCapNhat = Convert.ToDateTime(doc["ngayCapNhat"]),
                    MaHoSo = Convert.ToInt32(doc["maHoSo"]),
                    NgayTiemDuKien = Convert.ToDateTime(doc["ngayTiemDuKien"]),
                    HoTenHoSo = doc["hoTenHoSo"] == DBNull.Value ? string.Empty : doc["hoTenHoSo"].ToString() ?? string.Empty,
                    TenVaccine = doc["tenVaccine"] == DBNull.Value ? string.Empty : doc["tenVaccine"].ToString() ?? string.Empty,
                    TenMui = doc["tenMui"] == DBNull.Value ? string.Empty : doc["tenMui"].ToString() ?? string.Empty,
                    SoMui = Convert.ToInt32(doc["soMui"])
                });
            }

            return danhSach;
        }
    }
}
