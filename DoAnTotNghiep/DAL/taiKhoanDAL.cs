using DoAnTotNghiep.Models;
using DoAnTotNghiep.Services;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp taiKhoanDAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
    public class taiKhoanDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public taiKhoanDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        // Mục đích: phương thức dangNhap thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public taiKhoanModels? dangNhap(string email, string matKhau)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select maTaiKhoan, hoTen, email, matKhau, soDienThoai, vaiTro, trangThai, ngayTao
from TaiKhoan
where email = @email and trangThai = 1";

            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@email", email);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            if (!doc.Read())
            {
                return null;
            }

            var taiKhoan = docTaiKhoan(doc);
            if (!MatKhauService.KiemTra(matKhau, taiKhoan.matKhau))
            {
                return null;
            }

            if (!MatKhauService.LaHash(taiKhoan.matKhau))
            {
                doc.Close();
                capNhatMatKhau(taiKhoan.maTaiKhoan, MatKhauService.TaoHash(matKhau));
            }

            return taiKhoan;
        }

        // Mục đích: phương thức emailDaTonTai thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public bool emailDaTonTai(string email)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = "select count(1) from TaiKhoan where email = @email";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@email", email);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // Mục đích: phương thức them thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public int them(taiKhoanModels taiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"insert into TaiKhoan(hoTen, email, matKhau, soDienThoai, vaiTro, trangThai, ngayTao)
values(@hoTen, @email, @matKhau, @soDienThoai, @vaiTro, @trangThai, @ngayTao);
select cast(scope_identity() as int);";

            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSoTaiKhoan(lenh, taiKhoan, themMoi: true);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }

        // Mục đích: phương thức layTatCa thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<taiKhoanModels> layTatCa()
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select maTaiKhoan, hoTen, email, matKhau, soDienThoai, vaiTro, trangThai, ngayTao
from TaiKhoan
order by maTaiKhoan desc";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<taiKhoanModels>();
            while (doc.Read())
            {
                danhSach.Add(docTaiKhoan(doc));
            }
            return danhSach;
        }

        // Đếm tài khoản bằng COUNT(*) để dashboard không phải tải toàn bộ danh sách chỉ để lấy số lượng.
        public int demTatCa()
        {
            const string sql = "select count(*) from TaiKhoan";

            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }

        // Mục đích: phương thức layTheoMa thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public taiKhoanModels? layTheoMa(int maTaiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select maTaiKhoan, hoTen, email, matKhau, soDienThoai, vaiTro, trangThai, ngayTao
from TaiKhoan where maTaiKhoan = @maTaiKhoan";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? docTaiKhoan(doc) : null;
        }

        // Mục đích: phương thức capNhat thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public void capNhat(taiKhoanModels taiKhoan)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"update TaiKhoan
set hoTen = @hoTen, email = @email, matKhau = @matKhau, soDienThoai = @soDienThoai,
    vaiTro = @vaiTro, trangThai = @trangThai
where maTaiKhoan = @maTaiKhoan";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSoTaiKhoan(lenh, taiKhoan, themMoi: false);
            lenh.Parameters.AddWithValue("@maTaiKhoan", taiKhoan.maTaiKhoan);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // Mục đích: phương thức doiTrangThai thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public void doiTrangThai(int maTaiKhoan, bool trangThai)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = "update TaiKhoan set trangThai = @trangThai where maTaiKhoan = @maTaiKhoan";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@trangThai", trangThai);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        private void capNhatMatKhau(int maTaiKhoan, string matKhauDaHash)
        {
            const string sql = "update TaiKhoan set matKhau = @matKhau where maTaiKhoan = @maTaiKhoan";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@matKhau", matKhauDaHash);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // Mục đích: phương thức ganThamSoTaiKhoan thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static void ganThamSoTaiKhoan(SqlCommand lenh, taiKhoanModels taiKhoan, bool themMoi)
        {
            lenh.Parameters.AddWithValue("@hoTen", taiKhoan.hoTen);
            lenh.Parameters.AddWithValue("@email", taiKhoan.email);
            lenh.Parameters.AddWithValue("@matKhau", MatKhauService.ChuanBiLuu(taiKhoan.matKhau));
            lenh.Parameters.AddWithValue("@soDienThoai", (object?)taiKhoan.soDienThoai ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@vaiTro", taiKhoan.vaiTro);
            lenh.Parameters.AddWithValue("@trangThai", taiKhoan.trangThai);
            if (themMoi)
            {
                lenh.Parameters.AddWithValue("@ngayTao", DateTime.Now);
            }
        }

        // Mục đích: phương thức docTaiKhoan thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static taiKhoanModels docTaiKhoan(SqlDataReader doc)
        {
            return new taiKhoanModels
            {
                maTaiKhoan = Convert.ToInt32(doc["maTaiKhoan"]),
                hoTen = doc["hoTen"].ToString() ?? string.Empty,
                email = doc["email"].ToString() ?? string.Empty,
                matKhau = doc["matKhau"].ToString() ?? string.Empty,
                soDienThoai = doc["soDienThoai"] == DBNull.Value ? null : doc["soDienThoai"].ToString(),
                vaiTro = doc["vaiTro"].ToString() ?? string.Empty,
                trangThai = Convert.ToBoolean(doc["trangThai"]),
                ngayTao = Convert.ToDateTime(doc["ngayTao"])
            };
        }
    }
}
