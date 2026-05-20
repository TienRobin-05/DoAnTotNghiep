using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class thongBaoDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public thongBaoDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        public List<thongBaoModels> layTheoTaiKhoan(int maTaiKhoan)
        {
            const string sql = @"select maThongBao, maTaiKhoan, maLichTiem, tieuDe, noiDung, ngayGui, daDoc
from ThongBao where maTaiKhoan = @maTaiKhoan order by ngayGui desc";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<thongBaoModels>();
            while (doc.Read())
            {
                danhSach.Add(new thongBaoModels
                {
                    maThongBao = Convert.ToInt32(doc["maThongBao"]),
                    maTaiKhoan = Convert.ToInt32(doc["maTaiKhoan"]),
                    maLichTiem = doc["maLichTiem"] == DBNull.Value ? null : Convert.ToInt32(doc["maLichTiem"]),
                    tieuDe = doc["tieuDe"].ToString() ?? string.Empty,
                    noiDung = doc["noiDung"] == DBNull.Value ? null : doc["noiDung"].ToString(),
                    ngayGui = Convert.ToDateTime(doc["ngayGui"]),
                    daDoc = Convert.ToBoolean(doc["daDoc"])
                });
            }
            return danhSach;
        }

        public void danhDauDaDoc(int maThongBao, int maTaiKhoan)
        {
            const string sql = "update ThongBao set daDoc = 1 where maThongBao = @maThongBao and maTaiKhoan = @maTaiKhoan";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maThongBao", maThongBao);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }
    }
}
