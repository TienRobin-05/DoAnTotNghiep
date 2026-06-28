using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class BaiVietCamNang_DAL
    {
        private const string LoaiTinTuc = "Tin tức";
        private const string LoaiCamNang = "Cẩm nang sức khỏe";
        private readonly string chuoiKetNoi;

        public BaiVietCamNang_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        // khởi tạo mở rộng bảng nếu cần
        public void KhoiTaoMoRongNeuCan()
        {
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            ketNoi.Open();

            ChayLenh(ketNoi, @"
IF COL_LENGTH('BaiVietCamNang', 'slug') IS NULL
BEGIN
    ALTER TABLE BaiVietCamNang ADD slug NVARCHAR(250) NULL;
END");

            ChayLenh(ketNoi, @"
IF COL_LENGTH('BaiVietCamNang', 'moTaNgan') IS NULL
BEGIN
    ALTER TABLE BaiVietCamNang ADD moTaNgan NVARCHAR(700) NULL;
END");

            ChayLenh(ketNoi, @"
IF COL_LENGTH('BaiVietCamNang', 'noiBat') IS NULL
BEGIN
    ALTER TABLE BaiVietCamNang ADD noiBat BIT NOT NULL CONSTRAINT DF_BaiVietCamNang_noiBat DEFAULT(0);
END");

            ChayLenh(ketNoi, @"
IF COL_LENGTH('BaiVietCamNang', 'luotXem') IS NULL
BEGIN
    ALTER TABLE BaiVietCamNang ADD luotXem INT NOT NULL CONSTRAINT DF_BaiVietCamNang_luotXem DEFAULT(0);
END");

            ChayLenh(ketNoi, @"
UPDATE BaiVietCamNang
SET loaiBaiViet = N'Cẩm nang sức khỏe'
WHERE loaiBaiViet IN (N'Cẩm nang', N'Cam nang', N'cam-nang', N'guide');");

            ChayLenh(ketNoi, @"
UPDATE BaiVietCamNang
SET slug = CONCAT(N'bai-viet-', maBaiViet)
WHERE slug IS NULL OR LTRIM(RTRIM(slug)) = N'';");

            ChayLenh(ketNoi, @"
UPDATE BaiVietCamNang
SET moTaNgan = LEFT(REPLACE(REPLACE(COALESCE(noiDung, N''), CHAR(13), N' '), CHAR(10), N' '), 240)
WHERE moTaNgan IS NULL OR LTRIM(RTRIM(moTaNgan)) = N'';");

            SeedNeuChuaCoDuLieu(ketNoi);
        }

        // lấy bài viết cho admin
        public List<BaiVietCamNang> LayTatCaChoAdmin()
        {
            KhoiTaoMoRongNeuCan();
            const string sql = @"SELECT bv.maBaiViet, bv.maTaiKhoan, bv.tieuDe, bv.slug, bv.moTaNgan, bv.noiDung,
       bv.loaiBaiViet, bv.anhDaiDien, bv.ngayTao, bv.trangThai, bv.noiBat, bv.luotXem,
       tk.hoTen AS tenTacGia
FROM BaiVietCamNang bv
INNER JOIN TaiKhoan tk ON bv.maTaiKhoan = tk.maTaiKhoan
ORDER BY bv.ngayTao DESC";

            return DocDanhSach(sql);
        }

        public List<BaiVietCamNang> LayDanhSachHienThiChoUser(string? loaiBaiViet = null)
        {
            return LayDanhSachChoUser(loaiBaiViet, null, "newest", 1, 100, null);
        }

        public List<BaiVietCamNang> LayBaiVietNoiBatDashboard(int soLuong = 5)
        {
            var danhSach = LayDanhSachChoUser(null, null, "newest", 1, soLuong, true);
            return danhSach.Count > 0 ? danhSach : LayDanhSachChoUser(null, null, "most_viewed", 1, soLuong, null);
        }

        public List<BaiVietCamNang> LayBaiVietMoiNhatDashboard(int soLuong = 8)
        {
            return LayDanhSachChoUser(null, null, "newest", 1, soLuong, null);
        }

        // lấy bài viết cho người dùng
        public List<BaiVietCamNang> LayDanhSachChoUser(string? loaiBaiViet, string? keyword, string? sort, int page, int limit, bool? featured)
        {
            KhoiTaoMoRongNeuCan();
            page = Math.Max(1, page);
            limit = Math.Clamp(limit, 1, 24);
            var sql = new StringBuilder(@"SELECT bv.maBaiViet, bv.maTaiKhoan, bv.tieuDe, bv.slug, bv.moTaNgan, bv.noiDung,
       bv.loaiBaiViet, bv.anhDaiDien, bv.ngayTao, bv.trangThai, bv.noiBat, bv.luotXem,
       tk.hoTen AS tenTacGia
FROM BaiVietCamNang bv
INNER JOIN TaiKhoan tk ON bv.maTaiKhoan = tk.maTaiKhoan
WHERE bv.trangThai = 1");

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand();
            lenh.Connection = ketNoi;
            GanDieuKienLoc(sql, lenh, loaiBaiViet, keyword, featured);
            sql.Append(sort switch
            {
                "oldest" => " ORDER BY bv.ngayTao ASC, bv.maBaiViet ASC",
                "most_viewed" => " ORDER BY bv.luotXem DESC, bv.ngayTao DESC",
                _ => " ORDER BY bv.ngayTao DESC, bv.maBaiViet DESC"
            });
            sql.Append(" OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY");
            lenh.Parameters.AddWithValue("@Offset", (page - 1) * limit);
            lenh.Parameters.AddWithValue("@Limit", limit);
            lenh.CommandText = sql.ToString();

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return DocDanhSachTuReader(doc);
        }

        // đếm bài viết cho người dùng
        public int DemDanhSachChoUser(string? loaiBaiViet, string? keyword, bool? featured)
        {
            KhoiTaoMoRongNeuCan();
            var sql = new StringBuilder("SELECT COUNT(*) FROM BaiVietCamNang bv WHERE bv.trangThai = 1");
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand();
            lenh.Connection = ketNoi;
            GanDieuKienLoc(sql, lenh, loaiBaiViet, keyword, featured);
            lenh.CommandText = sql.ToString();

            ketNoi.Open();
            return Convert.ToInt32(lenh.ExecuteScalar());
        }

        public (int Total, int News, int Guide) LayThongKeUser()
        {
            KhoiTaoMoRongNeuCan();
            const string sql = @"SELECT
    COUNT(*) AS total,
    SUM(CASE WHEN loaiBaiViet = N'Tin tức' THEN 1 ELSE 0 END) AS news,
    SUM(CASE WHEN loaiBaiViet IN (N'Cẩm nang sức khỏe', N'Cẩm nang') THEN 1 ELSE 0 END) AS guide
FROM BaiVietCamNang
WHERE trangThai = 1";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            if (!doc.Read())
            {
                return (0, 0, 0);
            }

            return (
                Convert.ToInt32(doc["total"]),
                doc["news"] == DBNull.Value ? 0 : Convert.ToInt32(doc["news"]),
                doc["guide"] == DBNull.Value ? 0 : Convert.ToInt32(doc["guide"]));
        }

        // lấy bài viết theo mã
        public BaiVietCamNang? LayTheoId(int maBaiViet)
        {
            KhoiTaoMoRongNeuCan();
            const string sql = @"SELECT bv.maBaiViet, bv.maTaiKhoan, bv.tieuDe, bv.slug, bv.moTaNgan, bv.noiDung,
       bv.loaiBaiViet, bv.anhDaiDien, bv.ngayTao, bv.trangThai, bv.noiBat, bv.luotXem,
       tk.hoTen AS tenTacGia
FROM BaiVietCamNang bv
INNER JOIN TaiKhoan tk ON bv.maTaiKhoan = tk.maTaiKhoan
WHERE bv.maBaiViet = @MaBaiViet";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaBaiViet", maBaiViet);

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocBaiViet(doc) : null;
        }

        public BaiVietCamNang? LayTheoSlug(string slug)
        {
            KhoiTaoMoRongNeuCan();
            const string sql = @"SELECT bv.maBaiViet, bv.maTaiKhoan, bv.tieuDe, bv.slug, bv.moTaNgan, bv.noiDung,
       bv.loaiBaiViet, bv.anhDaiDien, bv.ngayTao, bv.trangThai, bv.noiBat, bv.luotXem,
       tk.hoTen AS tenTacGia
FROM BaiVietCamNang bv
INNER JOIN TaiKhoan tk ON bv.maTaiKhoan = tk.maTaiKhoan
WHERE bv.slug = @Slug AND bv.trangThai = 1";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@Slug", slug);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocBaiViet(doc) : null;
        }

        // tăng lượt xem
        public void TangLuotXem(int maBaiViet)
        {
            KhoiTaoMoRongNeuCan();
            const string sql = "UPDATE BaiVietCamNang SET luotXem = ISNULL(luotXem, 0) + 1 WHERE maBaiViet = @MaBaiViet";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaBaiViet", maBaiViet);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // thêm bài viết
        public bool Them(BaiVietCamNang bv)
        {
            KhoiTaoMoRongNeuCan();
            bv.LoaiBaiViet = ChuanHoaLoaiBaiViet(bv.LoaiBaiViet) ?? LoaiCamNang;
            bv.Slug = string.IsNullOrWhiteSpace(bv.Slug) ? TaoSlug(bv.TieuDe) : TaoSlug(bv.Slug);
            bv.MoTaNgan = TaoMoTaNgan(bv.MoTaNgan, bv.NoiDung);

            const string sql = @"INSERT INTO BaiVietCamNang(maTaiKhoan, tieuDe, slug, moTaNgan, noiDung, loaiBaiViet, anhDaiDien, ngayTao, trangThai, noiBat, luotXem)
VALUES(@MaTaiKhoan, @TieuDe, @Slug, @MoTaNgan, @NoiDung, @LoaiBaiViet, @AnhDaiDien, @NgayTao, @TrangThai, @NoiBat, @LuotXem)";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSoBaiViet(lenh, bv);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        // cập nhật bài viết
        public bool CapNhat(BaiVietCamNang bv)
        {
            KhoiTaoMoRongNeuCan();
            bv.LoaiBaiViet = ChuanHoaLoaiBaiViet(bv.LoaiBaiViet) ?? LoaiCamNang;
            bv.Slug = string.IsNullOrWhiteSpace(bv.Slug) ? TaoSlug(bv.TieuDe) : TaoSlug(bv.Slug);
            bv.MoTaNgan = TaoMoTaNgan(bv.MoTaNgan, bv.NoiDung);

            const string sql = @"UPDATE BaiVietCamNang
SET tieuDe = @TieuDe,
    slug = @Slug,
    moTaNgan = @MoTaNgan,
    noiDung = @NoiDung,
    loaiBaiViet = @LoaiBaiViet,
    anhDaiDien = @AnhDaiDien,
    trangThai = @TrangThai,
    noiBat = @NoiBat
WHERE maBaiViet = @MaBaiViet";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSoBaiViet(lenh, bv);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        // ẩn/hiện bài viết
        public bool AnHien(int maBaiViet, bool trangThai)
        {
            const string sql = "UPDATE BaiVietCamNang SET trangThai = @TrangThai WHERE maBaiViet = @MaBaiViet";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaBaiViet", maBaiViet);
            lenh.Parameters.AddWithValue("@TrangThai", trangThai);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        // đổi trạng thái nổi bật
        public bool DoiNoiBat(int maBaiViet, bool noiBat)
        {
            KhoiTaoMoRongNeuCan();
            const string sql = "UPDATE BaiVietCamNang SET noiBat = @NoiBat WHERE maBaiViet = @MaBaiViet";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaBaiViet", maBaiViet);
            lenh.Parameters.AddWithValue("@NoiBat", noiBat);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        // xóa bài viết
        public bool Xoa(int maBaiViet)
        {
            const string sql = "DELETE FROM BaiVietCamNang WHERE maBaiViet = @MaBaiViet";
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaBaiViet", maBaiViet);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        // gán điều kiện lọc
        private static void GanDieuKienLoc(StringBuilder sql, SqlCommand lenh, string? loaiBaiViet, string? keyword, bool? featured)
        {
            var loaiDaChuanHoa = ChuanHoaLoaiBaiViet(loaiBaiViet);
            if (loaiDaChuanHoa == LoaiTinTuc)
            {
                sql.Append(" AND bv.loaiBaiViet = N'Tin tức'");
            }
            else if (loaiDaChuanHoa == LoaiCamNang)
            {
                sql.Append(" AND bv.loaiBaiViet IN (N'Cẩm nang sức khỏe', N'Cẩm nang')");
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql.Append(" AND (bv.tieuDe LIKE @Keyword OR bv.moTaNgan LIKE @Keyword OR bv.noiDung LIKE @Keyword)");
                lenh.Parameters.AddWithValue("@Keyword", $"%{keyword.Trim()}%");
            }

            if (featured == true)
            {
                sql.Append(" AND bv.noiBat = 1");
            }
        }

        // gán tham số bài viết
        private static void GanThamSoBaiViet(SqlCommand lenh, BaiVietCamNang bv)
        {
            lenh.Parameters.AddWithValue("@MaBaiViet", bv.MaBaiViet);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", bv.MaTaiKhoan);
            lenh.Parameters.AddWithValue("@TieuDe", bv.TieuDe);
            lenh.Parameters.AddWithValue("@Slug", bv.Slug);
            lenh.Parameters.AddWithValue("@MoTaNgan", bv.MoTaNgan);
            lenh.Parameters.AddWithValue("@NoiDung", bv.NoiDung);
            lenh.Parameters.AddWithValue("@LoaiBaiViet", bv.LoaiBaiViet);
            lenh.Parameters.AddWithValue("@AnhDaiDien", string.IsNullOrWhiteSpace(bv.AnhDaiDien) ? DBNull.Value : bv.AnhDaiDien);
            lenh.Parameters.AddWithValue("@NgayTao", bv.NgayTao == default ? DateTime.Now : bv.NgayTao);
            lenh.Parameters.AddWithValue("@TrangThai", bv.TrangThai);
            lenh.Parameters.AddWithValue("@NoiBat", bv.NoiBat);
            lenh.Parameters.AddWithValue("@LuotXem", Math.Max(0, bv.LuotXem));
        }

        // chạy lệnh sql
        private static void ChayLenh(SqlConnection ketNoi, string sql)
        {
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.ExecuteNonQuery();
        }

        // đọc danh sách từ sql
        private List<BaiVietCamNang> DocDanhSach(string sql)
        {
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return DocDanhSachTuReader(doc);
        }

        // đọc danh sách từ reader
        private static List<BaiVietCamNang> DocDanhSachTuReader(SqlDataReader doc)
        {
            var danhSach = new List<BaiVietCamNang>();
            while (doc.Read())
            {
                danhSach.Add(DocBaiViet(doc));
            }

            return danhSach;
        }

        // đọc bài viết từ reader
        private static BaiVietCamNang DocBaiViet(SqlDataReader doc)
        {
            return new BaiVietCamNang
            {
                MaBaiViet = Convert.ToInt32(doc["maBaiViet"]),
                MaTaiKhoan = Convert.ToInt32(doc["maTaiKhoan"]),
                TieuDe = doc["tieuDe"] == DBNull.Value ? string.Empty : doc["tieuDe"].ToString() ?? string.Empty,
                Slug = doc["slug"] == DBNull.Value ? string.Empty : doc["slug"].ToString() ?? string.Empty,
                MoTaNgan = doc["moTaNgan"] == DBNull.Value ? string.Empty : doc["moTaNgan"].ToString() ?? string.Empty,
                NoiDung = doc["noiDung"] == DBNull.Value ? string.Empty : doc["noiDung"].ToString() ?? string.Empty,
                LoaiBaiViet = ChuanHoaLoaiBaiViet(doc["loaiBaiViet"] == DBNull.Value ? null : doc["loaiBaiViet"].ToString()) ?? LoaiCamNang,
                AnhDaiDien = doc["anhDaiDien"] == DBNull.Value ? string.Empty : doc["anhDaiDien"].ToString() ?? string.Empty,
                NgayTao = Convert.ToDateTime(doc["ngayTao"]),
                TrangThai = Convert.ToBoolean(doc["trangThai"]),
                NoiBat = doc["noiBat"] != DBNull.Value && Convert.ToBoolean(doc["noiBat"]),
                LuotXem = doc["luotXem"] == DBNull.Value ? 0 : Convert.ToInt32(doc["luotXem"]),
                TenTacGia = doc["tenTacGia"] == DBNull.Value ? string.Empty : doc["tenTacGia"].ToString() ?? string.Empty
            };
        }

        // tạo mô tả ngắn
        private static string TaoMoTaNgan(string? moTa, string noiDung)
        {
            var text = string.IsNullOrWhiteSpace(moTa) ? noiDung : moTa;
            text = Regex.Replace(text ?? string.Empty, "<.*?>", " ");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return text.Length <= 240 ? text : text[..240];
        }

        // tạo slug từ tiêu đề
        public static string TaoSlug(string text)
        {
            text = string.IsNullOrWhiteSpace(text) ? "bai-viet" : text.Trim().ToLowerInvariant();
            var normalized = text.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();
            foreach (var c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c == 'đ' ? 'd' : c);
                }
            }

            var slug = Regex.Replace(builder.ToString().Normalize(NormalizationForm.FormC), @"[^a-z0-9]+", "-").Trim('-');
            return string.IsNullOrWhiteSpace(slug) ? "bai-viet" : slug;
        }

        // chuẩn hóa loại bài viết
        public static string? ChuanHoaLoaiBaiViet(string? loaiBaiViet)
        {
            if (string.IsNullOrWhiteSpace(loaiBaiViet))
            {
                return null;
            }

            return loaiBaiViet.Trim() switch
            {
                "news" => LoaiTinTuc,
                "Tin tức" => LoaiTinTuc,
                "tin-tuc" => LoaiTinTuc,
                "Tin tuc" => LoaiTinTuc,
                "guide" => LoaiCamNang,
                "Cẩm nang" => LoaiCamNang,
                "Cẩm nang sức khỏe" => LoaiCamNang,
                "cam-nang" => LoaiCamNang,
                "cam-nang-suc-khoe" => LoaiCamNang,
                "Cam nang" => LoaiCamNang,
                _ => null
            };
        }

        // lấy loại api
        private static string LayApiType(BaiVietCamNang baiViet)
        {
            return baiViet.LoaiBaiViet == LoaiTinTuc ? "news" : "guide";
        }

        // tạo dto cho bài viết
        public static object TaoArticleDto(BaiVietCamNang baiViet)
        {
            var type = LayApiType(baiViet);
            return new
            {
                id = baiViet.MaBaiViet,
                title = baiViet.TieuDe,
                slug = string.IsNullOrWhiteSpace(baiViet.Slug) ? TaoSlug(baiViet.TieuDe) : baiViet.Slug,
                excerpt = baiViet.MoTaNgan,
                content = baiViet.NoiDung,
                thumbnail_url = string.IsNullOrWhiteSpace(baiViet.AnhDaiDien) ? null : baiViet.AnhDaiDien,
                type,
                type_label = type == "news" ? LoaiTinTuc : LoaiCamNang,
                status = baiViet.TrangThai ? "published" : "hidden",
                is_featured = baiViet.NoiBat,
                view_count = baiViet.LuotXem,
                published_at = baiViet.NgayTao,
                created_at = baiViet.NgayTao,
                author = baiViet.TenTacGia
            };
        }

        // seed dữ liệu mẫu nếu chưa có
        private static void SeedNeuChuaCoDuLieu(SqlConnection ketNoi)
        {
            using (var dem = new SqlCommand("SELECT COUNT(*) FROM dbo.BaiVietCamNang", ketNoi))
            {
                if (Convert.ToInt32(dem.ExecuteScalar()) > 0)
                {
                    return;
                }
            }

            const string sql = @"
DECLARE @MaTaiKhoan INT = (SELECT TOP 1 maTaiKhoan FROM dbo.TaiKhoan ORDER BY maTaiKhoan);
IF @MaTaiKhoan IS NULL RETURN;

INSERT INTO dbo.BaiVietCamNang(maTaiKhoan, tieuDe, slug, moTaNgan, noiDung, loaiBaiViet, anhDaiDien, ngayTao, trangThai, noiBat, luotXem)
VALUES
(@MaTaiKhoan, N'Bộ Y tế khuyến cáo tiêm vaccine cúm mùa trước thời điểm giao mùa', N'bo-y-te-khuyen-cao-tiem-vaccine-cum-mua-truoc-thoi-diem-giao-mua', N'Tiêm vaccine cúm mùa giúp giảm nguy cơ mắc bệnh và các biến chứng nặng.', N'Tiêm vaccine cúm mùa giúp giảm nguy cơ mắc bệnh và các biến chứng nặng, đặc biệt với trẻ nhỏ, người cao tuổi và người có bệnh nền.', N'Tin tức', N'/images/articles/chuan-bi-truoc-tiem.svg', '2026-06-09', 1, 1, 1256),
(@MaTaiKhoan, N'Trẻ em cần tiêm những loại vaccine nào trong năm đầu đời?', N'tre-em-can-tiem-nhung-loai-vaccine-nao-trong-nam-dau-doi', N'Hướng dẫn chi tiết lịch tiêm chủng cho trẻ từ 0 - 12 tháng tuổi.', N'Năm đầu đời là giai đoạn quan trọng để trẻ hình thành miễn dịch chủ động. Cha mẹ nên theo dõi lịch tiêm để không bỏ sót mũi quan trọng.', N'Cẩm nang sức khỏe', N'/images/articles/lich-tiem-tre-nho.svg', '2026-06-08', 1, 1, 2340),
(@MaTaiKhoan, N'Gia tăng ca mắc sởi tại nhiều địa phương, Bộ Y tế kêu gọi tiêm chủng đầy đủ', N'gia-tang-ca-mac-soi-tai-nhieu-dia-phuong', N'Sởi có thể gây biến chứng nặng, đặc biệt ở trẻ nhỏ chưa được tiêm vaccine.', N'Sởi là bệnh truyền nhiễm lây lan nhanh. Tiêm vaccine đúng lịch giúp giảm nguy cơ mắc bệnh và hạn chế biến chứng nặng.', N'Tin tức', N'/images/articles/theo-doi-sau-tiem.svg', '2026-06-07', 1, 0, 1890),
(@MaTaiKhoan, N'5 nhóm thực phẩm giúp tăng cường miễn dịch cho cả gia đình', N'5-nhom-thuc-pham-giup-tang-cuong-mien-dich-cho-ca-gia-dinh', N'Dinh dưỡng hợp lý giúp cơ thể khỏe mạnh và phòng ngừa bệnh tật hiệu quả.', N'Một chế độ ăn cân bằng với rau xanh, trái cây, đạm tốt, ngũ cốc nguyên hạt và nước đầy đủ sẽ hỗ trợ hệ miễn dịch hoạt động bền bỉ.', N'Cẩm nang sức khỏe', N'/images/articles/lich-su-tiem-chung.svg', '2026-06-06', 1, 0, 1102),
(@MaTaiKhoan, N'Mở rộng độ tuổi tiêm vaccine HPV đến 26 tuổi cho cả nam và nữ', N'mo-rong-do-tuoi-tiem-vaccine-hpv-den-26-tuoi', N'Quyết định mới giúp nhiều người trẻ có cơ hội phòng ngừa ung thư hiệu quả hơn.', N'Vaccine HPV giúp phòng ngừa nhiều bệnh lý liên quan đến HPV. Người dùng nên trao đổi với nhân viên y tế để được tư vấn phác đồ phù hợp.', N'Tin tức', N'/images/articles/chuan-bi-truoc-tiem.svg', '2026-06-05', 1, 0, 980),
(@MaTaiKhoan, N'Hướng dẫn chăm sóc trẻ sau tiêm vaccine tại nhà', N'huong-dan-cham-soc-tre-sau-tiem-vaccine-tai-nha', N'Những điều cha mẹ cần lưu ý để trẻ an toàn và khỏe mạnh sau tiêm chủng.', N'Sau tiêm, cha mẹ nên theo dõi thân nhiệt, vùng tiêm và biểu hiện toàn thân của trẻ. Khi có dấu hiệu bất thường cần liên hệ cơ sở y tế.', N'Cẩm nang sức khỏe', N'/images/articles/theo-doi-sau-tiem.svg', '2026-06-04', 1, 1, 1540);";

            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.ExecuteNonQuery();
        }
    }
}
