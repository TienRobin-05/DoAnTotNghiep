using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class cauHoiTuVanDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public cauHoiTuVanDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        public List<cauHoiTuVanModels> layTatCa()
        {
            const string sql = @"select ch.maCauHoi, ch.maNguoiGui, ch.maNguoiTraLoi, ch.cauHoi, ch.cauTraLoi,
ch.ngayGui, ch.ngayTraLoi, ch.trangThai, gui.hoTen as tenNguoiGui, traLoi.hoTen as tenNguoiTraLoi
from CauHoiTuVan ch
inner join TaiKhoan gui on ch.maNguoiGui = gui.maTaiKhoan
left join TaiKhoan traLoi on ch.maNguoiTraLoi = traLoi.maTaiKhoan
order by ch.ngayGui desc";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<cauHoiTuVanModels>();
            while (doc.Read())
            {
                danhSach.Add(docCauHoi(doc));
            }
            return danhSach;
        }

        public List<cauHoiTuVanModels> layTheoNguoiGui(int maNguoiGui)
        {
            const string sql = @"select ch.maCauHoi, ch.maNguoiGui, ch.maNguoiTraLoi, ch.cauHoi, ch.cauTraLoi,
ch.ngayGui, ch.ngayTraLoi, ch.trangThai, gui.hoTen as tenNguoiGui, traLoi.hoTen as tenNguoiTraLoi
from CauHoiTuVan ch
inner join TaiKhoan gui on ch.maNguoiGui = gui.maTaiKhoan
left join TaiKhoan traLoi on ch.maNguoiTraLoi = traLoi.maTaiKhoan
where ch.maNguoiGui = @maNguoiGui
order by ch.ngayGui desc";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maNguoiGui", maNguoiGui);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<cauHoiTuVanModels>();
            while (doc.Read())
            {
                danhSach.Add(docCauHoi(doc));
            }
            return danhSach;
        }

        public cauHoiTuVanModels? layTheoMa(int maCauHoi)
        {
            const string sql = @"select ch.maCauHoi, ch.maNguoiGui, ch.maNguoiTraLoi, ch.cauHoi, ch.cauTraLoi,
ch.ngayGui, ch.ngayTraLoi, ch.trangThai, gui.hoTen as tenNguoiGui, traLoi.hoTen as tenNguoiTraLoi
from CauHoiTuVan ch
inner join TaiKhoan gui on ch.maNguoiGui = gui.maTaiKhoan
left join TaiKhoan traLoi on ch.maNguoiTraLoi = traLoi.maTaiKhoan
where ch.maCauHoi = @maCauHoi";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maCauHoi", maCauHoi);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? docCauHoi(doc) : null;
        }

        public void them(cauHoiTuVanModels cauHoi)
        {
            const string sql = @"insert into CauHoiTuVan(maNguoiGui, maNguoiTraLoi, cauHoi, cauTraLoi, ngayGui, ngayTraLoi, trangThai)
values(@maNguoiGui, null, @cauHoi, null, @ngayGui, null, @trangThai)";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maNguoiGui", cauHoi.maNguoiGui);
            lenh.Parameters.AddWithValue("@cauHoi", cauHoi.cauHoi);
            lenh.Parameters.AddWithValue("@ngayGui", DateTime.Now);
            lenh.Parameters.AddWithValue("@trangThai", "Chờ trả lời");
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        public void traLoi(int maCauHoi, int maNguoiTraLoi, string cauTraLoi)
        {
            const string sql = @"update CauHoiTuVan
set maNguoiTraLoi = @maNguoiTraLoi, cauTraLoi = @cauTraLoi, ngayTraLoi = @ngayTraLoi, trangThai = @trangThai
where maCauHoi = @maCauHoi";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maCauHoi", maCauHoi);
            lenh.Parameters.AddWithValue("@maNguoiTraLoi", maNguoiTraLoi);
            lenh.Parameters.AddWithValue("@cauTraLoi", cauTraLoi);
            lenh.Parameters.AddWithValue("@ngayTraLoi", DateTime.Now);
            lenh.Parameters.AddWithValue("@trangThai", "Đã trả lời");
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        private static cauHoiTuVanModels docCauHoi(SqlDataReader doc)
        {
            return new cauHoiTuVanModels
            {
                maCauHoi = Convert.ToInt32(doc["maCauHoi"]),
                maNguoiGui = Convert.ToInt32(doc["maNguoiGui"]),
                maNguoiTraLoi = doc["maNguoiTraLoi"] == DBNull.Value ? null : Convert.ToInt32(doc["maNguoiTraLoi"]),
                cauHoi = doc["cauHoi"].ToString() ?? string.Empty,
                cauTraLoi = doc["cauTraLoi"] == DBNull.Value ? null : doc["cauTraLoi"].ToString(),
                ngayGui = Convert.ToDateTime(doc["ngayGui"]),
                ngayTraLoi = doc["ngayTraLoi"] == DBNull.Value ? null : Convert.ToDateTime(doc["ngayTraLoi"]),
                trangThai = doc["trangThai"].ToString() ?? string.Empty,
                tenNguoiGui = doc["tenNguoiGui"].ToString(),
                tenNguoiTraLoi = doc["tenNguoiTraLoi"] == DBNull.Value ? null : doc["tenNguoiTraLoi"].ToString()
            };
        }
    }
}
