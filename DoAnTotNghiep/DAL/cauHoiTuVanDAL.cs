using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp cauHoiTuVanDAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
    public class cauHoiTuVanDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public cauHoiTuVanDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        // Mục đích: phương thức layTatCa thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<cauHoiTuVanModels> layTatCa()
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select ch.maCauHoi, ch.maNguoiGui, ch.maNguoiTraLoi, ch.cauHoi, ch.cauTraLoi,
ch.ngayGui, ch.ngayTraLoi, ch.trangThai, gui.hoTen as tenNguoiGui, traLoi.hoTen as tenNguoiTraLoi
from CauHoiTuVan ch
inner join TaiKhoan gui on ch.maNguoiGui = gui.maTaiKhoan
left join TaiKhoan traLoi on ch.maNguoiTraLoi = traLoi.maTaiKhoan
order by ch.ngayGui desc";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<cauHoiTuVanModels>();
            while (doc.Read())
            {
                danhSach.Add(docCauHoi(doc));
            }
            return danhSach;
        }

        // Mục đích: phương thức layTheoNguoiGui thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<cauHoiTuVanModels> layTheoNguoiGui(int maNguoiGui)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select ch.maCauHoi, ch.maNguoiGui, ch.maNguoiTraLoi, ch.cauHoi, ch.cauTraLoi,
ch.ngayGui, ch.ngayTraLoi, ch.trangThai, gui.hoTen as tenNguoiGui, traLoi.hoTen as tenNguoiTraLoi
from CauHoiTuVan ch
inner join TaiKhoan gui on ch.maNguoiGui = gui.maTaiKhoan
left join TaiKhoan traLoi on ch.maNguoiTraLoi = traLoi.maTaiKhoan
where ch.maNguoiGui = @maNguoiGui
order by ch.ngayGui desc";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maNguoiGui", maNguoiGui);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<cauHoiTuVanModels>();
            while (doc.Read())
            {
                danhSach.Add(docCauHoi(doc));
            }
            return danhSach;
        }

        // Mục đích: phương thức layTheoMa thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public cauHoiTuVanModels? layTheoMa(int maCauHoi)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select ch.maCauHoi, ch.maNguoiGui, ch.maNguoiTraLoi, ch.cauHoi, ch.cauTraLoi,
ch.ngayGui, ch.ngayTraLoi, ch.trangThai, gui.hoTen as tenNguoiGui, traLoi.hoTen as tenNguoiTraLoi
from CauHoiTuVan ch
inner join TaiKhoan gui on ch.maNguoiGui = gui.maTaiKhoan
left join TaiKhoan traLoi on ch.maNguoiTraLoi = traLoi.maTaiKhoan
where ch.maCauHoi = @maCauHoi";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maCauHoi", maCauHoi);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? docCauHoi(doc) : null;
        }

        // Mục đích: phương thức them thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public void them(cauHoiTuVanModels cauHoi)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"insert into CauHoiTuVan(maNguoiGui, maNguoiTraLoi, cauHoi, cauTraLoi, ngayGui, ngayTraLoi, trangThai)
values(@maNguoiGui, null, @cauHoi, null, @ngayGui, null, @trangThai)";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maNguoiGui", cauHoi.maNguoiGui);
            lenh.Parameters.AddWithValue("@cauHoi", cauHoi.cauHoi);
            lenh.Parameters.AddWithValue("@ngayGui", DateTime.Now);
            lenh.Parameters.AddWithValue("@trangThai", "Chờ trả lời");
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // Mục đích: phương thức traLoi thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public void traLoi(int maCauHoi, int maNguoiTraLoi, string cauTraLoi)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"update CauHoiTuVan
set maNguoiTraLoi = @maNguoiTraLoi, cauTraLoi = @cauTraLoi, ngayTraLoi = @ngayTraLoi, trangThai = @trangThai
where maCauHoi = @maCauHoi";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maCauHoi", maCauHoi);
            lenh.Parameters.AddWithValue("@maNguoiTraLoi", maNguoiTraLoi);
            lenh.Parameters.AddWithValue("@cauTraLoi", cauTraLoi);
            lenh.Parameters.AddWithValue("@ngayTraLoi", DateTime.Now);
            lenh.Parameters.AddWithValue("@trangThai", "Đã trả lời");
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // Mục đích: phương thức docCauHoi thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static cauHoiTuVanModels docCauHoi(SqlDataReader doc)
        {
            return new cauHoiTuVanModels
            {
                maCauHoi = Convert.ToInt32(doc["maCauHoi"]),
                maNguoiGui = Convert.ToInt32(doc["maNguoiGui"]),
                maNguoiTraLoi = doc["maNguoiTraLoi"] == DBNull.Value ? null : Convert.ToInt32(doc["maNguoiTraLoi"]),
                cauHoi = doc["cauHoi"].ToString() ?? string.Empty,
                cauTraLoi = doc["cauTraLoi"] == DBNull.Value ? null : doc["cauTraLoi"].ToString(),
                ngayGui = Convert.ToDateTime(doc["ngayGui"]),
                ngayTraLoi = doc["ngayTraLoi"] == DBNull.Value ? null : Convert.ToDateTime(doc["ngayTraLoi"]),
                trangThai = doc["trangThai"].ToString() ?? string.Empty,
                tenNguoiGui = doc["tenNguoiGui"].ToString(),
                tenNguoiTraLoi = doc["tenNguoiTraLoi"] == DBNull.Value ? null : doc["tenNguoiTraLoi"].ToString()
            };
        }
    }
}
