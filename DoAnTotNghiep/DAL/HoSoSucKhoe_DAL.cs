using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class HoSoSucKhoe_DAL
    {
        private readonly string chuoiKetNoi;

        public HoSoSucKhoe_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public bool KiemTraTaiKhoanDaCoHoSo(int maTaiKhoan)
        {
            const string sql = "SELECT COUNT(*) FROM HoSoSucKhoe WHERE maTaiKhoan = @MaTaiKhoan";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        public List<HoSoSucKhoe> LayDanhSachTheoTaiKhoan(int maTaiKhoan)
        {
            const string sql = @"SELECT
    maHoSo,
    maTaiKhoan,
    hoTen,
    ngaySinh,
    gioiTinh,
    chieuCao,
    canNang,
    tienSuBenh,
    diUng,
    ngayTao
FROM HoSoSucKhoe
WHERE maTaiKhoan = @MaTaiKhoan
ORDER BY ngayTao DESC";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<HoSoSucKhoe>();
            while (doc.Read())
            {
                danhSach.Add(DocHoSo(doc));
            }

            return danhSach;
        }

        public HoSoSucKhoe? LayTheoId(int maHoSo)
        {
            const string sql = @"SELECT
    maHoSo,
    maTaiKhoan,
    hoTen,
    ngaySinh,
    gioiTinh,
    chieuCao,
    canNang,
    tienSuBenh,
    diUng,
    ngayTao
FROM HoSoSucKhoe
WHERE maHoSo = @MaHoSo";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocHoSo(doc) : null;
        }

        public HoSoSucKhoe? LayTheoId(int maHoSo, int maTaiKhoan)
        {
            const string sql = @"SELECT
    maHoSo,
    maTaiKhoan,
    hoTen,
    ngaySinh,
    gioiTinh,
    chieuCao,
    canNang,
    tienSuBenh,
    diUng,
    ngayTao
FROM HoSoSucKhoe
WHERE maHoSo = @MaHoSo
AND maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocHoSo(doc) : null;
        }

        public bool Them(HoSoSucKhoe hs)
        {
            const string sql = @"INSERT INTO HoSoSucKhoe
(
    maTaiKhoan,
    hoTen,
    ngaySinh,
    gioiTinh,
    chieuCao,
    canNang,
    tienSuBenh,
    diUng,
    ngayTao
)
VALUES
(
    @MaTaiKhoan,
    @HoTen,
    @NgaySinh,
    @GioiTinh,
    @ChieuCao,
    @CanNang,
    @TienSuBenh,
    @DiUng,
    @NgayTao
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSoHoSo(lenh, hs);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool CapNhat(HoSoSucKhoe hs)
        {
            const string sql = @"UPDATE HoSoSucKhoe
SET hoTen = @HoTen,
    ngaySinh = @NgaySinh,
    gioiTinh = @GioiTinh,
    chieuCao = @ChieuCao,
    canNang = @CanNang,
    tienSuBenh = @TienSuBenh,
    diUng = @DiUng
WHERE maHoSo = @MaHoSo
AND maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", hs.MaHoSo);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", hs.MaTaiKhoan);
            lenh.Parameters.AddWithValue("@HoTen", hs.HoTen);
            lenh.Parameters.AddWithValue("@NgaySinh", hs.NgaySinh.Date);
            lenh.Parameters.AddWithValue("@GioiTinh", (object?)hs.GioiTinh ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@ChieuCao", (object?)hs.ChieuCao ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@CanNang", (object?)hs.CanNang ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@TienSuBenh", (object?)hs.TienSuBenh ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@DiUng", (object?)hs.DiUng ?? DBNull.Value);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool Xoa(int maHoSo, int maTaiKhoan)
        {
            const string sql = @"DELETE FROM HoSoSucKhoe
WHERE maHoSo = @MaHoSo
AND maTaiKhoan = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public HoSoSucKhoe? LayHoSoDauTienTheoTaiKhoan(int maTaiKhoan)
        {
            const string sql = @"SELECT TOP 1 *
FROM HoSoSucKhoe
WHERE maTaiKhoan = @MaTaiKhoan
ORDER BY ngayTao ASC";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocHoSo(doc) : null;
        }

        private static void GanThamSoHoSo(SqlCommand lenh, HoSoSucKhoe hs)
        {
            lenh.Parameters.AddWithValue("@MaTaiKhoan", hs.MaTaiKhoan);
            lenh.Parameters.AddWithValue("@HoTen", hs.HoTen);
            lenh.Parameters.AddWithValue("@NgaySinh", hs.NgaySinh.Date);
            lenh.Parameters.AddWithValue("@GioiTinh", (object?)hs.GioiTinh ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@ChieuCao", (object?)hs.ChieuCao ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@CanNang", (object?)hs.CanNang ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@TienSuBenh", (object?)hs.TienSuBenh ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@DiUng", (object?)hs.DiUng ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@NgayTao", hs.NgayTao);
        }

        private static HoSoSucKhoe DocHoSo(SqlDataReader doc)
        {
            return new HoSoSucKhoe
            {
                MaHoSo = Convert.ToInt32(doc["maHoSo"]),
                MaTaiKhoan = Convert.ToInt32(doc["maTaiKhoan"]),
                HoTen = doc["hoTen"] == DBNull.Value ? string.Empty : doc["hoTen"].ToString() ?? string.Empty,
                NgaySinh = Convert.ToDateTime(doc["ngaySinh"]),
                GioiTinh = doc["gioiTinh"] == DBNull.Value ? string.Empty : doc["gioiTinh"].ToString() ?? string.Empty,
                ChieuCao = doc["chieuCao"] == DBNull.Value ? null : Convert.ToDouble(doc["chieuCao"]),
                CanNang = doc["canNang"] == DBNull.Value ? null : Convert.ToDouble(doc["canNang"]),
                TienSuBenh = doc["tienSuBenh"] == DBNull.Value ? string.Empty : doc["tienSuBenh"].ToString() ?? string.Empty,
                DiUng = doc["diUng"] == DBNull.Value ? string.Empty : doc["diUng"].ToString() ?? string.Empty,
                NgayTao = Convert.ToDateTime(doc["ngayTao"])
            };
        }
    }
}
