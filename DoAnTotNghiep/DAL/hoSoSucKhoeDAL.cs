using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class hoSoSucKhoeDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public hoSoSucKhoeDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        public List<hoSoSucKhoeModels> layTheoTaiKhoan(int maTaiKhoan)
        {
            const string sql = @"select maHoSo, maTaiKhoan, hoTen, ngaySinh, gioiTinh, chieuCao, canNang, tienSuBenh, diUng, ngayTao
from HoSoSucKhoe where maTaiKhoan = @maTaiKhoan order by maHoSo desc";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<hoSoSucKhoeModels>();
            while (doc.Read())
            {
                danhSach.Add(docHoSo(doc));
            }
            return danhSach;
        }

        public hoSoSucKhoeModels? layTheoMa(int maHoSo, int maTaiKhoan)
        {
            const string sql = @"select maHoSo, maTaiKhoan, hoTen, ngaySinh, gioiTinh, chieuCao, canNang, tienSuBenh, diUng, ngayTao
from HoSoSucKhoe where maHoSo = @maHoSo and maTaiKhoan = @maTaiKhoan";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? docHoSo(doc) : null;
        }

        public void them(hoSoSucKhoeModels hoSo)
        {
            const string sql = @"insert into HoSoSucKhoe(maTaiKhoan, hoTen, ngaySinh, gioiTinh, chieuCao, canNang, tienSuBenh, diUng, ngayTao)
values(@maTaiKhoan, @hoTen, @ngaySinh, @gioiTinh, @chieuCao, @canNang, @tienSuBenh, @diUng, @ngayTao)";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSo(lenh, hoSo, themMoi: true);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        public void capNhat(hoSoSucKhoeModels hoSo)
        {
            const string sql = @"update HoSoSucKhoe
set hoTen = @hoTen, ngaySinh = @ngaySinh, gioiTinh = @gioiTinh, chieuCao = @chieuCao,
    canNang = @canNang, tienSuBenh = @tienSuBenh, diUng = @diUng
where maHoSo = @maHoSo and maTaiKhoan = @maTaiKhoan";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSo(lenh, hoSo, themMoi: false);
            lenh.Parameters.AddWithValue("@maHoSo", hoSo.maHoSo);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        public void xoa(int maHoSo, int maTaiKhoan)
        {
            const string sql = "delete from HoSoSucKhoe where maHoSo = @maHoSo and maTaiKhoan = @maTaiKhoan";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        private static void ganThamSo(SqlCommand lenh, hoSoSucKhoeModels hoSo, bool themMoi)
        {
            lenh.Parameters.AddWithValue("@maTaiKhoan", hoSo.maTaiKhoan);
            lenh.Parameters.AddWithValue("@hoTen", hoSo.hoTen);
            lenh.Parameters.AddWithValue("@ngaySinh", hoSo.ngaySinh.Date);
            lenh.Parameters.AddWithValue("@gioiTinh", (object?)hoSo.gioiTinh ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@chieuCao", (object?)hoSo.chieuCao ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@canNang", (object?)hoSo.canNang ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@tienSuBenh", (object?)hoSo.tienSuBenh ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@diUng", (object?)hoSo.diUng ?? DBNull.Value);
            if (themMoi)
            {
                lenh.Parameters.AddWithValue("@ngayTao", DateTime.Now);
            }
        }

        private static hoSoSucKhoeModels docHoSo(SqlDataReader doc)
        {
            return new hoSoSucKhoeModels
            {
                maHoSo = Convert.ToInt32(doc["maHoSo"]),
                maTaiKhoan = Convert.ToInt32(doc["maTaiKhoan"]),
                hoTen = doc["hoTen"].ToString() ?? string.Empty,
                ngaySinh = Convert.ToDateTime(doc["ngaySinh"]),
                gioiTinh = doc["gioiTinh"] == DBNull.Value ? null : doc["gioiTinh"].ToString(),
                chieuCao = doc["chieuCao"] == DBNull.Value ? null : Convert.ToDouble(doc["chieuCao"]),
                canNang = doc["canNang"] == DBNull.Value ? null : Convert.ToDouble(doc["canNang"]),
                tienSuBenh = doc["tienSuBenh"] == DBNull.Value ? null : doc["tienSuBenh"].ToString(),
                diUng = doc["diUng"] == DBNull.Value ? null : doc["diUng"].ToString(),
                ngayTao = Convert.ToDateTime(doc["ngayTao"])
            };
        }
    }
}
