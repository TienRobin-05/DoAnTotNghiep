using DoAnTotNghiep.DAL;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class BaiVietCamNangController : Controller
    {
        private readonly BaiVietCamNang_DAL baiVietDAL;

        public BaiVietCamNangController(BaiVietCamNang_DAL baiVietDAL)
        {
            this.baiVietDAL = baiVietDAL;
        }

        // hiển thị trang kiến thức
        public IActionResult Index(string? loai)
        {
            return View();
        }

        // xem chi tiết bài viết
        public IActionResult Details(int maBaiViet)
        {
            var baiViet = baiVietDAL.LayTheoId(maBaiViet);
            if (baiViet == null || !baiViet.TrangThai)
            {
                return NotFound();
            }

            baiVietDAL.TangLuotXem(baiViet.MaBaiViet);
            baiViet.LuotXem += 1;
            return View(baiViet);
        }

        [HttpGet("/api/knowledge/articles")]
        public IActionResult GetArticles(string? type, string? keyword, string? sort, int page = 1, int limit = 6, bool? featured = null)
        {
            page = Math.Max(1, page);
            limit = Math.Clamp(limit, 1, 24);
            var articles = baiVietDAL.LayDanhSachChoUser(type, keyword, sort, page, limit, featured);
            var total = baiVietDAL.DemDanhSachChoUser(type, keyword, featured);
            var totalPages = (int)Math.Ceiling(total / (double)limit);

            if (featured == true && articles.Count == 0)
            {
                articles = baiVietDAL.LayDanhSachChoUser(type, keyword, "most_viewed", page, limit, null);
                total = baiVietDAL.DemDanhSachChoUser(type, keyword, null);
                totalPages = (int)Math.Ceiling(total / (double)limit);
            }

            return Json(new
            {
                data = articles.Select(BaiVietCamNang_DAL.TaoArticleDto),
                pagination = new
                {
                    page,
                    limit,
                    total,
                    total_pages = totalPages
                }
            });
        }

        [HttpGet("/api/knowledge/stats")]
        public IActionResult GetStats()
        {
            var stats = baiVietDAL.LayThongKeUser();
            return Json(new { total = stats.Total, news = stats.News, guide = stats.Guide });
        }

        [HttpGet("/api/knowledge/articles/{slug}")]
        public IActionResult GetArticleDetail(string slug)
        {
            var article = baiVietDAL.LayTheoSlug(slug);
            if (article == null)
            {
                return NotFound();
            }

            baiVietDAL.TangLuotXem(article.MaBaiViet);
            article.LuotXem += 1;
            return Json(BaiVietCamNang_DAL.TaoArticleDto(article));
        }
    }
}
