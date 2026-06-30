using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text;

namespace DoAnTotNghiep.Services
{
    public class TaoLichTiemService
    {
        private readonly string chuoiKetNoi;

        public TaoLichTiemService(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        // tạo lịch tiêm cho hồ sơ
        public KetQuaTaoLichTiem TaoLichTiemChoHoSo(int maHoSo)
        {
            var hoSo = LayHoSoTheoMa(maHoSo);
            if (hoSo == null)
            {
                return new KetQuaTaoLichTiem();
            }

            var danhSachMuiTiem = LayDanhSachMuiTiemVaccine();
            var ketQua = new KetQuaTaoLichTiem
            {
                SoMuiTiemVaccine = danhSachMuiTiem.Count
            };

            foreach (var nhomVaccine in danhSachMuiTiem.GroupBy(muiTiem => muiTiem.MaVaccine))
            {
                TaoLichChoTungVaccine(maHoSo, hoSo.NgaySinh, nhomVaccine.OrderBy(muiTiem => muiTiem.SoMui), ketQua);
            }

            return ketQua;
        }

        // điều chỉnh lịch mũi sau khi tiêm
        public int DieuChinhLichMuiTiepTheoSauKhiTiem(int maHoSo, int maMuiTiemDaTiem, DateTime ngayTiemThucTe)
        {
            var danhSachMuiCungVaccine = LayDanhSachLichTheoVaccineCuaMui(maHoSo, maMuiTiemDaTiem);
            var viTriMuiDaTiem = danhSachMuiCungVaccine.FindIndex(mui => mui.MaMuiTiem == maMuiTiemDaTiem);
            if (viTriMuiDaTiem < 0)
            {
                return 0;
            }

            var ngayCoSo = ngayTiemThucTe.Date;
            var ngayCoSoLaNgayTiemThucTe = true;
            var soLichDaDieuChinh = 0;

            for (var i = viTriMuiDaTiem + 1; i < danhSachMuiCungVaccine.Count; i++)
            {
                var muiTiepTheo = danhSachMuiCungVaccine[i];
                if (muiTiepTheo.NgayTiemThucTe.HasValue || string.Equals(muiTiepTheo.TrangThai, "Đã tiêm", StringComparison.OrdinalIgnoreCase))
                {
                    ngayCoSo = muiTiepTheo.NgayTiemThucTe?.Date ?? muiTiepTheo.NgayTiemDuKien.Date;
                    ngayCoSoLaNgayTiemThucTe = muiTiepTheo.NgayTiemThucTe.HasValue;
                    continue;
                }

                var khoangCachNgay = LayKhoangCachNgay(muiTiepTheo);
                var ngayDuKienMoi = TinhNgaySauKhoangCach(ngayCoSo, khoangCachNgay, ngayCoSoLaNgayTiemThucTe);
                if (CapNhatNgayTiemDuKien(muiTiepTheo.MaLichTiem, ngayDuKienMoi, $"Lịch được điều chỉnh theo ngày tiêm thực tế của mũi trước, cách {khoangCachNgay} ngày"))
                {
                    soLichDaDieuChinh++;
                }

                ngayCoSo = ngayDuKienMoi;
                ngayCoSoLaNgayTiemThucTe = false;
            }

            return soLichDaDieuChinh;
        }

        // tạo lịch cho từng vaccine
        private void TaoLichChoTungVaccine(
            int maHoSo,
            DateTime ngaySinh,
            IEnumerable<MuiTiemTaoLich> danhSachMuiTiem,
            KetQuaTaoLichTiem ketQua)
        {
            DateTime? ngayTiemMuiTruoc = null;
            var ngayTiemMuiTruocLaNgayThucTe = false;

            foreach (var muiTiem in danhSachMuiTiem)
            {
                if (!KiemTraMuiTiemPhuHopVoiTuoi(ngaySinh, muiTiem))
                {
                    continue;
                }

                ketQua.SoMuiTiemPhuHop++;
                ketQua.MaMuiTiemPhuHop.Add(muiTiem.MaMuiTiem);

                if (LaVaccineNhacHangNam(muiTiem))
                {
                    var ketQuaNgay = TinhNgayTiemNhacHangNam(ngaySinh, muiTiem);
                    if (ThemLichNhacHangNamNeuChuaTonTai(maHoSo, muiTiem.MaMuiTiem, ketQuaNgay.NgayTiemDuKien, ketQuaNgay.GhiChu))
                    {
                        ketQua.SoLichTiemDaTao++;
                    }

                    ngayTiemMuiTruoc = ketQuaNgay.NgayTiemDuKien;
                    continue;
                }

                if (KiemTraLichTiemDaTonTai(maHoSo, muiTiem.MaMuiTiem))
                {
                    var ketQuaNgayTiemDaCo = TinhNgayTiemTheoThuTuMui(ngaySinh, muiTiem, ngayTiemMuiTruoc, ngayTiemMuiTruocLaNgayThucTe);
                    CapNhatLichTiemTuDongNeuCan(maHoSo, muiTiem.MaMuiTiem, ketQuaNgayTiemDaCo.NgayTiemDuKien, ketQuaNgayTiemDaCo.GhiChu);
                    var ngayTiemCoSoDaCo = LayNgayTiemCoSoDaCo(maHoSo, muiTiem.MaMuiTiem);
                    ngayTiemMuiTruoc = ngayTiemCoSoDaCo?.NgayTiem ?? ketQuaNgayTiemDaCo.NgayTiemDuKien;
                    ngayTiemMuiTruocLaNgayThucTe = ngayTiemCoSoDaCo?.LaNgayTiemThucTe ?? false;
                    continue;
                }

                var ketQuaNgayTiem = TinhNgayTiemTheoThuTuMui(ngaySinh, muiTiem, ngayTiemMuiTruoc, ngayTiemMuiTruocLaNgayThucTe);
                if (ThemLichTiemNeuChuaTonTai(maHoSo, muiTiem.MaMuiTiem, ketQuaNgayTiem.NgayTiemDuKien, ketQuaNgayTiem.GhiChu))
                {
                    ketQua.SoLichTiemDaTao++;
                }

                ngayTiemMuiTruoc = ketQuaNgayTiem.NgayTiemDuKien;
                ngayTiemMuiTruocLaNgayThucTe = false;
            }
        }

        // lấy hồ sơ theo mã
        private HoSoTaoLich? LayHoSoTheoMa(int maHoSo)
        {
            const string sql = @"SELECT maHoSo, ngaySinh
FROM HoSoSucKhoe
WHERE maHoSo = @MaHoSo";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            if (!doc.Read())
            {
                return null;
            }

            return new HoSoTaoLich
            {
                MaHoSo = Convert.ToInt32(doc["maHoSo"]),
                NgaySinh = Convert.ToDateTime(doc["ngaySinh"])
            };
        }

        // lấy danh sách mũi tiêm để tạo lịch
        // Dữ liệu đầu vào: các model, mã định danh hoặc dữ liệu nghiệp vụ cần thiết cho quá trình xử lý.
        // Xử lý chính: phối hợp các bước tính toán, kiểm tra điều kiện và gọi DAL khi cần truy xuất dữ liệu.
        // Kết quả trả về: kết quả nghiệp vụ sau khi xử lý, có thể là dữ liệu, trạng thái thành công hoặc không trả về giá trị nếu chỉ thực hiện tác vụ.
        private List<MuiTiemTaoLich> LayDanhSachMuiTiemVaccine()
        {
            const string sql = @"SELECT
    mt.maMuiTiem,
    mt.maVaccine,
    mt.soMui,
    mt.doTuoiToiThieu,
    mt.doTuoiToiDa,
    mt.doTuoiKhuyenNghi,
    mt.donViTuoi,
    mt.khoangCachNgay,
    v.tenVaccine,
    v.nhomVaccine
FROM MuiTiemVaccine mt
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
WHERE v.trangThai = 1
ORDER BY mt.maVaccine, mt.soMui";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<MuiTiemTaoLich>();
            while (doc.Read())
            {
                danhSach.Add(new MuiTiemTaoLich
                {
                    MaMuiTiem = Convert.ToInt32(doc["maMuiTiem"]),
                    MaVaccine = Convert.ToInt32(doc["maVaccine"]),
                    SoMui = Convert.ToInt32(doc["soMui"]),
                    DoTuoiToiThieu = doc["doTuoiToiThieu"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiThieu"]),
                    DoTuoiToiDa = doc["doTuoiToiDa"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiDa"]),
                    DoTuoiKhuyenNghi = doc["doTuoiKhuyenNghi"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiKhuyenNghi"]),
                    DonViTuoi = doc["donViTuoi"] == DBNull.Value ? string.Empty : doc["donViTuoi"].ToString() ?? string.Empty,
                    KhoangCachNgay = doc["khoangCachNgay"] == DBNull.Value ? null : Convert.ToInt32(doc["khoangCachNgay"]),
                    TenVaccine = doc["tenVaccine"] == DBNull.Value ? string.Empty : doc["tenVaccine"].ToString() ?? string.Empty,
                    NhomVaccine = doc["nhomVaccine"] == DBNull.Value ? string.Empty : doc["nhomVaccine"].ToString() ?? string.Empty
                });
            }

            return danhSach;
        }

        // lấy danh sách lịch theo vaccine của mũi
        private List<MuiTiemDieuChinhLich> LayDanhSachLichTheoVaccineCuaMui(int maHoSo, int maMuiTiem)
        {
            const string sql = @"SELECT
    lt.maLichTiem,
    lt.maMuiTiem,
    lt.ngayTiemDuKien,
    lt.trangThai,
    mt.maVaccine,
    mt.soMui,
    mt.khoangCachNgay,
    v.tenVaccine,
    v.nhomVaccine,
    lst.ngayTiemThucTe
FROM LichTiem lt
INNER JOIN MuiTiemVaccine mt ON lt.maMuiTiem = mt.maMuiTiem
INNER JOIN Vaccine v ON mt.maVaccine = v.maVaccine
OUTER APPLY (
    SELECT TOP 1 ngayTiemThucTe
    FROM LichSuTiem
    WHERE maLichTiem = lt.maLichTiem
    ORDER BY ngayCapNhat DESC, maLichSu DESC
) lst
WHERE lt.maHoSo = @MaHoSo
AND mt.maVaccine = (
    SELECT maVaccine
    FROM MuiTiemVaccine
    WHERE maMuiTiem = @MaMuiTiem
)
ORDER BY mt.soMui";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();

            var danhSach = new List<MuiTiemDieuChinhLich>();
            while (doc.Read())
            {
                danhSach.Add(new MuiTiemDieuChinhLich
                {
                    MaLichTiem = Convert.ToInt32(doc["maLichTiem"]),
                    MaMuiTiem = Convert.ToInt32(doc["maMuiTiem"]),
                    MaVaccine = Convert.ToInt32(doc["maVaccine"]),
                    SoMui = Convert.ToInt32(doc["soMui"]),
                    NgayTiemDuKien = Convert.ToDateTime(doc["ngayTiemDuKien"]),
                    TrangThai = doc["trangThai"] == DBNull.Value ? string.Empty : doc["trangThai"].ToString() ?? string.Empty,
                    KhoangCachNgay = doc["khoangCachNgay"] == DBNull.Value ? null : Convert.ToInt32(doc["khoangCachNgay"]),
                    TenVaccine = doc["tenVaccine"] == DBNull.Value ? string.Empty : doc["tenVaccine"].ToString() ?? string.Empty,
                    NhomVaccine = doc["nhomVaccine"] == DBNull.Value ? string.Empty : doc["nhomVaccine"].ToString() ?? string.Empty,
                    NgayTiemThucTe = doc["ngayTiemThucTe"] == DBNull.Value ? null : Convert.ToDateTime(doc["ngayTiemThucTe"])
                });
            }

            return danhSach;
        }

        private bool CapNhatNgayTiemDuKien(int maLichTiem, DateTime ngayTiemDuKien, string ghiChu)
        {
            const string sql = @"UPDATE LichTiem
SET ngayTiemDuKien = @NgayTiemDuKien,
    ghiChu = @GhiChu
WHERE maLichTiem = @MaLichTiem
AND trangThai <> N'Đã tiêm'
AND NOT EXISTS (
    SELECT 1
    FROM LichSuTiem
    WHERE maLichTiem = @MaLichTiem
)
AND CAST(ngayTiemDuKien AS date) <> @NgayTiemDuKien";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaLichTiem", maLichTiem);
            lenh.Parameters.AddWithValue("@NgayTiemDuKien", ngayTiemDuKien.Date);
            lenh.Parameters.AddWithValue("@GhiChu", ghiChu);

            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        // kiểm tra lịch tiêm đã tồn tại
        private bool KiemTraLichTiemDaTonTai(int maHoSo, int maMuiTiem)
        {
            const string sql = @"SELECT COUNT(*)
FROM LichTiem
WHERE maHoSo = @MaHoSo
AND maMuiTiem = @MaMuiTiem";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);

            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // lấy ngày tiêm cơ sở đã có
        private NgayTiemCoSo? LayNgayTiemCoSoDaCo(int maHoSo, int maMuiTiem)
        {
            const string sql = @"SELECT TOP 1
    COALESCE(lst.ngayTiemThucTe, lt.ngayTiemDuKien) AS ngayTiemCoSo,
    CASE WHEN lst.ngayTiemThucTe IS NULL THEN 0 ELSE 1 END AS laNgayTiemThucTe
FROM LichTiem lt
OUTER APPLY (
    SELECT TOP 1 ngayTiemThucTe
    FROM LichSuTiem
    WHERE maLichTiem = lt.maLichTiem
    ORDER BY ngayCapNhat DESC, maLichSu DESC
) lst
WHERE lt.maHoSo = @MaHoSo
AND lt.maMuiTiem = @MaMuiTiem
ORDER BY COALESCE(lst.ngayTiemThucTe, lt.ngayTiemDuKien) DESC";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            if (!doc.Read())
            {
                return null;
            }

            return new NgayTiemCoSo
            {
                NgayTiem = Convert.ToDateTime(doc["ngayTiemCoSo"]),
                LaNgayTiemThucTe = Convert.ToInt32(doc["laNgayTiemThucTe"]) == 1
            };
        }

        // kiểm tra lịch nhắc năm đã tồn tại
        private bool KiemTraLichNhacHangNamDaTonTai(int maHoSo, int maMuiTiem, int nam)
        {
            const string sql = @"SELECT COUNT(*)
FROM LichTiem
WHERE maHoSo = @MaHoSo
AND maMuiTiem = @MaMuiTiem
AND YEAR(ngayTiemDuKien) = @Nam";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);
            lenh.Parameters.AddWithValue("@Nam", nam);

            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
        }

        // cập nhật lịch tự động nếu cần
        private void CapNhatLichTiemTuDongNeuCan(int maHoSo, int maMuiTiem, DateTime ngayTiemDuKien, string ghiChu)
        {
            const string sql = @"UPDATE LichTiem
SET ngayTiemDuKien = @NgayTiemDuKien,
    ghiChu = @GhiChu
WHERE maHoSo = @MaHoSo
AND maMuiTiem = @MaMuiTiem
AND trangThai <> N'Đã tiêm'
AND CAST(ngayTiemDuKien AS date) <> @NgayTiemDuKien
AND NOT EXISTS (
    SELECT 1
    FROM LichSuTiem lst
    WHERE lst.maLichTiem = LichTiem.maLichTiem
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);
            lenh.Parameters.AddWithValue("@NgayTiemDuKien", ngayTiemDuKien.Date);
            lenh.Parameters.AddWithValue("@GhiChu", ghiChu);

            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // insert lịch nếu chưa tồn tại
        // Dữ liệu đầu vào: các model, mã định danh hoặc dữ liệu nghiệp vụ cần thiết cho quá trình xử lý.
        // Xử lý chính: phối hợp các bước tính toán, kiểm tra điều kiện và gọi DAL khi cần truy xuất dữ liệu.
        // Kết quả trả về: kết quả nghiệp vụ sau khi xử lý, có thể là dữ liệu, trạng thái thành công hoặc không trả về giá trị nếu chỉ thực hiện tác vụ.
        private bool ThemLichTiemNeuChuaTonTai(int maHoSo, int maMuiTiem, DateTime ngayTiemDuKien, string ghiChu)
        {
            const string sql = @"INSERT INTO LichTiem(maHoSo, maMuiTiem, ngayTiemDuKien, trangThai, ghiChu)
SELECT @MaHoSo, @MaMuiTiem, @NgayTiemDuKien, @TrangThai, @GhiChu
WHERE NOT EXISTS (
    SELECT 1
    FROM LichTiem WITH (UPDLOCK, HOLDLOCK)
    WHERE maHoSo = @MaHoSo
    AND maMuiTiem = @MaMuiTiem
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);
            lenh.Parameters.AddWithValue("@NgayTiemDuKien", ngayTiemDuKien.Date);
            lenh.Parameters.AddWithValue("@TrangThai", "Chưa tiêm");
            lenh.Parameters.AddWithValue("@GhiChu", ghiChu);

            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        // thêm lịch nhắc năm nếu chưa tồn tại
        private bool ThemLichNhacHangNamNeuChuaTonTai(int maHoSo, int maMuiTiem, DateTime ngayTiemDuKien, string ghiChu)
        {
            if (KiemTraLichNhacHangNamDaTonTai(maHoSo, maMuiTiem, ngayTiemDuKien.Year))
            {
                return false;
            }

            const string sql = @"INSERT INTO LichTiem(maHoSo, maMuiTiem, ngayTiemDuKien, trangThai, ghiChu)
SELECT @MaHoSo, @MaMuiTiem, @NgayTiemDuKien, @TrangThai, @GhiChu
WHERE NOT EXISTS (
    SELECT 1
    FROM LichTiem WITH (UPDLOCK, HOLDLOCK)
    WHERE maHoSo = @MaHoSo
    AND maMuiTiem = @MaMuiTiem
    AND YEAR(ngayTiemDuKien) = @Nam
)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaHoSo", maHoSo);
            lenh.Parameters.AddWithValue("@MaMuiTiem", maMuiTiem);
            lenh.Parameters.AddWithValue("@NgayTiemDuKien", ngayTiemDuKien.Date);
            lenh.Parameters.AddWithValue("@Nam", ngayTiemDuKien.Year);
            lenh.Parameters.AddWithValue("@TrangThai", "Chưa tiêm");
            lenh.Parameters.AddWithValue("@GhiChu", ghiChu);

            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        // tính ngày tiêm theo thứ tự mũi
        private static KetQuaTinhNgay TinhNgayTiemTheoThuTuMui(
            DateTime ngaySinh,
            MuiTiemTaoLich muiTiem,
            DateTime? ngayTiemMuiTruoc,
            bool ngayTiemMuiTruocLaNgayThucTe = false)
        {
            if (muiTiem.SoMui > 1 && ngayTiemMuiTruoc.HasValue)
            {
                var khoangCachNgay = LayKhoangCachNgay(muiTiem);
                return new KetQuaTinhNgay
                {
                    NgayTiemDuKien = TinhNgaySauKhoangCach(ngayTiemMuiTruoc.Value.Date, khoangCachNgay, ngayTiemMuiTruocLaNgayThucTe),
                    GhiChu = $"Mũi tiêm được tính theo khoảng cách {khoangCachNgay} ngày từ mũi trước"
                };
            }

            var ngayTheoTuoi = TinhNgayTiemTheoDoTuoi(ngaySinh, muiTiem);
            if (ngayTheoTuoi.HasValue)
            {
                return new KetQuaTinhNgay
                {
                    NgayTiemDuKien = ngayTheoTuoi.Value,
                    GhiChu = "Mũi tiêm được khuyến nghị theo độ tuổi"
                };
            }

            return new KetQuaTinhNgay
            {
                NgayTiemDuKien = DateTime.Today,
                GhiChu = "Không đủ dữ liệu độ tuổi/khoảng cách để tính lịch, hệ thống tạm dùng ngày hiện tại"
            };
        }

        // tính ngày sau khoảng cách
        private static DateTime TinhNgaySauKhoangCach(DateTime ngayCoSo, int khoangCachNgay, bool ngayCoSoLaNgayTiemThucTe)
        {
            return ngayCoSo.Date.AddDays(khoangCachNgay + (ngayCoSoLaNgayTiemThucTe ? 1 : 0));
        }

        // lấy khoảng cách ngày
        private static int LayKhoangCachNgay(MuiTiemTaoLich muiTiem)
        {
            if (muiTiem.KhoangCachNgay.HasValue && muiTiem.KhoangCachNgay.Value > 0)
            {
                return muiTiem.KhoangCachNgay.Value;
            }

            var noiDung = BoDauTiengViet($"{muiTiem.TenVaccine} {muiTiem.NhomVaccine}").ToLowerInvariant();
            if (noiDung.Contains("hpv"))
            {
                return muiTiem.SoMui == 2 ? 60 : 120;
            }

            return 30;
        }

        // tính ngày tiêm nhắc năm
        private static KetQuaTinhNgay TinhNgayTiemNhacHangNam(DateTime ngaySinh, MuiTiemTaoLich muiTiem)
        {
            var ngayTheoTuoi = TinhNgayTiemTheoDoTuoi(ngaySinh, muiTiem);
            if (!ngayTheoTuoi.HasValue)
            {
                return new KetQuaTinhNgay
                {
                    NgayTiemDuKien = DateTime.Today,
                    GhiChu = "Mũi nhắc hằng năm, không đủ dữ liệu độ tuổi để tính lịch nên tạm dùng ngày hiện tại"
                };
            }

            var ngayCoSo = ngayTheoTuoi.Value.Date;
            return new KetQuaTinhNgay
            {
                NgayTiemDuKien = TaoNgayTrongNamAnToan(DateTime.Today.Year, ngayCoSo.Month, ngayCoSo.Day),
                GhiChu = "Mũi nhắc hằng năm được tạo cho năm hiện tại"
            };
        }

        // tính ngày tiêm theo độ tuổi
        private static DateTime? TinhNgayTiemTheoDoTuoi(DateTime ngaySinh, MuiTiemTaoLich muiTiem)
        {
            var doTuoiTinhLich = muiTiem.DoTuoiKhuyenNghi ?? muiTiem.DoTuoiToiThieu;
            if (!doTuoiTinhLich.HasValue)
            {
                return null;
            }

            return TinhNgayTiemDuKien(ngaySinh, doTuoiTinhLich.Value, muiTiem.DonViTuoi);
        }

        // tính ngày tiêm dự kiến
        private static DateTime? TinhNgayTiemDuKien(DateTime ngaySinh, int doTuoi, string donViTuoi)
        {
            return DonViTuoiHelper.CongTheoDonVi(ngaySinh, doTuoi, donViTuoi);
        }

        // tạo ngày an toàn trong năm
        private static DateTime TaoNgayTrongNamAnToan(int nam, int thang, int ngay)
        {
            var ngayToiDaTrongThang = DateTime.DaysInMonth(nam, thang);
            return new DateTime(nam, thang, Math.Min(ngay, ngayToiDaTrongThang));
        }

        // kiểm tra vaccine nhắc năm
        private static bool LaVaccineNhacHangNam(MuiTiemTaoLich muiTiem)
        {
            var noiDung = BoDauTiengViet($"{muiTiem.TenVaccine} {muiTiem.NhomVaccine}").ToLowerInvariant();
            return noiDung.Contains("cum")
                || noiDung.Contains("cum mua")
                || noiDung.Contains("hang nam");
        }

        // bỏ dấu tiếng việt
        private static string BoDauTiengViet(string giaTri)
        {
            var normalized = giaTri.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();
            foreach (var kyTu in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(kyTu) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(kyTu);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        // chuẩn hóa đơn vị tuổi
        private static string ChuanHoaDonViTuoi(string donViTuoi)
        {
            return DonViTuoiHelper.ChuanHoa(donViTuoi);
        }

        // Kiểm tra mũi tiêm có phù hợp với tuổi hiện tại của hồ sơ theo đúng đơn vị ngày/tuần/tháng/năm.
        private static bool KiemTraMuiTiemPhuHopVoiTuoi(DateTime ngaySinh, MuiTiemTaoLich muiTiem)
        {
            // Luôn trả về true để tạo lịch cho tất cả vaccine/mũi đang hoạt động.
            // Ngày dự kiến sẽ được tính theo ngày sinh + độ tuổi khuyến nghị.
            // Nếu ngày dự kiến trong quá khứ → quá hạn, trong tương lai → sắp tới.
            // Không lọc bỏ bất kỳ mũi nào để người dùng thấy toàn bộ kế hoạch dài hạn.
            return true;
        }

        // Tính tuổi hiện tại của hồ sơ theo đơn vị tương ứng để lọc mũi tiêm phù hợp.
        private static int TinhTuoiTheoDonVi(DateTime ngaySinh, string donViTuoi)
        {
            return DonViTuoiHelper.TinhTuoiHienTai(ngaySinh, donViTuoi);
        }

        // tính số tháng tuổi
        private static int TinhSoThangTuoi(DateTime ngaySinh, DateTime ngayHienTai)
        {
            var soThang = ((ngayHienTai.Year - ngaySinh.Year) * 12) + ngayHienTai.Month - ngaySinh.Month;
            if (ngayHienTai.Day < ngaySinh.Day)
            {
                soThang--;
            }

            return Math.Max(0, soThang);
        }

        // tính số năm tuổi
        private static int TinhSoNamTuoi(DateTime ngaySinh, DateTime ngayHienTai)
        {
            var soNam = ngayHienTai.Year - ngaySinh.Year;
            if (ngayHienTai.Date < ngaySinh.Date.AddYears(soNam))
            {
                soNam--;
            }

            return Math.Max(0, soNam);
        }

        private class HoSoTaoLich
        {
            public int MaHoSo { get; set; }
            public DateTime NgaySinh { get; set; }
        }

        private class MuiTiemTaoLich
        {
            public int MaMuiTiem { get; set; }
            public int MaVaccine { get; set; }
            public int SoMui { get; set; }
            public int? DoTuoiToiThieu { get; set; }
            public int? DoTuoiToiDa { get; set; }
            public int? DoTuoiKhuyenNghi { get; set; }
            public string DonViTuoi { get; set; } = string.Empty;
            public int? KhoangCachNgay { get; set; }
            public string TenVaccine { get; set; } = string.Empty;
            public string NhomVaccine { get; set; } = string.Empty;
        }

        private class MuiTiemDieuChinhLich : MuiTiemTaoLich
        {
            public int MaLichTiem { get; set; }
            public DateTime NgayTiemDuKien { get; set; }
            public string TrangThai { get; set; } = string.Empty;
            public DateTime? NgayTiemThucTe { get; set; }
        }

        private class NgayTiemCoSo
        {
            public DateTime NgayTiem { get; set; }
            public bool LaNgayTiemThucTe { get; set; }
        }

        private class KetQuaTinhNgay
        {
            public DateTime NgayTiemDuKien { get; set; }
            public string GhiChu { get; set; } = string.Empty;
        }

        public class KetQuaTaoLichTiem
        {
            public int SoMuiTiemVaccine { get; set; }
            public int SoMuiTiemPhuHop { get; set; }
            public int SoLichTiemDaTao { get; set; }
            public List<int> MaMuiTiemPhuHop { get; set; } = new();
        }

        // Lớp debug dùng để trả về dữ liệu lịch tiêm
    }
}
