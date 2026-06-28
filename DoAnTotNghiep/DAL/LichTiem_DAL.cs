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

        // lấy lịch tiêm theo hồ sơ
        public List<LichTiem> LayDanhSachTheoHoSo(int maHoSo, int maTaiKhoan)
        {
            const string sql = @"SELECT lt.maLichTiem, lt.maHoSo, lt.maMuiTiem, lt.ngayTiemDuKien, lt.trangThai, lt.ghiChu,
 hs.hoTen AS hoTenHoSo, v.tenVaccine, v.nhomVaccine, mt.tenMui, mt.soMui
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

        // Lấy tất cả lịch tiêm của một tài khoản trong một lần truy vấn, tránh gọi database lặp theo từng hồ sơ.
        public List<LichTiem> LayDanhSachTheoTaiKhoan(int maTaiKhoan)
        {
            const string sql = @"SELECT lt.maLichTiem, lt.maHoSo, lt.maMuiTiem, lt.ngayTiemDuKien, lt.trangThai, lt.ghiChu,
hs.hoTen AS hoTenHoSo, v.tenVaccine, v.nhomVaccine, mt.tenMui, mt.soMui
FROM LichTiem lt
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE hs.maTaiKhoan = @MaTaiKhoan
ORDER BY lt.ngayTiemDuKien, v.tenVaccine, mt.soMui";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
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

        // đếm tổng lịch tiêm
        public int DemTatCa()
        {
            const string sql = "SELECT COUNT(*) FROM LichTiem";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }

        // đếm số mũi đã tiêm
        public int DemDaTiem()
        {
            const string sql = "SELECT COUNT(*) FROM LichTiem WHERE trangThai = N'Đã tiêm'";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }

        public int DemSapToi()
        {
            const string sql = @"SELECT COUNT(*)
FROM LichTiem
WHERE CONVERT(date, ngayTiemDuKien) >= CONVERT(date, GETDATE())
AND ISNULL(trangThai, N'') <> N'Đã tiêm'";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }

        // kiểm tra hồ sơ có lịch tiêm chưa
        public bool KiemTraHoSoCoLichTiem(int maHoSo)
        {
            const string sql = "SELECT COUNT(*) FROM LichTiem WHERE maHoSo = @MaHoSo";
                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
                        ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // thêm lịch tiêm
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
hs.hoTen AS hoTenHoSo, v.tenVaccine, v.nhomVaccine, mt.tenMui, mt.soMui
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

// lấy chi tiết lịch tiêm kèm kiểm tra quyền
        public LichTiem? LayChiTietCoKiemTraChuSoHuu(int maLichTiem, int maTaiKhoan)
        {
            const string sql = @"SELECT lt.maLichTiem, lt.maHoSo, lt.maMuiTiem, lt.ngayTiemDuKien, lt.trangThai, lt.ghiChu,
hs.hoTen AS hoTenHoSo, hs.ngaySinh AS ngaySinhHoSo, v.tenVaccine, v.nhomVaccine, mt.tenMui, mt.soMui
FROM LichTiem lt
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE lt.maLichTiem = @MaLichTiem
AND hs.maTaiKhoan = @MaTaiKhoan";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

                        ketNoi.Open();
                        using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocLichTiem(doc) : null;
        }

        public bool CapNhatDaTiem(int maLichTiem)
        {
            const string sql = "UPDATE LichTiem SET trangThai = @TrangThai WHERE maLichTiem = @MaLichTiem";

                        using var ketNoi = new SqlConnection(chuoiKetNoi);
                        using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@TrangThai", "Đã tiêm");

                        ketNoi.Open();
                        return lenh.ExecuteNonQuery() > 0;
        }

        // cập nhật đã tiêm và ghi lịch sử
        public bool CapNhatDaTiemVaGhiLichSu(LichSuTiem lichSu, int maTaiKhoan)
        {
            const string sqlKhoaLich = @"SELECT TOP 1
    lt.maLichTiem,
    lt.trangThai,
    lt.ngayTiemDuKien,
    hs.hoTen,
    v.tenVaccine,
    mt.tenMui,
    mt.soMui
FROM LichTiem lt WITH (UPDLOCK, HOLDLOCK)
INNER JOIN HoSoSucKhoe hs ON lt.maHoSo = hs.maHoSo
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE lt.maLichTiem = @MaLichTiem
AND hs.maTaiKhoan = @MaTaiKhoan";

            const string sqlKiemTraLichSu = "SELECT COUNT(*) FROM LichSuTiem WITH (UPDLOCK, HOLDLOCK) WHERE maLichTiem = @MaLichTiem";
            const string sqlCapNhatLich = "UPDATE LichTiem SET trangThai = @TrangThai WHERE maLichTiem = @MaLichTiem";
            const string sqlThemLichSu = @"INSERT INTO LichSuTiem(maLichTiem, ngayTiemThucTe, ghiChu, ngayCapNhat)
VALUES(@MaLichTiem, @NgayTiemThucTe, @GhiChu, @NgayCapNhat)";
            const string sqlTimThongBaoCapNhat = @"SELECT TOP 1 maThongBao
FROM ThongBao WITH (UPDLOCK, HOLDLOCK)
WHERE maTaiKhoan = @MaTaiKhoan
AND maLichTiem = @MaLichTiem
AND tieuDe = @TieuDeDaCapNhat
ORDER BY ngayGui DESC, maThongBao DESC";
            const string sqlTimThongBaoNhacLich = @"SELECT TOP 1 maThongBao
FROM ThongBao WITH (UPDLOCK, HOLDLOCK)
WHERE maTaiKhoan = @MaTaiKhoan
AND maLichTiem = @MaLichTiem
AND tieuDe IN (@TieuDeSapDen, @TieuDeHomNay, @TieuDeQuaHan)
ORDER BY ngayGui DESC, maThongBao DESC";
            const string sqlChuyenThongBao = @"UPDATE ThongBao
SET tieuDe = @TieuDeDaCapNhat,
    noiDung = @NoiDungDaCapNhat,
    ngayGui = @NgayGui,
    daDoc = 0
WHERE maThongBao = @MaThongBao
AND maTaiKhoan = @MaTaiKhoan";
            const string sqlThemThongBao = @"INSERT INTO ThongBao(maTaiKhoan, maLichTiem, tieuDe, noiDung, ngayGui, daDoc)
VALUES(@MaTaiKhoan, @MaLichTiem, @TieuDeDaCapNhat, @NoiDungDaCapNhat, @NgayGui, 0)";
            const string sqlXoaThongBaoNhacLichConLai = @"DELETE FROM ThongBao
WHERE maTaiKhoan = @MaTaiKhoan
AND maLichTiem = @MaLichTiem
AND tieuDe IN (@TieuDeSapDen, @TieuDeHomNay, @TieuDeQuaHan)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            ketNoi.Open();
            using var giaoDich = ketNoi.BeginTransaction();

            try
            {
                string hoTenHoSo;
                string tenVaccine;
                string tenMui;
                int soMui;

                using (var lenh = new SqlCommand(sqlKhoaLich, ketNoi, giaoDich))
                {
                    lenh.Parameters.AddWithValue("@MaLichTiem", lichSu.MaLichTiem);
                    lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
                    using var doc = lenh.ExecuteReader();
                    if (!doc.Read())
                    {
                        giaoDich.Rollback();
                        return false;
                    }

                    hoTenHoSo = doc["hoTen"] == DBNull.Value ? string.Empty : doc["hoTen"].ToString() ?? string.Empty;
                    tenVaccine = doc["tenVaccine"] == DBNull.Value ? string.Empty : doc["tenVaccine"].ToString() ?? string.Empty;
                    tenMui = doc["tenMui"] == DBNull.Value ? string.Empty : doc["tenMui"].ToString() ?? string.Empty;
                    soMui = doc["soMui"] == DBNull.Value ? 0 : Convert.ToInt32(doc["soMui"]);
                }

                using (var lenh = new SqlCommand(sqlKiemTraLichSu, ketNoi, giaoDich))
                {
                    lenh.Parameters.AddWithValue("@MaLichTiem", lichSu.MaLichTiem);
                    if (Convert.ToInt32(lenh.ExecuteScalar()) > 0)
                    {
                        giaoDich.Rollback();
                        return false;
                    }
                }

                using (var lenh = new SqlCommand(sqlCapNhatLich, ketNoi, giaoDich))
                {
                    lenh.Parameters.AddWithValue("@MaLichTiem", lichSu.MaLichTiem);
                    lenh.Parameters.AddWithValue("@TrangThai", "Đã tiêm");
                    if (lenh.ExecuteNonQuery() <= 0)
                    {
                        giaoDich.Rollback();
                        return false;
                    }
                }

                using (var lenh = new SqlCommand(sqlThemLichSu, ketNoi, giaoDich))
                {
                    lenh.Parameters.AddWithValue("@MaLichTiem", lichSu.MaLichTiem);
                    lenh.Parameters.AddWithValue("@NgayTiemThucTe", lichSu.NgayTiemThucTe.Date);
                    lenh.Parameters.AddWithValue("@GhiChu", string.IsNullOrWhiteSpace(lichSu.GhiChu) ? DBNull.Value : lichSu.GhiChu);
                    lenh.Parameters.AddWithValue("@NgayCapNhat", lichSu.NgayCapNhat);
                    lenh.ExecuteNonQuery();
                }

                var tieuDeDaCapNhat = "Đã cập nhật trạng thái tiêm";
                var thongTinMui = tenMui;
                if (string.IsNullOrWhiteSpace(thongTinMui))
                {
                    thongTinMui = $"mũi {soMui}";
                }
                else if (!thongTinMui.StartsWith("mũi", StringComparison.OrdinalIgnoreCase))
                {
                    thongTinMui = $"mũi {soMui} - {tenMui}";
                }

                var noiDungDaCapNhat = $"{hoTenHoSo} đã được cập nhật trạng thái đã tiêm cho {thongTinMui} - {tenVaccine}.";
                var maThongBaoDaCapNhat = LayMaThongBao(sqlTimThongBaoCapNhat, ketNoi, giaoDich, maTaiKhoan, lichSu.MaLichTiem, tieuDeDaCapNhat);

                if (!maThongBaoDaCapNhat.HasValue)
                {
                    var maThongBaoNhacLich = LayMaThongBao(sqlTimThongBaoNhacLich, ketNoi, giaoDich, maTaiKhoan, lichSu.MaLichTiem, tieuDeDaCapNhat);
                    if (maThongBaoNhacLich.HasValue)
                    {
                        using var lenh = new SqlCommand(sqlChuyenThongBao, ketNoi, giaoDich);
                        GanThamSoThongBao(lenh, maTaiKhoan, lichSu.MaLichTiem, tieuDeDaCapNhat, noiDungDaCapNhat);
                        lenh.Parameters.AddWithValue("@MaThongBao", maThongBaoNhacLich.Value);
                        lenh.ExecuteNonQuery();
                    }
                    else
                    {
                        using var lenh = new SqlCommand(sqlThemThongBao, ketNoi, giaoDich);
                        GanThamSoThongBao(lenh, maTaiKhoan, lichSu.MaLichTiem, tieuDeDaCapNhat, noiDungDaCapNhat);
                        lenh.ExecuteNonQuery();
                    }
                }

                using (var lenh = new SqlCommand(sqlXoaThongBaoNhacLichConLai, ketNoi, giaoDich))
                {
                    GanThamSoThongBaoCoBan(lenh, maTaiKhoan, lichSu.MaLichTiem);
                    lenh.ExecuteNonQuery();
                }

                giaoDich.Commit();
                return true;
            }
            catch
            {
                giaoDich.Rollback();
                throw;
            }
        }

        // cập nhật trạng thái mũi tiêm
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

        // kiểm tra lịch đã tồn tại
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

        // Xóa toàn bộ lịch tiêm, lịch sử, thông báo của hồ sơ (dùng khi đổi ngày sinh)
        // Tuân thủ thứ tự: ThongBao → LichSuTiem → LichTiem (tránh FK conflict)
        public bool XoaToanBoLichTiemCuaHoSo(int maHoSo, int maTaiKhoan)
        {
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            ketNoi.Open();
            using var giaoDich = ketNoi.BeginTransaction();

            try
            {
                // 1. Xóa ThongBao liên quan đến lịch tiêm của hồ sơ
                using (var lenh = new SqlCommand(@"
DELETE tb FROM ThongBao tb
INNER JOIN LichTiem lt ON tb.maLichTiem = lt.maLichTiem
WHERE lt.maHoSo = @MaHoSo AND tb.maTaiKhoan = @MaTaiKhoan", ketNoi, giaoDich))
                {
                    lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
                    lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
                    lenh.ExecuteNonQuery();
                }

                // 2. Xóa LichSuTiem liên quan đến lịch tiêm của hồ sơ
                using (var lenh = new SqlCommand(@"
DELETE lst FROM LichSuTiem lst
INNER JOIN LichTiem lt ON lst.maLichTiem = lt.maLichTiem
WHERE lt.maHoSo = @MaHoSo", ketNoi, giaoDich))
                {
                    lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
                    lenh.ExecuteNonQuery();
                }

                // 3. Xóa LichTiem của hồ sơ
                using (var lenh = new SqlCommand(@"
DELETE FROM LichTiem WHERE maHoSo = @MaHoSo", ketNoi, giaoDich))
                {
                    lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
                    lenh.ExecuteNonQuery();
                }

                giaoDich.Commit();
                return true;
            }
            catch
            {
                giaoDich.Rollback();
                return false;
            }
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
                GhiChu = ChuanHoaGhiChuHienThi(doc["ghiChu"] == DBNull.Value ? string.Empty : doc["ghiChu"].ToString() ?? string.Empty),
                HoTenHoSo = doc["hoTenHoSo"] == DBNull.Value ? string.Empty : doc["hoTenHoSo"].ToString() ?? string.Empty,
                TenVaccine = doc["tenVaccine"] == DBNull.Value ? string.Empty : doc["tenVaccine"].ToString() ?? string.Empty,
                NhomVaccine = CoCot(doc, "nhomVaccine") && doc["nhomVaccine"] != DBNull.Value ? doc["nhomVaccine"].ToString() ?? string.Empty : string.Empty,
                TenMui = doc["tenMui"] == DBNull.Value ? string.Empty : doc["tenMui"].ToString() ?? string.Empty,
                SoMui = Convert.ToInt32(doc["soMui"]),
                NgaySinhHoSo = CoCot(doc, "ngaySinhHoSo") && doc["ngaySinhHoSo"] != DBNull.Value
                    ? Convert.ToDateTime(doc["ngaySinhHoSo"])
                    : DateTime.MinValue
            };
        }

        // kiểm tra cột tồn tại
        private static bool CoCot(SqlDataReader doc, string tenCot)
        {
            for (var i = 0; i < doc.FieldCount; i++)
            {
                if (string.Equals(doc.GetName(i), tenCot, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        // lấy mã thông báo
        private static int? LayMaThongBao(string sql, SqlConnection ketNoi, SqlTransaction giaoDich, int maTaiKhoan, int maLichTiem, string tieuDeDaCapNhat)
        {
            using var lenh = new SqlCommand(sql, ketNoi, giaoDich);
            GanThamSoThongBaoCoBan(lenh, maTaiKhoan, maLichTiem);
            lenh.Parameters.AddWithValue("@TieuDeDaCapNhat", tieuDeDaCapNhat);
            var ketQua = lenh.ExecuteScalar();
            return ketQua == null || ketQua == DBNull.Value ? null : Convert.ToInt32(ketQua);
        }

        private static void GanThamSoThongBao(SqlCommand lenh, int maTaiKhoan, int maLichTiem, string tieuDeDaCapNhat, string noiDungDaCapNhat)
        {
            GanThamSoThongBaoCoBan(lenh, maTaiKhoan, maLichTiem);
            lenh.Parameters.AddWithValue("@TieuDeDaCapNhat", tieuDeDaCapNhat);
            lenh.Parameters.AddWithValue("@NoiDungDaCapNhat", noiDungDaCapNhat);
            lenh.Parameters.AddWithValue("@NgayGui", DateTime.Now);
        }

        private static void GanThamSoThongBaoCoBan(SqlCommand lenh, int maTaiKhoan, int maLichTiem)
        {
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@MaLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@TieuDeSapDen", "Sắp đến lịch tiêm");
            lenh.Parameters.AddWithValue("@TieuDeHomNay", "Đến lịch tiêm hôm nay");
            lenh.Parameters.AddWithValue("@TieuDeQuaHan", "Quá hạn lịch tiêm");
        }

        // Chuẩn hóa ghi chú cũ/null để người dùng không thấy nội dung kỹ thuật khi xem lịch tiêm.
        private static string ChuanHoaGhiChuHienThi(string ghiChu)
        {
            if (string.IsNullOrWhiteSpace(ghiChu))
            {
                return "Theo lịch tiêm khuyến nghị";
            }

            var noiDungCu = ghiChu.Trim();
            if (string.Equals(noiDungCu, "Tự động tạo lịch tiêm", StringComparison.OrdinalIgnoreCase)
                || string.Equals(noiDungCu, "Tự động tạo lịch tiêm theo độ tuổi", StringComparison.OrdinalIgnoreCase))
            {
                return "Mũi tiêm được khuyến nghị theo độ tuổi";
            }

            return ghiChu;
        }
    }
}
