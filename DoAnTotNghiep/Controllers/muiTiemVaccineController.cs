using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoAnTotNghiep.Controllers
{
    public class muiTiemVaccineController : Controller
    {
        private readonly muiTiemVaccineDAL muiTiemVaccineDAL;
        private readonly vaccineDAL vaccineDAL;

        public muiTiemVaccineController(muiTiemVaccineDAL muiTiemVaccineDAL, vaccineDAL vaccineDAL)
        {
            this.muiTiemVaccineDAL = muiTiemVaccineDAL;
            this.vaccineDAL = vaccineDAL;
        }

        public IActionResult index()
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            return View(muiTiemVaccineDAL.layTatCa());
        }

        [HttpGet]
        public IActionResult them()
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            napDanhSachVaccine();
            return View(new muiTiemVaccineModels());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult them(muiTiemVaccineModels model)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            if (!ModelState.IsValid)
            {
                napDanhSachVaccine();
                return View(model);
            }
            muiTiemVaccineDAL.them(model);
            return RedirectToAction(nameof(index));
        }

        [HttpGet]
        public IActionResult sua(int id)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            var muiTiem = muiTiemVaccineDAL.layTheoMa(id);
            if (muiTiem == null) return NotFound();
            napDanhSachVaccine();
            return View(muiTiem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult sua(muiTiemVaccineModels model)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            if (!ModelState.IsValid)
            {
                napDanhSachVaccine();
                return View(model);
            }
            muiTiemVaccineDAL.capNhat(model);
            return RedirectToAction(nameof(index));
        }

        private void napDanhSachVaccine()
        {
            ViewBag.DanhSachVaccine = new SelectList(vaccineDAL.layTatCa(), "maVaccine", "tenVaccine");
        }

        private bool laAdmin()
        {
            return string.Equals(HttpContext.Session.GetString("VaiTro"), "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
