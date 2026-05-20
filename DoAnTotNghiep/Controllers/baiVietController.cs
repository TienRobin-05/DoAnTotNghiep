using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class baiVietController : Controller
    {
        private readonly baiVietCamNangDAL baiVietCamNangDAL;

        public baiVietController(baiVietCamNangDAL baiVietCamNangDAL)
        {
            this.baiVietCamNangDAL = baiVietCamNangDAL;
        }

        public IActionResult index()
        {
            return View(baiVietCamNangDAL.layTatCa(chiLayDangHienThi: !laAdmin()));
        }

        public IActionResult chiTiet(int id)
        {
            var baiViet = baiVietCamNangDAL.layTheoMa(id);
            return baiViet == null ? NotFound() : View(baiViet);
        }

        [HttpGet]
        public IActionResult them()
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            return View(new baiVietCamNangModels());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult them(baiVietCamNangModels model)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            if (!ModelState.IsValid) return View(model);
            model.maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
            baiVietCamNangDAL.them(model);
            return RedirectToAction(nameof(index));
        }

        [HttpGet]
        public IActionResult sua(int id)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            var baiViet = baiVietCamNangDAL.layTheoMa(id);
            return baiViet == null ? NotFound() : View(baiViet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult sua(baiVietCamNangModels model)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            if (!ModelState.IsValid) return View(model);
            baiVietCamNangDAL.capNhat(model);
            return RedirectToAction(nameof(index));
        }

        private bool laAdmin()
        {
            return string.Equals(HttpContext.Session.GetString("VaiTro"), "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
