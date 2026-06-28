using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class CauHoiTuVan_DAL
    {
        private readonly string chuoiKetNoi;

        public CauHoiTuVan_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        // lấy câu hỏi theo người gửi
        public List<CauHoiTuVan> LayDanhSachTheoNguoiGui(int maTaiKhoan)
        {
            DamBaoCotMaVaccine();
            const string sql = @"SELECT
    ch.maCauHoi,
    ch.maNguoiGui,
    ch.maNguoiTraLoi,
    ch.maVaccine,
    ch.cauHoi,
    ch.cauTraLoi,
    ch.ngayGui,
    ch.ngayTraLoi,
    ch.trangThai,
    gui.hoTen AS tenNguoiGui,
    traLoi.hoTen AS tenNguoiTraLoi,
    v.tenVaccine,
    v.nhomVaccine,
    v.doTuoiToiThieu,
    v.doTuoiToiDa,
    v.donViTuoi
FROM CauHoiTuVan ch
INNER JOIN TaiKhoan gui ON ch.maNguoiGui = gui.maTaiKhoan
LEFT JOIN TaiKhoan traLoi ON ch.maNguoiTraLoi = traLoi.maTaiKhoan
LEFT JOIN Vaccine v ON ch.maVaccine = v.maVaccine
WHERE ch.maNguoiGui = @MaTaiKhoan
ORDER BY ch.ngayGui DESC";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<CauHoiTuVan>();
            while (doc.Read())
            {
                danhSach.Add(DocCauHoi(doc));
            }

            return danhSach;
        }

        // lấy tất cả câu hỏi
        public List<CauHoiTuVan> LayTatCa()
        {
            DamBaoCotMaVaccine();
            const string sql = @"SELECT
    ch.maCauHoi,
    ch.maNguoiGui,
    ch.maNguoiTraLoi,
    ch.maVaccine,
    ch.cauHoi,
    ch.cauTraLoi,
    ch.ngayGui,
    ch.ngayTraLoi,
    ch.trangThai,
    gui.hoTen AS tenNguoiGui,
    traLoi.hoTen AS tenNguoiTraLoi,
    v.tenVaccine,
    v.nhomVaccine,
    v.doTuoiToiThieu,
    v.doTuoiToiDa,
    v.donViTuoi
FROM CauHoiTuVan ch
INNER JOIN TaiKhoan gui ON ch.maNguoiGui = gui.maTaiKhoan
LEFT JOIN TaiKhoan traLoi ON ch.maNguoiTraLoi = traLoi.maTaiKhoan
LEFT JOIN Vaccine v ON ch.maVaccine = v.maVaccine
ORDER BY ch.ngayGui DESC";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<CauHoiTuVan>();
            while (doc.Read())
            {
                danhSach.Add(DocCauHoi(doc));
            }

            return danhSach;
        }

        // lấy câu hỏi theo mã
        public CauHoiTuVan? LayTheoIdCuaNguoiGui(int maCauHoi, int maTaiKhoan)
        {
            DamBaoCotMaVaccine();
            const string sql = @"SELECT
    ch.maCauHoi,
    ch.maNguoiGui,
    ch.maNguoiTraLoi,
    ch.maVaccine,
    ch.cauHoi,
    ch.cauTraLoi,
    ch.ngayGui,
    ch.ngayTraLoi,
    ch.trangThai,
    gui.hoTen AS tenNguoiGui,
    traLoi.hoTen AS tenNguoiTraLoi,
    v.tenVaccine,
    v.nhomVaccine,
    v.doTuoiToiThieu,
    v.doTuoiToiDa,
    v.donViTuoi
FROM CauHoiTuVan ch
INNER JOIN TaiKhoan gui ON ch.maNguoiGui = gui.maTaiKhoan
LEFT JOIN TaiKhoan traLoi ON ch.maNguoiTraLoi = traLoi.maTaiKhoan
LEFT JOIN Vaccine v ON ch.maVaccine = v.maVaccine
WHERE ch.maCauHoi = @MaCauHoi
AND ch.maNguoiGui = @MaTaiKhoan";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaCauHoi", maCauHoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocCauHoi(doc) : null;
        }

        // lấy câu hỏi theo mã
        public CauHoiTuVan? LayTheoId(int maCauHoi)
        {
            DamBaoCotMaVaccine();
            const string sql = @"SELECT
    ch.maCauHoi,
    ch.maNguoiGui,
    ch.maNguoiTraLoi,
    ch.maVaccine,
    ch.cauHoi,
    ch.cauTraLoi,
    ch.ngayGui,
    ch.ngayTraLoi,
    ch.trangThai,
    gui.hoTen AS tenNguoiGui,
    traLoi.hoTen AS tenNguoiTraLoi,
    v.tenVaccine,
    v.nhomVaccine,
    v.doTuoiToiThieu,
    v.doTuoiToiDa,
    v.donViTuoi
FROM CauHoiTuVan ch
INNER JOIN TaiKhoan gui ON ch.maNguoiGui = gui.maTaiKhoan
LEFT JOIN TaiKhoan traLoi ON ch.maNguoiTraLoi = traLoi.maTaiKhoan
LEFT JOIN Vaccine v ON ch.maVaccine = v.maVaccine
WHERE ch.maCauHoi = @MaCauHoi";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaCauHoi", maCauHoi);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocCauHoi(doc) : null;
        }

        public bool GuiCauHoi(CauHoiTuVan ch)
        {
            DamBaoCotMaVaccine();
            const string sql = @"INSERT INTO CauHoiTuVan
(
    maNguoiGui,
    maNguoiTraLoi,
    maVaccine,
    cauHoi,
    cauTraLoi,
    ngayGui,
    ngayTraLoi,
    trangThai
)
VALUES
(
    @MaNguoiGui,
    @MaNguoiTraLoi,
    @MaVaccine,
    @CauHoi,
    @CauTraLoi,
    @NgayGui,
    @NgayTraLoi,
    @TrangThai
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaNguoiGui", ch.MaNguoiGui);
            lenh.Parameters.AddWithValue("@MaNguoiTraLoi", DBNull.Value);
            lenh.Parameters.AddWithValue("@MaVaccine", (object?)ch.MaVaccine ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@CauHoi", ch.CauHoi);
            lenh.Parameters.AddWithValue("@CauTraLoi", DBNull.Value);
            lenh.Parameters.AddWithValue("@NgayGui", ch.NgayGui);
            lenh.Parameters.AddWithValue("@NgayTraLoi", DBNull.Value);
            lenh.Parameters.AddWithValue("@TrangThai", ch.TrangThai);

            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public int GuiCauHoiVaLayId(CauHoiTuVan ch)
        {
            DamBaoCotMaVaccine();
            const string sql = @"INSERT INTO CauHoiTuVan
(
    maNguoiGui,
    maNguoiTraLoi,
    maVaccine,
    cauHoi,
    cauTraLoi,
    ngayGui,
    ngayTraLoi,
    trangThai
)
OUTPUT INSERTED.maCauHoi
VALUES
(
    @MaNguoiGui,
    @MaNguoiTraLoi,
    @MaVaccine,
    @CauHoi,
    @CauTraLoi,
    @NgayGui,
    @NgayTraLoi,
    @TrangThai
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaNguoiGui", ch.MaNguoiGui);
            lenh.Parameters.AddWithValue("@MaNguoiTraLoi", DBNull.Value);
            lenh.Parameters.AddWithValue("@MaVaccine", (object?)ch.MaVaccine ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@CauHoi", ch.CauHoi);
            lenh.Parameters.AddWithValue("@CauTraLoi", DBNull.Value);
            lenh.Parameters.AddWithValue("@NgayGui", ch.NgayGui);
            lenh.Parameters.AddWithValue("@NgayTraLoi", DBNull.Value);
            lenh.Parameters.AddWithValue("@TrangThai", ch.TrangThai);

            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }

        // trả lời câu hỏi
        public bool TraLoi(int maCauHoi, int maNguoiTraLoi, string cauTraLoi)
        {
            const string sql = @"UPDATE CauHoiTuVan
SET maNguoiTraLoi = @MaNguoiTraLoi,
    cauTraLoi = @CauTraLoi,
    ngayTraLoi = @NgayTraLoi,
    trangThai = @TrangThai
WHERE maCauHoi = @MaCauHoi";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaCauHoi", maCauHoi);
            lenh.Parameters.AddWithValue("@MaNguoiTraLoi", maNguoiTraLoi);
            lenh.Parameters.AddWithValue("@CauTraLoi", cauTraLoi);
            lenh.Parameters.AddWithValue("@NgayTraLoi", DateTime.Now);
            lenh.Parameters.AddWithValue("@TrangThai", "Đã trả lời");
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        private void DamBaoCotMaVaccine()
        {
            const string sql = @"
IF COL_LENGTH('dbo.CauHoiTuVan', 'maVaccine') IS NULL
BEGIN
    ALTER TABLE dbo.CauHoiTuVan ADD maVaccine INT NULL;
END;

IF COL_LENGTH('dbo.CauHoiTuVan', 'maVaccine') IS NOT NULL
    AND OBJECT_ID('dbo.Vaccine', 'U') IS NOT NULL
    AND NOT EXISTS (
        SELECT 1
        FROM sys.foreign_keys
        WHERE name = N'FK_CauHoiTuVan_Vaccine'
          AND parent_object_id = OBJECT_ID(N'dbo.CauHoiTuVan')
    )
BEGIN
    ALTER TABLE dbo.CauHoiTuVan
    ADD CONSTRAINT FK_CauHoiTuVan_Vaccine
    FOREIGN KEY (maVaccine) REFERENCES dbo.Vaccine(maVaccine);
END;";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        private static string HienThiDoTuoi(SqlDataReader doc)
        {
            if (!CotTonTai(doc, "doTuoiToiThieu") || doc["doTuoiToiThieu"] == DBNull.Value && doc["doTuoiToiDa"] == DBNull.Value)
            {
                return string.Empty;
            }

            var toiThieu = doc["doTuoiToiThieu"] == DBNull.Value ? "0" : doc["doTuoiToiThieu"].ToString();
            var toiDa = doc["doTuoiToiDa"] == DBNull.Value ? "không giới hạn" : doc["doTuoiToiDa"].ToString();
            var donVi = doc["donViTuoi"] == DBNull.Value ? string.Empty : $" {doc["donViTuoi"].ToString()?.Trim()}";
            return $"{toiThieu} - {toiDa}{donVi}".Trim();
        }

        private static bool CotTonTai(SqlDataReader doc, string tenCot)
        {
            for (var i = 0; i < doc.FieldCount; i++)
            {
                if (string.Equals(doc.GetName(i), tenCot, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static CauHoiTuVan DocCauHoi(SqlDataReader doc)
        {
            int? maNguoiTraLoi = doc["maNguoiTraLoi"] == DBNull.Value ? null : Convert.ToInt32(doc["maNguoiTraLoi"]);
            var cauTraLoi = doc["cauTraLoi"] == DBNull.Value ? string.Empty : doc["cauTraLoi"].ToString() ?? string.Empty;
            var tenNguoiTraLoi = maNguoiTraLoi.HasValue || !string.IsNullOrWhiteSpace(cauTraLoi)
                ? doc["tenNguoiTraLoi"] == DBNull.Value ? "Quản trị viên" : doc["tenNguoiTraLoi"].ToString() ?? "Quản trị viên"
                : string.Empty;

            return new CauHoiTuVan
            {
                MaCauHoi = Convert.ToInt32(doc["maCauHoi"]),
                MaNguoiGui = Convert.ToInt32(doc["maNguoiGui"]),
                MaNguoiTraLoi = maNguoiTraLoi,
                MaVaccine = doc["maVaccine"] == DBNull.Value ? null : Convert.ToInt32(doc["maVaccine"]),
                CauHoi = doc["cauHoi"] == DBNull.Value ? string.Empty : doc["cauHoi"].ToString() ?? string.Empty,
                CauTraLoi = cauTraLoi,
                NgayGui = Convert.ToDateTime(doc["ngayGui"]),
                NgayTraLoi = doc["ngayTraLoi"] == DBNull.Value ? null : Convert.ToDateTime(doc["ngayTraLoi"]),
                TrangThai = doc["trangThai"] == DBNull.Value ? string.Empty : doc["trangThai"].ToString() ?? string.Empty,
                TenNguoiGui = doc["tenNguoiGui"] == DBNull.Value ? string.Empty : doc["tenNguoiGui"].ToString() ?? string.Empty,
                TenNguoiTraLoi = tenNguoiTraLoi,
                TenVaccine = doc["tenVaccine"] == DBNull.Value ? string.Empty : doc["tenVaccine"].ToString() ?? string.Empty,
                NhomVaccine = doc["nhomVaccine"] == DBNull.Value ? string.Empty : doc["nhomVaccine"].ToString() ?? string.Empty,
                DoTuoiVaccine = HienThiDoTuoi(doc)
            };
        }
    }
}
