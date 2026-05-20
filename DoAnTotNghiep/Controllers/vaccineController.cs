using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class vaccineController : Controller
    {
        private readonly vaccineDAL vaccineDAL;

        public vaccineController(vaccineDAL vaccineDAL)
        {
            this.vaccineDAL = vaccineDAL;
        }

        public IActionResult index()
        {
            return View(vaccineDAL.layTatCa(chiLayDangHoatDong: !laAdmin()));
        }

        [HttpGet]
        public IActionResult them()
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            return View(new vaccineModels());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult them(vaccineModels model)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            if (!ModelState.IsValid) return View(model);
            vaccineDAL.them(model);
            return RedirectToAction(nameof(index));
        }

        [HttpGet]
        public IActionResult sua(int id)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            var vaccine = vaccineDAL.layTheoMa(id);
            return vaccine == null ? NotFound() : View(vaccine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult sua(vaccineModels model)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            if (!ModelState.IsValid) return View(model);
            vaccineDAL.capNhat(model);
            return RedirectToAction(nameof(index));
        }

        private bool laAdmin()
        {
            return string.Equals(HttpContext.Session.GetString("VaiTro"), "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
