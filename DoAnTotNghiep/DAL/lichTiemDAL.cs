using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class lichTiemDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public lichTiemDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        public List<lichTiemModels> layTheoTaiKhoan(int maTaiKhoan)
        {
            const string sql = @"select lt.maLichTiem, lt.maHoSo, lt.maMuiTiem, lt.ngayTiemDuKien, lt.trangThai, lt.ghiChu,
hs.hoTen as tenHoSo, v.tenVaccine, mt.tenMui, mt.soMui
from LichTiem lt
inner join HoSoSucKhoe hs on lt.maHoSo = hs.maHoSo
inner join MuiTiemVaccine mt on lt.maMuiTiem = mt.maMuiTiem
inner join Vaccine v on mt.maVaccine = v.maVaccine
where hs.maTaiKhoan = @maTaiKhoan
order by lt.ngayTiemDuKien";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<lichTiemModels>();
            while (doc.Read())
            {
                danhSach.Add(docLichTiem(doc));
            }
            return danhSach;
        }

        public List<lichTiemModels> layTatCa()
        {
            const string sql = @"select lt.maLichTiem, lt.maHoSo, lt.maMuiTiem, lt.ngayTiemDuKien, lt.trangThai, lt.ghiChu,
hs.hoTen as tenHoSo, v.tenVaccine, mt.tenMui, mt.soMui
from LichTiem lt
inner join HoSoSucKhoe hs on lt.maHoSo = hs.maHoSo
inner join MuiTiemVaccine mt on lt.maMuiTiem = mt.maMuiTiem
inner join Vaccine v on mt.maVaccine = v.maVaccine
order by lt.ngayTiemDuKien desc";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<lichTiemModels>();
            while (doc.Read())
            {
                danhSach.Add(docLichTiem(doc));
            }
            return danhSach;
        }

        public bool lichThuocTaiKhoan(int maLichTiem, int maTaiKhoan)
        {
            const string sql = @"select count(1)
from LichTiem lt inner join HoSoSucKhoe hs on lt.maHoSo = hs.maHoSo
where lt.maLichTiem = @maLichTiem and hs.maTaiKhoan = @maTaiKhoan";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        public void them(lichTiemModels lichTiem)
        {
            const string sql = @"insert into LichTiem(maHoSo, maMuiTiem, ngayTiemDuKien, trangThai, ghiChu)
values(@maHoSo, @maMuiTiem, @ngayTiemDuKien, @trangThai, @ghiChu)";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSo(lenh, lichTiem);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        public void capNhatTrangThai(int maLichTiem, string trangThai)
        {
            const string sql = "update LichTiem set trangThai = @trangThai where maLichTiem = @maLichTiem";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@trangThai", trangThai);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        private static void ganThamSo(SqlCommand lenh, lichTiemModels lichTiem)
        {
            lenh.Parameters.AddWithValue("@maHoSo", lichTiem.maHoSo);
            lenh.Parameters.AddWithValue("@maMuiTiem", lichTiem.maMuiTiem);
            lenh.Parameters.AddWithValue("@ngayTiemDuKien", lichTiem.ngayTiemDuKien.Date);
            lenh.Parameters.AddWithValue("@trangThai", lichTiem.trangThai);
            lenh.Parameters.AddWithValue("@ghiChu", (object?)lichTiem.ghiChu ?? DBNull.Value);
        }

        private static lichTiemModels docLichTiem(SqlDataReader doc)
        {
            return new lichTiemModels
            {
                maLichTiem = Convert.ToInt32(doc["maLichTiem"]),
                maHoSo = Convert.ToInt32(doc["maHoSo"]),
                maMuiTiem = Convert.ToInt32(doc["maMuiTiem"]),
                ngayTiemDuKien = Convert.ToDateTime(doc["ngayTiemDuKien"]),
                trangThai = doc["trangThai"].ToString() ?? string.Empty,
                ghiChu = doc["ghiChu"] == DBNull.Value ? null : doc["ghiChu"].ToString(),
                tenHoSo = doc["tenHoSo"].ToString(),
                tenVaccine = doc["tenVaccine"].ToString(),
                tenMui = doc["tenMui"] == DBNull.Value ? null : doc["tenMui"].ToString(),
                soMui = Convert.ToInt32(doc["soMui"])
            };
        }
    }
}
