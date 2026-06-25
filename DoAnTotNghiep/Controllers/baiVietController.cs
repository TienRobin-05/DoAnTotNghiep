using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
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
