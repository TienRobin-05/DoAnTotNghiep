using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly TaiKhoan_DAL taiKhoanDAL;
        private readonly HoSoSucKhoe_DAL hoSoSucKhoeDAL;

        public TaiKhoanController(TaiKhoan_DAL taiKhoanDAL, HoSoSucKhoe_DAL hoSoSucKhoeDAL)
        {
            this.taiKhoanDAL = taiKhoanDAL;
            this.hoSoSucKhoeDAL = hoSoSucKhoeDAL;
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangKy(string hoTen, string email, string soDienThoai, string matKhau, string nhapLaiMatKhau)
        {
            if (string.IsNullOrWhiteSpace(hoTen))
            {
                ViewBag.ThongBao = "Họ tên không được bỏ trống";
                return View();
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.ThongBao = "Email không được bỏ trống";
                return View();
            }
            if (string.IsNullOrWhiteSpace(soDienThoai))
            {
                ViewBag.ThongBao = "Số điện thoại không được bỏ trống";
                return View();
            }
            if (string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.ThongBao = "Mật khẩu không được bỏ trống";
                return View();
            }
            if (string.IsNullOrWhiteSpace(nhapLaiMatKhau))
            {
                ViewBag.ThongBao = "Nhập lại mật khẩu không được bỏ trống";
                return View();
            }
            if (matKhau != nhapLaiMatKhau)
            {
                ViewBag.ThongBao = "Nhập lại mật khẩu không trùng khớp";
                return View();
            }
            if (taiKhoanDAL.KiemTraSoDienThoaiTonTai(soDienThoai))
            {
                ViewBag.ThongBao = "Số điện thoại đã được sử dụng";
                return View();
            }
            if (taiKhoanDAL.KiemTraEmailTonTai(email))
            {
                ViewBag.ThongBao = "Email đã được sử dụng";
                return View();
            }

            var taiKhoan = new TaiKhoan
            {
                HoTen = hoTen,
                Email = email,
                MatKhau = matKhau,
                SoDienThoai = soDienThoai,
                VaiTro = "User",
                TrangThai = true,
                NgayTao = DateTime.Now
            };

            if (taiKhoanDAL.DangKy(taiKhoan))
            {
                TempData["ThongBao"] = "Đăng ký thành công. Vui lòng đăng nhập.";
                return RedirectToAction(nameof(DangNhap));
            }

            ViewBag.ThongBao = "Đăng ký thất bại, vui lòng thử lại";
            return View();
        }

        [HttpGet]
        public IActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangNhap(string soDienThoai, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(soDienThoai))
            {
                ViewBag.ThongBao = "Số điện thoại không được bỏ trống";
                return View();
            }
            if (string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.ThongBao = "Mật khẩu không được bỏ trống";
                return View();
            }

            var taiKhoan = taiKhoanDAL.DangNhap(soDienThoai, matKhau);
            if (taiKhoan == null)
            {
                ViewBag.ThongBao = "Số điện thoại hoặc mật khẩu không đúng";
                return View();
            }
            if (!taiKhoan.TrangThai)
            {
                ViewBag.ThongBao = "Tài khoản đã bị khóa";
                return View();
            }

            HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", taiKhoan.HoTen ?? string.Empty);
            HttpContext.Session.SetString("VaiTro", taiKhoan.VaiTro);
            HttpContext.Session.SetString("SoDienThoai", taiKhoan.SoDienThoai);
            HttpContext.Session.SetString("Email", taiKhoan.Email ?? string.Empty);

            if (string.Equals(taiKhoan.VaiTro, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Admin");
            }

            if (string.Equals(taiKhoan.VaiTro, "User", StringComparison.OrdinalIgnoreCase))
            {
                if (!hoSoSucKhoeDAL.KiemTraTaiKhoanDaCoHoSo(taiKhoan.MaTaiKhoan))
                {
                    return RedirectToAction("CapNhatThongTinCaNhan", "HoSoSucKhoe");
                }

                return RedirectToAction("Index", "NguoiDung");
            }

            ViewBag.ThongBao = "Vai trò tài khoản không hợp lệ";
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(DangNhap));
        }
    }
}
