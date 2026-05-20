using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class muiTiemVaccineDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public muiTiemVaccineDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        public List<muiTiemVaccineModels> layTatCa()
        {
            const string sql = @"select mt.maMuiTiem, mt.maVaccine, mt.soMui, mt.tenMui, mt.doTuoiToiThieu, mt.doTuoiToiDa,
mt.doTuoiKhuyenNghi, mt.donViTuoi, mt.khoangCachNgay, mt.ghiChu, v.tenVaccine
from MuiTiemVaccine mt
inner join Vaccine v on mt.maVaccine = v.maVaccine
order by mt.maMuiTiem desc";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<muiTiemVaccineModels>();
            while (doc.Read())
            {
                danhSach.Add(docMuiTiem(doc));
            }
            return danhSach;
        }

        public muiTiemVaccineModels? layTheoMa(int maMuiTiem)
        {
            const string sql = @"select mt.maMuiTiem, mt.maVaccine, mt.soMui, mt.tenMui, mt.doTuoiToiThieu, mt.doTuoiToiDa,
mt.doTuoiKhuyenNghi, mt.donViTuoi, mt.khoangCachNgay, mt.ghiChu, v.tenVaccine
from MuiTiemVaccine mt
inner join Vaccine v on mt.maVaccine = v.maVaccine
where mt.maMuiTiem = @maMuiTiem";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maMuiTiem", maMuiTiem);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? docMuiTiem(doc) : null;
        }

        public void them(muiTiemVaccineModels muiTiem)
        {
            const string sql = @"insert into MuiTiemVaccine(maVaccine, soMui, tenMui, doTuoiToiThieu, doTuoiToiDa, doTuoiKhuyenNghi, donViTuoi, khoangCachNgay, ghiChu)
values(@maVaccine, @soMui, @tenMui, @doTuoiToiThieu, @doTuoiToiDa, @doTuoiKhuyenNghi, @donViTuoi, @khoangCachNgay, @ghiChu)";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSo(lenh, muiTiem);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        public void capNhat(muiTiemVaccineModels muiTiem)
        {
            const string sql = @"update MuiTiemVaccine
set maVaccine = @maVaccine, soMui = @soMui, tenMui = @tenMui, doTuoiToiThieu = @doTuoiToiThieu,
    doTuoiToiDa = @doTuoiToiDa, doTuoiKhuyenNghi = @doTuoiKhuyenNghi, donViTuoi = @donViTuoi,
    khoangCachNgay = @khoangCachNgay, ghiChu = @ghiChu
where maMuiTiem = @maMuiTiem";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSo(lenh, muiTiem);
            lenh.Parameters.AddWithValue("@maMuiTiem", muiTiem.maMuiTiem);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        private static void ganThamSo(SqlCommand lenh, muiTiemVaccineModels muiTiem)
        {
            lenh.Parameters.AddWithValue("@maVaccine", muiTiem.maVaccine);
            lenh.Parameters.AddWithValue("@soMui", muiTiem.soMui);
            lenh.Parameters.AddWithValue("@tenMui", (object?)muiTiem.tenMui ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@doTuoiToiThieu", (object?)muiTiem.doTuoiToiThieu ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@doTuoiToiDa", (object?)muiTiem.doTuoiToiDa ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@doTuoiKhuyenNghi", (object?)muiTiem.doTuoiKhuyenNghi ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@donViTuoi", (object?)muiTiem.donViTuoi ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@khoangCachNgay", (object?)muiTiem.khoangCachNgay ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@ghiChu", (object?)muiTiem.ghiChu ?? DBNull.Value);
        }

        private static muiTiemVaccineModels docMuiTiem(SqlDataReader doc)
        {
            return new muiTiemVaccineModels
            {
                maMuiTiem = Convert.ToInt32(doc["maMuiTiem"]),
                maVaccine = Convert.ToInt32(doc["maVaccine"]),
                soMui = Convert.ToInt32(doc["soMui"]),
                tenMui = doc["tenMui"] == DBNull.Value ? null : doc["tenMui"].ToString(),
                doTuoiToiThieu = doc["doTuoiToiThieu"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiThieu"]),
                doTuoiToiDa = doc["doTuoiToiDa"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiDa"]),
                doTuoiKhuyenNghi = doc["doTuoiKhuyenNghi"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiKhuyenNghi"]),
                donViTuoi = doc["donViTuoi"] == DBNull.Value ? null : doc["donViTuoi"].ToString(),
                khoangCachNgay = doc["khoangCachNgay"] == DBNull.Value ? null : Convert.ToInt32(doc["khoangCachNgay"]),
                ghiChu = doc["ghiChu"] == DBNull.Value ? null : doc["ghiChu"].ToString(),
                tenVaccine = doc["tenVaccine"].ToString()
            };
        }
    }
}
