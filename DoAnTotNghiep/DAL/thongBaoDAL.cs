using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp thongBaoDAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
    public class thongBaoDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public thongBaoDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        // Mục đích: phương thức layTheoTaiKhoan thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<thongBaoModels> layTheoTaiKhoan(int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select maThongBao, maTaiKhoan, tieuDe, noiDung, ngayGui, daDoc
from ThongBao
where maTaiKhoan = @maTaiKhoan
order by ngayGui desc";

            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);

            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<thongBaoModels>();
            while (doc.Read())
            {
                danhSach.Add(new thongBaoModels
                {
                    maThongBao = Convert.ToInt32(doc["maThongBao"]),
                    maTaiKhoan = Convert.ToInt32(doc["maTaiKhoan"]),
                    maLichTiem = null,
                    tieuDe = doc["tieuDe"].ToString() ?? string.Empty,
                    noiDung = doc["noiDung"] == DBNull.Value ? null : doc["noiDung"].ToString(),
                    ngayGui = Convert.ToDateTime(doc["ngayGui"]),
                    daDoc = Convert.ToBoolean(doc["daDoc"])
                });
            }

            return danhSach;
        }

        // Mục đích: phương thức danhDauDaDoc thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public void danhDauDaDoc(int maThongBao, int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = "update ThongBao set daDoc = 1 where maThongBao = @maThongBao and maTaiKhoan = @maTaiKhoan";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maThongBao", maThongBao);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }
    }
}
