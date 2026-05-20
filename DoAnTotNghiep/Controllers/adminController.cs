using DoAnTotNghiep.DAL;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class AdminController : Controller
    {
        private readonly taiKhoanDAL taiKhoanDAL;
        private readonly vaccineDAL vaccineDAL;
        private readonly lichTiemDAL lichTiemDAL;
        private readonly cauHoiTuVanDAL cauHoiTuVanDAL;

        public AdminController(taiKhoanDAL taiKhoanDAL, vaccineDAL vaccineDAL, lichTiemDAL lichTiemDAL, cauHoiTuVanDAL cauHoiTuVanDAL)
        {
            this.taiKhoanDAL = taiKhoanDAL;
            this.vaccineDAL = vaccineDAL;
            this.lichTiemDAL = lichTiemDAL;
            this.cauHoiTuVanDAL = cauHoiTuVanDAL;
        }

        public IActionResult Index()
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            ViewBag.HoTen = HttpContext.Session.GetString("HoTen") ?? string.Empty;
            ViewBag.SoTaiKhoan = taiKhoanDAL.layTatCa().Count;
            ViewBag.SoVaccine = vaccineDAL.layTatCa().Count;
            ViewBag.SoLichTiem = lichTiemDAL.layTatCa().Count;
            ViewBag.SoCauHoiChoTraLoi = cauHoiTuVanDAL.layTatCa().Count(x => x.trangThai != "Đã trả lời");
            return View();
        }

        public IActionResult taiKhoan()
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            return View(taiKhoanDAL.layTatCa());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult doiTrangThaiTaiKhoan(int maTaiKhoan, bool trangThai)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            taiKhoanDAL.doiTrangThai(maTaiKhoan, trangThai);
            return RedirectToAction(nameof(taiKhoan));
        }

        private bool LaAdmin()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            return maTaiKhoan != null
                && string.Equals(HttpContext.Session.GetString("VaiTro"), "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
