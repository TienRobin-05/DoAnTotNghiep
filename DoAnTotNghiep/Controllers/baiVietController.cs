using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    /// <summary>
    /// Controller tương thích cho đường dẫn bài viết cũ, chuyển về luồng BaiVietCamNang/AdminBaiViet hiện tại.
    /// </summary>
    public class baiVietController : Controller
    {
        public IActionResult index()
        {
            return RedirectToAction("Index", "BaiVietCamNang");
        }

        public IActionResult chiTiet(int id)
        {
            return RedirectToAction("Details", "BaiVietCamNang", new { maBaiViet = id });
        }

        public IActionResult them()
        {
            return RedirectToAction("Create", "AdminBaiViet");
        }

        public IActionResult sua(int id)
        {
            return RedirectToAction("Edit", "AdminBaiViet", new { maBaiViet = id });
        }
    }
}
