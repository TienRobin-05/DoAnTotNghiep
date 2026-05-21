using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp lichSuTiemDAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
    public class lichSuTiemDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public lichSuTiemDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        // Mục đích: phương thức layTheoTaiKhoan thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<lichSuTiemModels> layTheoTaiKhoan(int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select lst.maLichSu, lst.maLichTiem, lst.ngayTiemThucTe, lst.ghiChu, lst.ngayCapNhat,
hs.hoTen as tenHoSo, v.tenVaccine, mt.tenMui
from LichSuTiem lst
inner join LichTiem lt on lst.maLichTiem = lt.maLichTiem
inner join HoSoSucKhoe hs on lt.maHoSo = hs.maHoSo
inner join MuiTiemVaccine mt on lt.maMuiTiem = mt.maMuiTiem
inner join Vaccine v on mt.maVaccine = v.maVaccine
where hs.maTaiKhoan = @maTaiKhoan
order by lst.ngayTiemThucTe desc";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<lichSuTiemModels>();
            while (doc.Read())
            {
                danhSach.Add(new lichSuTiemModels
                {
                    maLichSu = Convert.ToInt32(doc["maLichSu"]),
                    maLichTiem = Convert.ToInt32(doc["maLichTiem"]),
                    ngayTiemThucTe = Convert.ToDateTime(doc["ngayTiemThucTe"]),
                    ghiChu = doc["ghiChu"] == DBNull.Value ? null : doc["ghiChu"].ToString(),
                    ngayCapNhat = Convert.ToDateTime(doc["ngayCapNhat"]),
                    tenHoSo = doc["tenHoSo"].ToString(),
                    tenVaccine = doc["tenVaccine"].ToString(),
                    tenMui = doc["tenMui"] == DBNull.Value ? null : doc["tenMui"].ToString()
                });
            }
            return danhSach;
        }

        // Mục đích: phương thức them thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public void them(lichSuTiemModels lichSu)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"insert into LichSuTiem(maLichTiem, ngayTiemThucTe, ghiChu, ngayCapNhat)
values(@maLichTiem, @ngayTiemThucTe, @ghiChu, @ngayCapNhat)";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maLichTiem", lichSu.maLichTiem);
            lenh.Parameters.AddWithValue("@ngayTiemThucTe", lichSu.ngayTiemThucTe.Date);
            lenh.Parameters.AddWithValue("@ghiChu", (object?)lichSu.ghiChu ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@ngayCapNhat", DateTime.Now);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }
    }
}
