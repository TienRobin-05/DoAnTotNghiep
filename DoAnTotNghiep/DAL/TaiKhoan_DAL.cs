using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class TaiKhoan_DAL
    {
        private readonly string chuoiKetNoi;

        public TaiKhoan_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public bool KiemTraSoDienThoaiTonTai(string soDienThoai)
        {
            const string sql = "SELECT COUNT(*) FROM TaiKhoan WHERE soDienThoai = @SoDienThoai";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        public bool KiemTraEmailTonTai(string email)
        {
            const string sql = "SELECT COUNT(*) FROM TaiKhoan WHERE email = @Email";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@Email", email);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        public bool DangKy(TaiKhoan tk)
        {
            const string sql = @"INSERT INTO TaiKhoan
(
    hoTen,
    email,
    matKhau,
    soDienThoai,
    vaiTro,
    trangThai,
    ngayTao
)
VALUES
(
    @HoTen,
    @Email,
    @MatKhau,
    @SoDienThoai,
    @VaiTro,
    @TrangThai,
    @NgayTao
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSoTaiKhoan(lenh, tk);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public TaiKhoan? DangNhap(string soDienThoai, string matKhau)
        {
            const string sql = @"SELECT
    maTaiKhoan,
    hoTen,
    email,
    matKhau,
    soDienThoai,
    vaiTro,
    trangThai,
    ngayTao
FROM TaiKhoan
WHERE soDienThoai = @SoDienThoai
AND matKhau = @MatKhau";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
            lenh.Parameters.AddWithValue("@MatKhau", matKhau);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocTaiKhoan(doc) : null;
        }

        public TaiKhoan? LayTaiKhoanTheoId(int maTaiKhoan)
        {
            const string sql = @"SELECT
    maTaiKhoan,
    hoTen,
    email,
    matKhau,
    soDienThoai,
    vaiTro,
    trangThai,
    ngayTao
FROM TaiKhoan
WHERE maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocTaiKhoan(doc) : null;
        }

        public bool CapNhatHoTen(int maTaiKhoan, string hoTen)
        {
            const string sql = @"UPDATE TaiKhoan
SET hoTen = @HoTen
WHERE maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@HoTen", hoTen);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        private static void GanThamSoTaiKhoan(SqlCommand lenh, TaiKhoan tk)
        {
            lenh.Parameters.AddWithValue("@HoTen", tk.HoTen);
            lenh.Parameters.AddWithValue("@Email", tk.Email);
            lenh.Parameters.AddWithValue("@MatKhau", tk.MatKhau);
            lenh.Parameters.AddWithValue("@SoDienThoai", tk.SoDienThoai);
            lenh.Parameters.AddWithValue("@VaiTro", tk.VaiTro);
            lenh.Parameters.AddWithValue("@TrangThai", tk.TrangThai);
            lenh.Parameters.AddWithValue("@NgayTao", tk.NgayTao);
        }

        private static TaiKhoan DocTaiKhoan(SqlDataReader doc)
        {
            return new TaiKhoan
            {
                MaTaiKhoan = Convert.ToInt32(doc["maTaiKhoan"]),
                HoTen = doc["hoTen"] == DBNull.Value ? string.Empty : doc["hoTen"].ToString() ?? string.Empty,
                Email = doc["email"] == DBNull.Value ? string.Empty : doc["email"].ToString() ?? string.Empty,
                MatKhau = doc["matKhau"] == DBNull.Value ? string.Empty : doc["matKhau"].ToString() ?? string.Empty,
                SoDienThoai = doc["soDienThoai"] == DBNull.Value ? string.Empty : doc["soDienThoai"].ToString() ?? string.Empty,
                VaiTro = doc["vaiTro"] == DBNull.Value ? string.Empty : doc["vaiTro"].ToString() ?? string.Empty,
                TrangThai = Convert.ToBoolean(doc["trangThai"]),
                NgayTao = Convert.ToDateTime(doc["ngayTao"])
            };
        }
    }
}
