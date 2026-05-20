using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class Vaccine_DAL
    {
        private readonly string chuoiKetNoi;

        public Vaccine_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public List<Vaccine> LayDanhSach()
        {
            const string sql = @"SELECT
    maVaccine,
    tenVaccine,
    nhomVaccine,
    doTuoiToiThieu,
    doTuoiToiDa,
    donViTuoi,
    moTa,
    luuY,
    trangThai
FROM Vaccine
ORDER BY maVaccine DESC";

            return DocDanhSach(sql);
        }

        public List<Vaccine> LayDanhSachDangSuDung()
        {
            const string sql = @"SELECT
    maVaccine,
    tenVaccine,
    nhomVaccine,
    doTuoiToiThieu,
    doTuoiToiDa,
    donViTuoi,
    moTa,
    luuY,
    trangThai
FROM Vaccine
WHERE trangThai = 1
ORDER BY maVaccine DESC";

            return DocDanhSach(sql);
        }

        public Vaccine? LayTheoId(int maVaccine)
        {
            const string sql = @"SELECT
    maVaccine,
    tenVaccine,
    nhomVaccine,
    doTuoiToiThieu,
    doTuoiToiDa,
    donViTuoi,
    moTa,
    luuY,
    trangThai
FROM Vaccine
WHERE maVaccine = @MaVaccine";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaVaccine", maVaccine);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocVaccine(doc) : null;
        }

        public bool Them(Vaccine vaccine)
        {
            const string sql = @"INSERT INTO Vaccine
(
    tenVaccine,
    nhomVaccine,
    doTuoiToiThieu,
    doTuoiToiDa,
    donViTuoi,
    moTa,
    luuY,
    trangThai
)
VALUES
(
    @TenVaccine,
    @NhomVaccine,
    @DoTuoiToiThieu,
    @DoTuoiToiDa,
    @DonViTuoi,
    @MoTa,
    @LuuY,
    @TrangThai
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSo(lenh, vaccine);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool CapNhat(Vaccine vaccine)
        {
            const string sql = @"UPDATE Vaccine
SET tenVaccine = @TenVaccine,
    nhomVaccine = @NhomVaccine,
    doTuoiToiThieu = @DoTuoiToiThieu,
    doTuoiToiDa = @DoTuoiToiDa,
    donViTuoi = @DonViTuoi,
    moTa = @MoTa,
    luuY = @LuuY,
    trangThai = @TrangThai
WHERE maVaccine = @MaVaccine";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaVaccine", vaccine.MaVaccine);
            GanThamSo(lenh, vaccine);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool XoaHoacAn(int maVaccine)
        {
            const string sql = @"UPDATE Vaccine
SET trangThai = 0
WHERE maVaccine = @MaVaccine";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaVaccine", maVaccine);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        private List<Vaccine> DocDanhSach(string sql)
        {
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<Vaccine>();
            while (doc.Read())
            {
                danhSach.Add(DocVaccine(doc));
            }

            return danhSach;
        }

        private static void GanThamSo(SqlCommand lenh, Vaccine vaccine)
        {
            lenh.Parameters.AddWithValue("@TenVaccine", vaccine.TenVaccine);
            lenh.Parameters.AddWithValue("@NhomVaccine", string.IsNullOrWhiteSpace(vaccine.NhomVaccine) ? DBNull.Value : vaccine.NhomVaccine);
            lenh.Parameters.AddWithValue("@DoTuoiToiThieu", (object?)vaccine.DoTuoiToiThieu ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@DoTuoiToiDa", (object?)vaccine.DoTuoiToiDa ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@DonViTuoi", string.IsNullOrWhiteSpace(vaccine.DonViTuoi) ? DBNull.Value : vaccine.DonViTuoi);
            lenh.Parameters.AddWithValue("@MoTa", string.IsNullOrWhiteSpace(vaccine.MoTa) ? DBNull.Value : vaccine.MoTa);
            lenh.Parameters.AddWithValue("@LuuY", string.IsNullOrWhiteSpace(vaccine.LuuY) ? DBNull.Value : vaccine.LuuY);
            lenh.Parameters.AddWithValue("@TrangThai", vaccine.TrangThai);
        }

        private static Vaccine DocVaccine(SqlDataReader doc)
        {
            return new Vaccine
            {
                MaVaccine = Convert.ToInt32(doc["maVaccine"]),
                TenVaccine = doc["tenVaccine"] == DBNull.Value ? string.Empty : doc["tenVaccine"].ToString() ?? string.Empty,
                NhomVaccine = doc["nhomVaccine"] == DBNull.Value ? string.Empty : doc["nhomVaccine"].ToString() ?? string.Empty,
                DoTuoiToiThieu = doc["doTuoiToiThieu"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiThieu"]),
                DoTuoiToiDa = doc["doTuoiToiDa"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiDa"]),
                DonViTuoi = doc["donViTuoi"] == DBNull.Value ? string.Empty : doc["donViTuoi"].ToString() ?? string.Empty,
                MoTa = doc["moTa"] == DBNull.Value ? string.Empty : doc["moTa"].ToString() ?? string.Empty,
                LuuY = doc["luuY"] == DBNull.Value ? string.Empty : doc["luuY"].ToString() ?? string.Empty,
                TrangThai = Convert.ToBoolean(doc["trangThai"])
            };
        }
    }
}
