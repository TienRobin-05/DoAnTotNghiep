using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.Models
{
    public class Tai_Khoan_DAL
    {
        private readonly string _connectionString;

        public Tai_Khoan_DAL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public Tai_Khoan? DangNhap(string email, string matKhau)
        {
            Tai_Khoan? taiKhoan = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT maTaiKhoan, hoTen, email, matKhau, soDienThoai, vaiTro, trangThai, ngayTao
                    FROM TaiKhoan
                    WHERE email = @email 
                    AND matKhau = @matKhau
                    AND trangThai = 1
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@matKhau", matKhau);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            taiKhoan = new Tai_Khoan
                            {
                                maTaiKhoan = Convert.ToInt32(reader["maTaiKhoan"]),
                                hoTen = reader["hoTen"].ToString() ?? "",
                                email = reader["email"].ToString() ?? "",
                                matKhau = reader["matKhau"].ToString() ?? "",
                                soDienThoai = reader["soDienThoai"].ToString() ?? "",
                                vaiTro = reader["vaiTro"].ToString() ?? "",
                                trangThai = reader["trangThai"].ToString() ?? "",
                                ngayTao = reader["ngayTao"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(reader["ngayTao"])
                            };
                        }
                    }
                }
            }

            return taiKhoan;
        }

        public bool EmailDaTonTai(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = "SELECT COUNT(1) FROM TaiKhoan WHERE email = @email";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public Tai_Khoan? LayTheoMa(int maTaiKhoan)
        {
            // Lấy thông tin tài khoản theo mã đang lưu trong Session.
            Tai_Khoan? taiKhoan = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT maTaiKhoan, hoTen, email, matKhau, soDienThoai, vaiTro, trangThai, ngayTao
                    FROM TaiKhoan
                    WHERE maTaiKhoan = @maTaiKhoan
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            taiKhoan = new Tai_Khoan
                            {
                                maTaiKhoan = Convert.ToInt32(reader["maTaiKhoan"]),
                                hoTen = reader["hoTen"].ToString() ?? "",
                                email = reader["email"].ToString() ?? "",
                                matKhau = reader["matKhau"].ToString() ?? "",
                                soDienThoai = reader["soDienThoai"].ToString() ?? "",
                                vaiTro = reader["vaiTro"].ToString() ?? "",
                                trangThai = reader["trangThai"].ToString() ?? "",
                                ngayTao = reader["ngayTao"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(reader["ngayTao"])
                            };
                        }
                    }
                }
            }

            return taiKhoan;
        }

        public bool CapNhatHoTen(int maTaiKhoan, string hoTen)
        {
            // Chỉ cập nhật họ tên, không cập nhật số điện thoại tại màn hình thông tin cá nhân.
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = @"
                    UPDATE TaiKhoan
                    SET hoTen = @hoTen
                    WHERE maTaiKhoan = @maTaiKhoan
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
                    cmd.Parameters.AddWithValue("@hoTen", hoTen);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DangKy(Dang_Ky_View_Model model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = @"
                    INSERT INTO TaiKhoan (hoTen, email, matKhau, soDienThoai, vaiTro, trangThai, ngayTao)
                    VALUES (@hoTen, @email, @matKhau, @soDienThoai, @vaiTro, @trangThai, @ngayTao)
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@hoTen", model.hoTen);
                    cmd.Parameters.AddWithValue("@email", model.email);
                    cmd.Parameters.AddWithValue("@matKhau", model.matKhau);
                    cmd.Parameters.AddWithValue("@soDienThoai", string.IsNullOrWhiteSpace(model.soDienThoai) ? DBNull.Value : model.soDienThoai);
                    cmd.Parameters.AddWithValue("@vaiTro", "NguoiDung");
                    cmd.Parameters.AddWithValue("@trangThai", true);
                    cmd.Parameters.AddWithValue("@ngayTao", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DoiMatKhau(string email, string matKhauMoi)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = @"
                    UPDATE TaiKhoan
                    SET matKhau = @matKhauMoi
                    WHERE email = @email
                    AND trangThai = 1
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@matKhauMoi", matKhauMoi);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
