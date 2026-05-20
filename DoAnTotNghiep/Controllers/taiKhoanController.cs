using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class taiKhoanController : Controller
    {
        private readonly taiKhoanDAL taiKhoanDAL;

        public taiKhoanController(taiKhoanDAL taiKhoanDAL)
        {
            this.taiKhoanDAL = taiKhoanDAL;
        }

        [HttpGet]
        public IActionResult dangNhap()
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                return chuyenTrangTheoVaiTro(HttpContext.Session.GetString("VaiTro"));
            }
            return View(new dangNhapViewModels());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult dangNhap(dangNhapViewModels model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var taiKhoan = taiKhoanDAL.dangNhap(model.email, model.matKhau);
            if (taiKhoan == null)
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng, hoặc tài khoản đã bị khóa.");
                return View(model);
            }

            HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.maTaiKhoan);
            HttpContext.Session.SetString("HoTen", taiKhoan.hoTen);
            HttpContext.Session.SetString("VaiTro", taiKhoan.vaiTro);

            return chuyenTrangTheoVaiTro(taiKhoan.vaiTro);
        }

        [HttpGet]
        public IActionResult dangKy()
        {
            return View(new dangKyViewModels());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult dangKy(dangKyViewModels model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (taiKhoanDAL.emailDaTonTai(model.email))
            {
                ModelState.AddModelError(nameof(model.email), "Email đã được sử dụng.");
                return View(model);
            }

            taiKhoanDAL.them(new taiKhoanModels
            {
                hoTen = model.hoTen,
                email = model.email,
                matKhau = model.matKhau,
                soDienThoai = model.soDienThoai,
                vaiTro = "User",
                trangThai = true
            });

            TempData["thongBao"] = "Đăng ký thành công. Vui lòng đăng nhập.";
            return RedirectToAction(nameof(dangNhap));
        }

        public IActionResult dangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(dangNhap));
        }

        private IActionResult chuyenTrangTheoVaiTro(string? vaiTro)
        {
            if (string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("index", "admin");
            }
            return RedirectToAction("index", "nguoiDung");
        }
    }
}
