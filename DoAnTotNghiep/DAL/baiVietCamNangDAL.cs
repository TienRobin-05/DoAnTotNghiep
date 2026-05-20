using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class baiVietCamNangDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public baiVietCamNangDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        public List<baiVietCamNangModels> layTatCa(bool chiLayDangHienThi = false)
        {
            var sql = @"select bv.maBaiViet, bv.maTaiKhoan, bv.tieuDe, bv.noiDung, bv.ngayTao, bv.trangThai, tk.hoTen as tenTacGia
from BaiVietCamNang bv
inner join TaiKhoan tk on bv.maTaiKhoan = tk.maTaiKhoan";
            if (chiLayDangHienThi)
            {
                sql += " where bv.trangThai = 1";
            }
            sql += " order by bv.ngayTao desc";

            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<baiVietCamNangModels>();
            while (doc.Read())
            {
                danhSach.Add(docBaiViet(doc));
            }
            return danhSach;
        }

        public baiVietCamNangModels? layTheoMa(int maBaiViet)
        {
            const string sql = @"select bv.maBaiViet, bv.maTaiKhoan, bv.tieuDe, bv.noiDung, bv.ngayTao, bv.trangThai, tk.hoTen as tenTacGia
from BaiVietCamNang bv
inner join TaiKhoan tk on bv.maTaiKhoan = tk.maTaiKhoan
where bv.maBaiViet = @maBaiViet";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maBaiViet", maBaiViet);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? docBaiViet(doc) : null;
        }

        public void them(baiVietCamNangModels baiViet)
        {
            const string sql = @"insert into BaiVietCamNang(maTaiKhoan, tieuDe, noiDung, ngayTao, trangThai)
values(@maTaiKhoan, @tieuDe, @noiDung, @ngayTao, @trangThai)";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", baiViet.maTaiKhoan);
            lenh.Parameters.AddWithValue("@tieuDe", baiViet.tieuDe);
            lenh.Parameters.AddWithValue("@noiDung", baiViet.noiDung);
            lenh.Parameters.AddWithValue("@ngayTao", DateTime.Now);
            lenh.Parameters.AddWithValue("@trangThai", baiViet.trangThai);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        public void capNhat(baiVietCamNangModels baiViet)
        {
            const string sql = @"update BaiVietCamNang
set tieuDe = @tieuDe, noiDung = @noiDung, trangThai = @trangThai
where maBaiViet = @maBaiViet";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maBaiViet", baiViet.maBaiViet);
            lenh.Parameters.AddWithValue("@tieuDe", baiViet.tieuDe);
            lenh.Parameters.AddWithValue("@noiDung", baiViet.noiDung);
            lenh.Parameters.AddWithValue("@trangThai", baiViet.trangThai);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        private static baiVietCamNangModels docBaiViet(SqlDataReader doc)
        {
            return new baiVietCamNangModels
            {
                maBaiViet = Convert.ToInt32(doc["maBaiViet"]),
                maTaiKhoan = Convert.ToInt32(doc["maTaiKhoan"]),
                tieuDe = doc["tieuDe"].ToString() ?? string.Empty,
                noiDung = doc["noiDung"].ToString() ?? string.Empty,
                ngayTao = Convert.ToDateTime(doc["ngayTao"]),
                trangThai = Convert.ToBoolean(doc["trangThai"]),
                tenTacGia = doc["tenTacGia"].ToString()
            };
        }
    }
}
