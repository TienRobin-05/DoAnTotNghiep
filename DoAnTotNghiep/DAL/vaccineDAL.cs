using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class vaccineDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public vaccineDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        public List<vaccineModels> layTatCa(bool chiLayDangHoatDong = false)
        {
            var sql = @"select maVaccine, tenVaccine, nhomVaccine, doTuoiToiThieu, doTuoiToiDa, donViTuoi, moTa, luuY, trangThai
from Vaccine";
            if (chiLayDangHoatDong)
            {
                sql += " where trangThai = 1";
            }
            sql += " order by maVaccine desc";

            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<vaccineModels>();
            while (doc.Read())
            {
                danhSach.Add(docVaccine(doc));
            }
            return danhSach;
        }

        public vaccineModels? layTheoMa(int maVaccine)
        {
            const string sql = @"select maVaccine, tenVaccine, nhomVaccine, doTuoiToiThieu, doTuoiToiDa, donViTuoi, moTa, luuY, trangThai
from Vaccine where maVaccine = @maVaccine";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maVaccine", maVaccine);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? docVaccine(doc) : null;
        }

        public void them(vaccineModels vaccine)
        {
            const string sql = @"insert into Vaccine(tenVaccine, nhomVaccine, doTuoiToiThieu, doTuoiToiDa, donViTuoi, moTa, luuY, trangThai)
values(@tenVaccine, @nhomVaccine, @doTuoiToiThieu, @doTuoiToiDa, @donViTuoi, @moTa, @luuY, @trangThai)";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSo(lenh, vaccine);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        public void capNhat(vaccineModels vaccine)
        {
            const string sql = @"update Vaccine
set tenVaccine = @tenVaccine, nhomVaccine = @nhomVaccine, doTuoiToiThieu = @doTuoiToiThieu,
    doTuoiToiDa = @doTuoiToiDa, donViTuoi = @donViTuoi, moTa = @moTa, luuY = @luuY, trangThai = @trangThai
where maVaccine = @maVaccine";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSo(lenh, vaccine);
            lenh.Parameters.AddWithValue("@maVaccine", vaccine.maVaccine);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        private static void ganThamSo(SqlCommand lenh, vaccineModels vaccine)
        {
            lenh.Parameters.AddWithValue("@tenVaccine", vaccine.tenVaccine);
            lenh.Parameters.AddWithValue("@nhomVaccine", (object?)vaccine.nhomVaccine ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@doTuoiToiThieu", (object?)vaccine.doTuoiToiThieu ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@doTuoiToiDa", (object?)vaccine.doTuoiToiDa ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@donViTuoi", (object?)vaccine.donViTuoi ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@moTa", (object?)vaccine.moTa ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@luuY", (object?)vaccine.luuY ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@trangThai", vaccine.trangThai);
        }

        private static vaccineModels docVaccine(SqlDataReader doc)
        {
            return new vaccineModels
            {
                maVaccine = Convert.ToInt32(doc["maVaccine"]),
                tenVaccine = doc["tenVaccine"].ToString() ?? string.Empty,
                nhomVaccine = doc["nhomVaccine"] == DBNull.Value ? null : doc["nhomVaccine"].ToString(),
                doTuoiToiThieu = doc["doTuoiToiThieu"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiThieu"]),
                doTuoiToiDa = doc["doTuoiToiDa"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiDa"]),
                donViTuoi = doc["donViTuoi"] == DBNull.Value ? null : doc["donViTuoi"].ToString(),
                moTa = doc["moTa"] == DBNull.Value ? null : doc["moTa"].ToString(),
                luuY = doc["luuY"] == DBNull.Value ? null : doc["luuY"].ToString(),
                trangThai = Convert.ToBoolean(doc["trangThai"])
            };
        }
    }
}
