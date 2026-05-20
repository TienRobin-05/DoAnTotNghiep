using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class MuiTiemVaccine_DAL
    {
        private readonly string chuoiKetNoi;

        public MuiTiemVaccine_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public List<MuiTiemVaccine> LayDanhSach()
        {
            const string sql = @"SELECT
    mt.maMuiTiem,
    mt.maVaccine,
    mt.soMui,
    mt.tenMui,
    mt.doTuoiToiThieu,
    mt.doTuoiToiDa,
    mt.doTuoiKhuyenNghi,
    mt.donViTuoi,
    mt.khoangCachNgay,
    mt.ghiChu,
    v.tenVaccine
FROM MuiTiemVaccine mt
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
ORDER BY v.tenVaccine, mt.soMui";

            return DocDanhSach(sql);
        }

        public List<MuiTiemVaccine> LayDanhSachTheoVaccine(int maVaccine)
        {
            const string sql = @"SELECT
    mt.maMuiTiem,
    mt.maVaccine,
    mt.soMui,
    mt.tenMui,
    mt.doTuoiToiThieu,
    mt.doTuoiToiDa,
    mt.doTuoiKhuyenNghi,
    mt.donViTuoi,
    mt.khoangCachNgay,
    mt.ghiChu,
    v.tenVaccine
FROM MuiTiemVaccine mt
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE mt.maVaccine = @MaVaccine
ORDER BY mt.soMui";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaVaccine", maVaccine);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return DocDanhSachTuReader(doc);
        }

        public MuiTiemVaccine? LayTheoId(int maMuiTiem)
        {
            const string sql = @"SELECT
    mt.maMuiTiem,
    mt.maVaccine,
    mt.soMui,
    mt.tenMui,
    mt.doTuoiToiThieu,
    mt.doTuoiToiDa,
    mt.doTuoiKhuyenNghi,
    mt.donViTuoi,
    mt.khoangCachNgay,
    mt.ghiChu,
    v.tenVaccine
FROM MuiTiemVaccine mt
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE mt.maMuiTiem = @MaMuiTiem";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocMuiTiem(doc) : null;
        }

        public bool Them(MuiTiemVaccine mt)
        {
            const string sql = @"INSERT INTO MuiTiemVaccine
(
    maVaccine,
    soMui,
    tenMui,
    doTuoiToiThieu,
    doTuoiToiDa,
    doTuoiKhuyenNghi,
    donViTuoi,
    khoangCachNgay,
    ghiChu
)
VALUES
(
    @MaVaccine,
    @SoMui,
    @TenMui,
    @DoTuoiToiThieu,
    @DoTuoiToiDa,
    @DoTuoiKhuyenNghi,
    @DonViTuoi,
    @KhoangCachNgay,
    @GhiChu
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSo(lenh, mt);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool CapNhat(MuiTiemVaccine mt)
        {
            const string sql = @"UPDATE MuiTiemVaccine
SET maVaccine = @MaVaccine,
    soMui = @SoMui,
    tenMui = @TenMui,
    doTuoiToiThieu = @DoTuoiToiThieu,
    doTuoiToiDa = @DoTuoiToiDa,
    doTuoiKhuyenNghi = @DoTuoiKhuyenNghi,
    donViTuoi = @DonViTuoi,
    khoangCachNgay = @KhoangCachNgay,
    ghiChu = @GhiChu
WHERE maMuiTiem = @MaMuiTiem";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaMuiTiem", mt.MaMuiTiem);
            GanThamSo(lenh, mt);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool Xoa(int maMuiTiem)
        {
            const string sql = "DELETE FROM MuiTiemVaccine WHERE maMuiTiem = @MaMuiTiem";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool KiemTraTrungSoMui(int maVaccine, int soMui, int? maMuiTiemBoQua)
        {
            var sql = @"SELECT COUNT(*)
FROM MuiTiemVaccine
WHERE maVaccine = @MaVaccine
AND soMui = @SoMui";

            if (maMuiTiemBoQua.HasValue)
            {
                sql += " AND maMuiTiem <> @MaMuiTiemBoQua";
            }

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaVaccine", maVaccine);
            lenh.Parameters.AddWithValue("@SoMui", soMui);
            if (maMuiTiemBoQua.HasValue)
            {
                lenh.Parameters.AddWithValue("@MaMuiTiemBoQua", maMuiTiemBoQua.Value);
            }

            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        private List<MuiTiemVaccine> DocDanhSach(string sql)
        {
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return DocDanhSachTuReader(doc);
        }

        private static List<MuiTiemVaccine> DocDanhSachTuReader(SqlDataReader doc)
        {
            var danhSach = new List<MuiTiemVaccine>();
            while (doc.Read())
            {
                danhSach.Add(DocMuiTiem(doc));
            }

            return danhSach;
        }

        private static void GanThamSo(SqlCommand lenh, MuiTiemVaccine mt)
        {
            lenh.Parameters.AddWithValue("@MaVaccine", mt.MaVaccine);
            lenh.Parameters.AddWithValue("@SoMui", mt.SoMui);
            lenh.Parameters.AddWithValue("@TenMui", string.IsNullOrWhiteSpace(mt.TenMui) ? DBNull.Value : mt.TenMui);
            lenh.Parameters.AddWithValue("@DoTuoiToiThieu", (object?)mt.DoTuoiToiThieu ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@DoTuoiToiDa", (object?)mt.DoTuoiToiDa ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@DoTuoiKhuyenNghi", (object?)mt.DoTuoiKhuyenNghi ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@DonViTuoi", string.IsNullOrWhiteSpace(mt.DonViTuoi) ? DBNull.Value : mt.DonViTuoi);
            lenh.Parameters.AddWithValue("@KhoangCachNgay", (object?)mt.KhoangCachNgay ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@GhiChu", string.IsNullOrWhiteSpace(mt.GhiChu) ? DBNull.Value : mt.GhiChu);
        }

        private static MuiTiemVaccine DocMuiTiem(SqlDataReader doc)
        {
            return new MuiTiemVaccine
            {
                MaMuiTiem = Convert.ToInt32(doc["maMuiTiem"]),
                MaVaccine = Convert.ToInt32(doc["maVaccine"]),
                SoMui = Convert.ToInt32(doc["soMui"]),
                TenMui = doc["tenMui"] == DBNull.Value ? string.Empty : doc["tenMui"].ToString() ?? string.Empty,
                DoTuoiToiThieu = doc["doTuoiToiThieu"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiThieu"]),
                DoTuoiToiDa = doc["doTuoiToiDa"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiDa"]),
                DoTuoiKhuyenNghi = doc["doTuoiKhuyenNghi"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiKhuyenNghi"]),
                DonViTuoi = doc["donViTuoi"] == DBNull.Value ? string.Empty : doc["donViTuoi"].ToString() ?? string.Empty,
                KhoangCachNgay = doc["khoangCachNgay"] == DBNull.Value ? null : Convert.ToInt32(doc["khoangCachNgay"]),
                GhiChu = doc["ghiChu"] == DBNull.Value ? string.Empty : doc["ghiChu"].ToString() ?? string.Empty,
                TenVaccine = doc["tenVaccine"] == DBNull.Value ? string.Empty : doc["tenVaccine"].ToString() ?? string.Empty
            };
        }
    }
}
