using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class NguoiDungController : Controller
    {
        public IActionResult Index()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (!string.Equals(HttpContext.Session.GetString("VaiTro"), "User", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            ViewBag.HoTen = HttpContext.Session.GetString("HoTen") ?? string.Empty;
            return View();
        }
    }
}
