using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp baiVietCamNangDAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
    public class baiVietCamNangDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public baiVietCamNangDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        // Mục đích: phương thức layTatCa thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<baiVietCamNangModels> layTatCa(bool chiLayDangHienThi = false)
        {
            var sql = @"select bv.maBaiViet, bv.maTaiKhoan, bv.tieuDe, bv.noiDung, bv.ngayTao, bv.trangThai, tk.hoTen as tenTacGia
from BaiVietCamNang bv
inner join TaiKhoan tk on bv.maTaiKhoan = tk.maTaiKhoan";
            if (chiLayDangHienThi)
            {
                sql += " where bv.trangThai = 1";
            }
            sql += " order by bv.ngayTao desc";

            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<baiVietCamNangModels>();
            while (doc.Read())
            {
                danhSach.Add(docBaiViet(doc));
            }
            return danhSach;
        }

        // Mục đích: phương thức layTheoMa thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public baiVietCamNangModels? layTheoMa(int maBaiViet)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select bv.maBaiViet, bv.maTaiKhoan, bv.tieuDe, bv.noiDung, bv.ngayTao, bv.trangThai, tk.hoTen as tenTacGia
from BaiVietCamNang bv
inner join TaiKhoan tk on bv.maTaiKhoan = tk.maTaiKhoan
where bv.maBaiViet = @maBaiViet";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maBaiViet", maBaiViet);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? docBaiViet(doc) : null;
        }

        // Mục đích: phương thức them thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public void them(baiVietCamNangModels baiViet)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"insert into BaiVietCamNang(maTaiKhoan, tieuDe, noiDung, ngayTao, trangThai)
values(@maTaiKhoan, @tieuDe, @noiDung, @ngayTao, @trangThai)";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", baiViet.maTaiKhoan);
            lenh.Parameters.AddWithValue("@tieuDe", baiViet.tieuDe);
            lenh.Parameters.AddWithValue("@noiDung", baiViet.noiDung);
            lenh.Parameters.AddWithValue("@ngayTao", DateTime.Now);
            lenh.Parameters.AddWithValue("@trangThai", baiViet.trangThai);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // Mục đích: phương thức capNhat thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public void capNhat(baiVietCamNangModels baiViet)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"update BaiVietCamNang
set tieuDe = @tieuDe, noiDung = @noiDung, trangThai = @trangThai
where maBaiViet = @maBaiViet";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maBaiViet", baiViet.maBaiViet);
            lenh.Parameters.AddWithValue("@tieuDe", baiViet.tieuDe);
            lenh.Parameters.AddWithValue("@noiDung", baiViet.noiDung);
            lenh.Parameters.AddWithValue("@trangThai", baiViet.trangThai);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // Mục đích: phương thức docBaiViet thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static baiVietCamNangModels docBaiViet(SqlDataReader doc)
        {
            return new baiVietCamNangModels
            {
                maBaiViet = Convert.ToInt32(doc["maBaiViet"]),
                maTaiKhoan = Convert.ToInt32(doc["maTaiKhoan"]),
                tieuDe = doc["tieuDe"].ToString() ?? string.Empty,
                noiDung = doc["noiDung"].ToString() ?? string.Empty,
                ngayTao = Convert.ToDateTime(doc["ngayTao"]),
                trangThai = Convert.ToBoolean(doc["trangThai"]),
                tenTacGia = doc["tenTacGia"].ToString()
            };
        }
    }
}
