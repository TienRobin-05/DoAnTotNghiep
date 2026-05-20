using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class LichTiem_DAL
    {
        private readonly string chuoiKetNoi;

        public LichTiem_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public List<LichTiem> LayDanhSachTheoHoSo(int maHoSo, int maTaiKhoan)
        {
            const string sql = @"SELECT lt.maLichTiem, lt.maHoSo, lt.maMuiTiem, lt.ngayTiemDuKien, lt.trangThai, lt.ghiChu,
hs.hoTen AS hoTenHoSo, v.tenVaccine, mt.tenMui, mt.soMui
FROM LichTiem lt
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE lt.maHoSo = @MaHoSo AND hs.maTaiKhoan = @MaTaiKhoan
ORDER BY lt.ngayTiemDuKien, v.tenVaccine, mt.soMui";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<LichTiem>();
            while (doc.Read())
            {
                danhSach.Add(DocLichTiem(doc));
            }
            return danhSach;
        }

        public bool KiemTraHoSoCoLichTiem(int maHoSo)
        {
            const string sql = "SELECT COUNT(*) FROM LichTiem WHERE maHoSo = @MaHoSo";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        public bool Them(LichTiem lich)
        {
            const string sql = @"INSERT INTO LichTiem(maHoSo, maMuiTiem, ngayTiemDuKien, trangThai, ghiChu)
VALUES(@MaHoSo, @MaMuiTiem, @NgayTiemDuKien, @TrangThai, @GhiChu)";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", lich.MaHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", lich.MaMuiTiem);
            lenh.Parameters.AddWithValue("@NgayTiemDuKien", lich.NgayTiemDuKien.Date);
            lenh.Parameters.AddWithValue("@TrangThai", lich.TrangThai);
            lenh.Parameters.AddWithValue("@GhiChu", string.IsNullOrWhiteSpace(lich.GhiChu) ? DBNull.Value : lich.GhiChu);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public LichTiem? LayTheoId(int maLichTiem)
        {
            const string sql = @"SELECT lt.maLichTiem, lt.maHoSo, lt.maMuiTiem, lt.ngayTiemDuKien, lt.trangThai, lt.ghiChu,
hs.hoTen AS hoTenHoSo, v.tenVaccine, mt.tenMui, mt.soMui
FROM LichTiem lt
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE lt.maLichTiem = @MaLichTiem";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaLichTiem", maLichTiem);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocLichTiem(doc) : null;
        }

        public bool CapNhatTrangThai(int maLichTiem, string trangThai)
        {
            const string sql = "UPDATE LichTiem SET trangThai = @TrangThai WHERE maLichTiem = @MaLichTiem";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@TrangThai", trangThai);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool KiemTraLichTonTai(int maHoSo, int maMuiTiem)
        {
            const string sql = "SELECT COUNT(*) FROM LichTiem WHERE maHoSo = @MaHoSo AND maMuiTiem = @MaMuiTiem";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        private static LichTiem DocLichTiem(SqlDataReader doc)
        {
            return new LichTiem
            {
                MaLichTiem = Convert.ToInt32(doc["maLichTiem"]),
                MaHoSo = Convert.ToInt32(doc["maHoSo"]),
                MaMuiTiem = Convert.ToInt32(doc["maMuiTiem"]),
                NgayTiemDuKien = Convert.ToDateTime(doc["ngayTiemDuKien"]),
                TrangThai = doc["trangThai"] == DBNull.Value ? string.Empty : doc["trangThai"].ToString() ?? string.Empty,
                GhiChu = doc["ghiChu"] == DBNull.Value ? string.Empty : doc["ghiChu"].ToString() ?? string.Empty,
                HoTenHoSo = doc["hoTenHoSo"] == DBNull.Value ? string.Empty : doc["hoTenHoSo"].ToString() ?? string.Empty,
                TenVaccine = doc["tenVaccine"] == DBNull.Value ? string.Empty : doc["tenVaccine"].ToString() ?? string.Empty,
                TenMui = doc["tenMui"] == DBNull.Value ? string.Empty : doc["tenMui"].ToString() ?? string.Empty,
                SoMui = Convert.ToInt32(doc["soMui"])
            };
        }
    }
}
