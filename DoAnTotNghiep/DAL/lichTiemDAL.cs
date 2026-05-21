using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp lichTiemDAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
    public class lichTiemDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public lichTiemDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        // Mục đích: phương thức layTheoTaiKhoan thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<lichTiemModels> layTheoTaiKhoan(int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select lt.maLichTiem, lt.maHoSo, lt.maMuiTiem, lt.ngayTiemDuKien, lt.trangThai, lt.ghiChu,
hs.hoTen as tenHoSo, v.tenVaccine, mt.tenMui, mt.soMui
from LichTiem lt
inner join HoSoSucKhoe hs on lt.maHoSo = hs.maHoSo
inner join MuiTiemVaccine mt on lt.maMuiTiem = mt.maMuiTiem
inner join Vaccine v on mt.maVaccine = v.maVaccine
where hs.maTaiKhoan = @maTaiKhoan
order by lt.ngayTiemDuKien";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<lichTiemModels>();
            while (doc.Read())
            {
                danhSach.Add(docLichTiem(doc));
            }
            return danhSach;
        }

        // Mục đích: phương thức layTatCa thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<lichTiemModels> layTatCa()
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select lt.maLichTiem, lt.maHoSo, lt.maMuiTiem, lt.ngayTiemDuKien, lt.trangThai, lt.ghiChu,
hs.hoTen as tenHoSo, v.tenVaccine, mt.tenMui, mt.soMui
from LichTiem lt
inner join HoSoSucKhoe hs on lt.maHoSo = hs.maHoSo
inner join MuiTiemVaccine mt on lt.maMuiTiem = mt.maMuiTiem
inner join Vaccine v on mt.maVaccine = v.maVaccine
order by lt.ngayTiemDuKien desc";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<lichTiemModels>();
            while (doc.Read())
            {
                danhSach.Add(docLichTiem(doc));
            }
            return danhSach;
        }

        // Mục đích: phương thức lichThuocTaiKhoan thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool lichThuocTaiKhoan(int maLichTiem, int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select count(1)
from LichTiem lt inner join HoSoSucKhoe hs on lt.maHoSo = hs.maHoSo
where lt.maLichTiem = @maLichTiem and hs.maTaiKhoan = @maTaiKhoan";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // Mục đích: phương thức them thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public void them(lichTiemModels lichTiem)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"insert into LichTiem(maHoSo, maMuiTiem, ngayTiemDuKien, trangThai, ghiChu)
values(@maHoSo, @maMuiTiem, @ngayTiemDuKien, @trangThai, @ghiChu)";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSo(lenh, lichTiem);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // Mục đích: phương thức capNhatTrangThai thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public void capNhatTrangThai(int maLichTiem, string trangThai)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = "update LichTiem set trangThai = @trangThai where maLichTiem = @maLichTiem";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@trangThai", trangThai);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // Mục đích: phương thức ganThamSo thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static void ganThamSo(SqlCommand lenh, lichTiemModels lichTiem)
        {
            lenh.Parameters.AddWithValue("@maHoSo", lichTiem.maHoSo);
            lenh.Parameters.AddWithValue("@maMuiTiem", lichTiem.maMuiTiem);
            lenh.Parameters.AddWithValue("@ngayTiemDuKien", lichTiem.ngayTiemDuKien.Date);
            lenh.Parameters.AddWithValue("@trangThai", lichTiem.trangThai);
            lenh.Parameters.AddWithValue("@ghiChu", (object?)lichTiem.ghiChu ?? DBNull.Value);
        }

        // Mục đích: phương thức docLichTiem thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static lichTiemModels docLichTiem(SqlDataReader doc)
        {
            return new lichTiemModels
            {
                maLichTiem = Convert.ToInt32(doc["maLichTiem"]),
                maHoSo = Convert.ToInt32(doc["maHoSo"]),
                maMuiTiem = Convert.ToInt32(doc["maMuiTiem"]),
                ngayTiemDuKien = Convert.ToDateTime(doc["ngayTiemDuKien"]),
                trangThai = doc["trangThai"].ToString() ?? string.Empty,
                ghiChu = doc["ghiChu"] == DBNull.Value ? null : doc["ghiChu"].ToString(),
                tenHoSo = doc["tenHoSo"].ToString(),
                tenVaccine = doc["tenVaccine"].ToString(),
                tenMui = doc["tenMui"] == DBNull.Value ? null : doc["tenMui"].ToString(),
                soMui = Convert.ToInt32(doc["soMui"])
            };
        }
    }
}
