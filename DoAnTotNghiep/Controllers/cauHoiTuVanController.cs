using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class cauHoiTuVanController : Controller
    {
        private readonly cauHoiTuVanDAL cauHoiTuVanDAL;

        public cauHoiTuVanController(cauHoiTuVanDAL cauHoiTuVanDAL)
        {
            this.cauHoiTuVanDAL = cauHoiTuVanDAL;
        }

        public IActionResult index()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return RedirectToAction("dangNhap", "taiKhoan");
            if (laAdmin()) return View(cauHoiTuVanDAL.layTatCa());
            return View(cauHoiTuVanDAL.layTheoNguoiGui(maTaiKhoan.Value));
        }

        [HttpGet]
        public IActionResult guiCauHoi()
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") == null) return RedirectToAction("dangNhap", "taiKhoan");
            return View(new cauHoiTuVanModels());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult guiCauHoi(cauHoiTuVanModels model)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return RedirectToAction("dangNhap", "taiKhoan");
            if (!ModelState.IsValid) return View(model);
            model.maNguoiGui = maTaiKhoan.Value;
            cauHoiTuVanDAL.them(model);
            return RedirectToAction(nameof(index));
        }

        [HttpGet]
        public IActionResult traLoi(int id)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            var cauHoi = cauHoiTuVanDAL.layTheoMa(id);
            return cauHoi == null ? NotFound() : View(cauHoi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult traLoi(int maCauHoi, string cauTraLoi)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            var maNguoiTraLoi = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
            cauHoiTuVanDAL.traLoi(maCauHoi, maNguoiTraLoi, cauTraLoi);
            return RedirectToAction(nameof(index));
        }

        private bool laAdmin()
        {
            return string.Equals(HttpContext.Session.GetString("VaiTro"), "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
