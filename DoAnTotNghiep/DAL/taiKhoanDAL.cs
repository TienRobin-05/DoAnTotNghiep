using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class taiKhoanDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public taiKhoanDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        public taiKhoanModels? dangNhap(string email, string matKhau)
        {
            const string sql = @"select maTaiKhoan, hoTen, email, matKhau, soDienThoai, vaiTro, trangThai, ngayTao
from TaiKhoan
where email = @email and matKhau = @matKhau and trangThai = 1";

            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@email", email);
            lenh.Parameters.AddWithValue("@matKhau", matKhau);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? docTaiKhoan(doc) : null;
        }

        public bool emailDaTonTai(string email)
        {
            const string sql = "select count(1) from TaiKhoan where email = @email";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@email", email);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        public int them(taiKhoanModels taiKhoan)
        {
            const string sql = @"insert into TaiKhoan(hoTen, email, matKhau, soDienThoai, vaiTro, trangThai, ngayTao)
values(@hoTen, @email, @matKhau, @soDienThoai, @vaiTro, @trangThai, @ngayTao);
select cast(scope_identity() as int);";

            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSoTaiKhoan(lenh, taiKhoan, themMoi: true);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }

        public List<taiKhoanModels> layTatCa()
        {
            const string sql = @"select maTaiKhoan, hoTen, email, matKhau, soDienThoai, vaiTro, trangThai, ngayTao
from TaiKhoan
order by maTaiKhoan desc";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<taiKhoanModels>();
            while (doc.Read())
            {
                danhSach.Add(docTaiKhoan(doc));
            }
            return danhSach;
        }

        public taiKhoanModels? layTheoMa(int maTaiKhoan)
        {
            const string sql = @"select maTaiKhoan, hoTen, email, matKhau, soDienThoai, vaiTro, trangThai, ngayTao
from TaiKhoan where maTaiKhoan = @maTaiKhoan";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? docTaiKhoan(doc) : null;
        }

        public void capNhat(taiKhoanModels taiKhoan)
        {
            const string sql = @"update TaiKhoan
set hoTen = @hoTen, email = @email, matKhau = @matKhau, soDienThoai = @soDienThoai,
    vaiTro = @vaiTro, trangThai = @trangThai
where maTaiKhoan = @maTaiKhoan";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSoTaiKhoan(lenh, taiKhoan, themMoi: false);
            lenh.Parameters.AddWithValue("@maTaiKhoan", taiKhoan.maTaiKhoan);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        public void doiTrangThai(int maTaiKhoan, bool trangThai)
        {
            const string sql = "update TaiKhoan set trangThai = @trangThai where maTaiKhoan = @maTaiKhoan";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@trangThai", trangThai);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        private static void ganThamSoTaiKhoan(SqlCommand lenh, taiKhoanModels taiKhoan, bool themMoi)
        {
            lenh.Parameters.AddWithValue("@hoTen", taiKhoan.hoTen);
            lenh.Parameters.AddWithValue("@email", taiKhoan.email);
            lenh.Parameters.AddWithValue("@matKhau", taiKhoan.matKhau);
            lenh.Parameters.AddWithValue("@soDienThoai", (object?)taiKhoan.soDienThoai ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@vaiTro", taiKhoan.vaiTro);
            lenh.Parameters.AddWithValue("@trangThai", taiKhoan.trangThai);
            if (themMoi)
            {
                lenh.Parameters.AddWithValue("@ngayTao", DateTime.Now);
            }
        }

        private static taiKhoanModels docTaiKhoan(SqlDataReader doc)
        {
            return new taiKhoanModels
            {
                maTaiKhoan = Convert.ToInt32(doc["maTaiKhoan"]),
                hoTen = doc["hoTen"].ToString() ?? string.Empty,
                email = doc["email"].ToString() ?? string.Empty,
                matKhau = doc["matKhau"].ToString() ?? string.Empty,
                soDienThoai = doc["soDienThoai"] == DBNull.Value ? null : doc["soDienThoai"].ToString(),
                vaiTro = doc["vaiTro"].ToString() ?? string.Empty,
                trangThai = Convert.ToBoolean(doc["trangThai"]),
                ngayTao = Convert.ToDateTime(doc["ngayTao"])
            };
        }
    }
}
