using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class lichSuTiemDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public lichSuTiemDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        public List<lichSuTiemModels> layTheoTaiKhoan(int maTaiKhoan)
        {
            const string sql = @"select lst.maLichSu, lst.maLichTiem, lst.ngayTiemThucTe, lst.ghiChu, lst.ngayCapNhat,
hs.hoTen as tenHoSo, v.tenVaccine, mt.tenMui
from LichSuTiem lst
inner join LichTiem lt on lst.maLichTiem = lt.maLichTiem
inner join HoSoSucKhoe hs on lt.maHoSo = hs.maHoSo
inner join MuiTiemVaccine mt on lt.maMuiTiem = mt.maMuiTiem
inner join Vaccine v on mt.maVaccine = v.maVaccine
where hs.maTaiKhoan = @maTaiKhoan
order by lst.ngayTiemThucTe desc";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<lichSuTiemModels>();
            while (doc.Read())
            {
                danhSach.Add(new lichSuTiemModels
                {
                    maLichSu = Convert.ToInt32(doc["maLichSu"]),
                    maLichTiem = Convert.ToInt32(doc["maLichTiem"]),
                    ngayTiemThucTe = Convert.ToDateTime(doc["ngayTiemThucTe"]),
                    ghiChu = doc["ghiChu"] == DBNull.Value ? null : doc["ghiChu"].ToString(),
                    ngayCapNhat = Convert.ToDateTime(doc["ngayCapNhat"]),
                    tenHoSo = doc["tenHoSo"].ToString(),
                    tenVaccine = doc["tenVaccine"].ToString(),
                    tenMui = doc["tenMui"] == DBNull.Value ? null : doc["tenMui"].ToString()
                });
            }
            return danhSach;
        }

        public void them(lichSuTiemModels lichSu)
        {
            const string sql = @"insert into LichSuTiem(maLichTiem, ngayTiemThucTe, ghiChu, ngayCapNhat)
values(@maLichTiem, @ngayTiemThucTe, @ghiChu, @ngayCapNhat)";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maLichTiem", lichSu.maLichTiem);
            lenh.Parameters.AddWithValue("@ngayTiemThucTe", lichSu.ngayTiemThucTe.Date);
            lenh.Parameters.AddWithValue("@ghiChu", (object?)lichSu.ghiChu ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@ngayCapNhat", DateTime.Now);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }
    }
}
