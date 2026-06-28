using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace DoAnTotNghiep.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly TaiKhoan_DAL taiKhoanDAL;
        private readonly HoSoSucKhoe_DAL hoSoSucKhoeDAL;
        private readonly IWebHostEnvironment webHostEnvironment;

        public TaiKhoanController(TaiKhoan_DAL taiKhoanDAL, HoSoSucKhoe_DAL hoSoSucKhoeDAL, IWebHostEnvironment webHostEnvironment)
        {
            this.taiKhoanDAL = taiKhoanDAL;
            this.hoSoSucKhoeDAL = hoSoSucKhoeDAL;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        // hiển thị form đăng ký
        public IActionResult DangKy()
        {
            XoaPhienDatLaiMatKhau();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // hiển thị form đăng ký
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
            if (!EmailHopLe(email))
            {
                ViewBag.ThongBao = "Email không đúng định dạng";
                return View();
            }
            if (string.IsNullOrWhiteSpace(soDienThoai))
            {
                ViewBag.ThongBao = "Số điện thoại không được bỏ trống";
                return View();
            }
            if (!SoDienThoaiHopLe(soDienThoai))
            {
                ViewBag.ThongBao = "Số điện thoại phải gồm 10-11 chữ số và bắt đầu bằng 0";
                return View();
            }
            if (string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.ThongBao = "Mật khẩu không được bỏ trống";
                return View();
            }
            if (!MatKhauHopLe(matKhau))
            {
                ViewBag.ThongBao = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường và số";
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
                ViewBag.ThongBao = "Số điện thoại hoặc email đã tồn tại";
                return View();
            }
            if (taiKhoanDAL.KiemTraEmailTonTai(email))
            {
                ViewBag.ThongBao = "Số điện thoại hoặc email đã tồn tại";
                return View();
            }

            var taiKhoan = new TaiKhoan
            {
                HoTen = hoTen,
                Email = email.Trim(),
                MatKhau = matKhau,
                SoDienThoai = soDienThoai.Trim(),
                VaiTro = "User",
                TrangThai = true,
                NgayTao = DateTime.Now
            };

            if (taiKhoanDAL.DangKy(taiKhoan))
            {
                TempData["ThongBao"] = "Đăng ký thành công";
                return RedirectToAction(nameof(DangNhap));
            }

            ViewBag.ThongBao = "Đăng ký thất bại, vui lòng thử lại";
            return View();
        }

        [HttpGet]
        // hiển thị form đăng nhập
        public IActionResult DangNhap()
        {
            XoaPhienDatLaiMatKhau();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // hiển thị form đăng nhập
        public IActionResult DangNhap(string soDienThoaiDangNhap, string matKhauDangNhap)
        {
            // Đăng nhập chỉ dùng số điện thoại và mật khẩu, không dùng email.
            var soDienThoai = soDienThoaiDangNhap;
            var matKhau = matKhauDangNhap;

            if (string.IsNullOrWhiteSpace(soDienThoai) || string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.ThongBao = "Số điện thoại hoặc mật khẩu không được bỏ trống";
                return View();
            }

            var taiKhoan = taiKhoanDAL.DangNhap(soDienThoai, matKhau);
            if (taiKhoan == null)
            {
                ViewBag.ThongBao = "Số điện thoại hoặc mật khẩu không đúng";
                return View();
            }

            if (taiKhoan.DaXoa)
            {
                ViewBag.ThongBao = "Tài khoản đã bị xóa do không hoạt động quá lâu.";
                return View();
            }

            if (!taiKhoan.TrangThai)
            {
                ViewBag.ThongBao = "Tài khoản đang bị khóa hoặc không hoạt động";
                return View();
            }

            taiKhoanDAL.CapNhatLanDangNhapCuoi(taiKhoan.MaTaiKhoan);

            HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", taiKhoan.HoTen ?? "");
            var vaiTro = ChuanHoaVaiTro(taiKhoan.VaiTro);
            HttpContext.Session.SetString("VaiTro", vaiTro);
            HttpContext.Session.SetString("SoDienThoai", taiKhoan.SoDienThoai ?? "");
            HttpContext.Session.SetString("Email", taiKhoan.Email ?? "");

            // Chỉ chấp nhận đúng hai vai trò trong database: Admin và User.
            if (vaiTro == "Admin")
            {
                TempData["ThongBao"] = "Đăng nhập thành công";
                return RedirectToAction("Index", "Admin");
            }

            if (vaiTro == "User")
            {
                TempData["ThongBao"] = "Đăng nhập thành công";
                return RedirectToAction("Index", "NguoiDung");
            }

            ViewBag.ThongBao = "Vai trò tài khoản không hợp lệ";
            HttpContext.Session.Clear();
            return View();
        }

        [HttpGet]
        // hiển thị form quên mật khẩu
        public IActionResult QuenMatKhau()
        {
            GanTrangThaiNhapMaXacNhan();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // hiển thị form quên mật khẩu
        public IActionResult QuenMatKhau(string soDienThoai, string email, string matKhauMoi, string xacNhanMatKhau, string maXacNhan)
        {
            soDienThoai = soDienThoai?.Trim() ?? string.Empty;
            email = email?.Trim() ?? string.Empty;
            maXacNhan = maXacNhan?.Trim() ?? string.Empty;

            ViewBag.SoDienThoai = soDienThoai;
            ViewBag.Email = email;

            if (!KiemTraThongTinTaiKhoanDatLaiMatKhau(soDienThoai, email, out var taiKhoan))
            {
                return View();
            }

            var maXacNhanTrongPhien = HttpContext.Session.GetString("DatLaiMatKhau_MaXacNhan");
            var maTaiKhoanTrongPhien = HttpContext.Session.GetInt32("DatLaiMatKhau_MaTaiKhoan");
            var hetHanTicks = HttpContext.Session.GetString("DatLaiMatKhau_HetHanTicks");
            var daCoMaHopLe = !string.IsNullOrWhiteSpace(maXacNhanTrongPhien)
                && maTaiKhoanTrongPhien == taiKhoan!.MaTaiKhoan
                && long.TryParse(hetHanTicks, out var ticks)
                && new DateTime(ticks, DateTimeKind.Utc) >= DateTime.UtcNow;

            if (!daCoMaHopLe || string.IsNullOrWhiteSpace(maXacNhan))
            {
                var maMoi = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
                HttpContext.Session.SetInt32("DatLaiMatKhau_MaTaiKhoan", taiKhoan!.MaTaiKhoan);
                HttpContext.Session.SetString("DatLaiMatKhau_MaXacNhan", maMoi);
                HttpContext.Session.SetString("DatLaiMatKhau_HetHanTicks", DateTime.UtcNow.AddMinutes(10).Ticks.ToString());
                ViewBag.YeuCauMaXacNhan = true;
                if (webHostEnvironment.IsDevelopment())
                {
                    ViewBag.MaXacNhanDemo = maMoi;
                }

                ViewBag.ThongBao = webHostEnvironment.IsDevelopment()
                    ? "Đã tạo mã xác nhận demo. Nhập mã này để đặt lại mật khẩu."
                    : "Đã tạo mã xác nhận. Vui lòng kiểm tra kênh nhận mã đã cấu hình.";
                return View();
            }

            ViewBag.YeuCauMaXacNhan = true;
            if (string.IsNullOrWhiteSpace(matKhauMoi))
            {
                ViewBag.ThongBao = "Mật khẩu mới không được bỏ trống";
                return View();
            }

            if (!MatKhauHopLe(matKhauMoi))
            {
                ViewBag.ThongBao = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường và số";
                return View();
            }

            if (string.IsNullOrWhiteSpace(xacNhanMatKhau))
            {
                ViewBag.ThongBao = "Xác nhận mật khẩu không được bỏ trống";
                return View();
            }

            if (matKhauMoi != xacNhanMatKhau)
            {
                ViewBag.ThongBao = "Mật khẩu mới và xác nhận mật khẩu không trùng khớp";
                return View();
            }

            if (!string.Equals(maXacNhan, maXacNhanTrongPhien, StringComparison.Ordinal))
            {
                ViewBag.ThongBao = "Mã xác nhận không đúng hoặc đã hết hạn.";
                return View();
            }

            if (!taiKhoanDAL.DatLaiMatKhau(taiKhoan!.MaTaiKhoan, matKhauMoi))
            {
                ViewBag.ThongBao = "Đặt lại mật khẩu thất bại, vui lòng thử lại.";
                return View();
            }

            XoaPhienDatLaiMatKhau();
            TempData["ThongBao"] = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại.";
            return RedirectToAction(nameof(DangNhap));
        }

        // đăng xuất tài khoản
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            TempData["ThongBao"] = "Đăng xuất thành công";
            return RedirectToAction(nameof(DangNhap));
        }

        [HttpGet]
        // hiển thị thông tin cá nhân
        public IActionResult ThongTinCaNhan()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction(nameof(DangNhap));
            }

            var taiKhoan = taiKhoanDAL.LayTaiKhoanTheoId(maTaiKhoan.Value);
            if (taiKhoan == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction(nameof(DangNhap));
            }

            var hoSoCaNhan = hoSoSucKhoeDAL.LayHoSoDauTienTheoTaiKhoan(maTaiKhoan.Value);
            var model = new ThongTinCaNhanViewModel
            {
                HoTen = !string.IsNullOrWhiteSpace(hoSoCaNhan?.HoTen) ? hoSoCaNhan.HoTen : taiKhoan.HoTen,
                SoDienThoai = taiKhoan.SoDienThoai,
                Email = taiKhoan.Email,
                GioiTinh = hoSoCaNhan?.GioiTinh ?? string.Empty,
                NgaySinh = hoSoCaNhan == null || hoSoCaNhan.NgaySinh == default ? null : hoSoCaNhan.NgaySinh
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // hiển thị thông tin cá nhân
        public IActionResult ThongTinCaNhan(ThongTinCaNhanViewModel model)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction(nameof(DangNhap));
            }

            var taiKhoan = taiKhoanDAL.LayTaiKhoanTheoId(maTaiKhoan.Value);
            if (taiKhoan == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction(nameof(DangNhap));
            }

            model.HoTen = model.HoTen?.Trim() ?? string.Empty;
            model.Email = model.Email?.Trim() ?? string.Empty;
            model.GioiTinh = model.GioiTinh?.Trim() ?? string.Empty;
            model.SoDienThoai = taiKhoan.SoDienThoai;

            if (string.IsNullOrWhiteSpace(model.HoTen)
                || string.IsNullOrWhiteSpace(model.Email)
                || string.IsNullOrWhiteSpace(model.GioiTinh)
                || model.NgaySinh == null)
            {
                ViewBag.ThongBao = "Cập nhật thông tin thất bại.";
                return View(model);
            }

            if (taiKhoanDAL.KiemTraEmailTonTaiChoTaiKhoanKhac(maTaiKhoan.Value, model.Email))
            {
                ViewBag.ThongBao = "Cập nhật thông tin thất bại.";
                return View(model);
            }

            var capNhatTaiKhoanThanhCong = taiKhoanDAL.CapNhatThongTinCaNhan(maTaiKhoan.Value, model.HoTen, model.Email);
            var hoSoCaNhan = hoSoSucKhoeDAL.LayHoSoDauTienTheoTaiKhoan(maTaiKhoan.Value);
            var capNhatHoSoThanhCong = true;

            if (hoSoCaNhan != null)
            {
                hoSoCaNhan.HoTen = model.HoTen;
                hoSoCaNhan.GioiTinh = model.GioiTinh;
                hoSoCaNhan.NgaySinh = model.NgaySinh.Value;
                capNhatHoSoThanhCong = hoSoSucKhoeDAL.CapNhat(hoSoCaNhan);
            }
            else
            {
                capNhatHoSoThanhCong = hoSoSucKhoeDAL.Them(new HoSoSucKhoe
                {
                    MaTaiKhoan = maTaiKhoan.Value,
                    HoTen = model.HoTen,
                    GioiTinh = model.GioiTinh,
                    NgaySinh = model.NgaySinh.Value,
                    NgayTao = DateTime.Now
                });
            }

            if (capNhatTaiKhoanThanhCong && capNhatHoSoThanhCong)
            {
                HttpContext.Session.SetString("HoTen", model.HoTen);
                HttpContext.Session.SetString("Email", model.Email);
                TempData["ThongBao"] = "Cập nhật thông tin thành công.";
                return RedirectToAction(nameof(ThongTinCaNhan));
            }

            ViewBag.ThongBao = "Cập nhật thông tin thất bại.";
            return View(model);
        }

        private static string ChuanHoaVaiTro(string? vaiTro)
        {
            if (string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase)
                || string.Equals(vaiTro, "Quản trị viên", StringComparison.OrdinalIgnoreCase))
            {
                return "Admin";
            }

            if (string.Equals(vaiTro, "User", StringComparison.OrdinalIgnoreCase)
                || string.Equals(vaiTro, "NguoiDung", StringComparison.OrdinalIgnoreCase))
            {
                return "User";
            }

            return string.Empty;
        }

        private bool KiemTraThongTinTaiKhoanDatLaiMatKhau(string soDienThoai, string email, out TaiKhoan? taiKhoan)
        {
            taiKhoan = null;

            if (string.IsNullOrWhiteSpace(soDienThoai))
            {
                ViewBag.ThongBao = "Số điện thoại không được bỏ trống";
                return false;
            }

            if (!SoDienThoaiHopLe(soDienThoai))
            {
                ViewBag.ThongBao = "Số điện thoại phải gồm 10-11 chữ số và bắt đầu bằng 0";
                return false;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.ThongBao = "Email không được bỏ trống";
                return false;
            }

            if (!EmailHopLe(email))
            {
                ViewBag.ThongBao = "Email không đúng định dạng";
                return false;
            }

            taiKhoan = taiKhoanDAL.LayTaiKhoanTheoSoDienThoaiVaEmail(soDienThoai, email);
            if (taiKhoan == null)
            {
                ViewBag.ThongBao = "Số điện thoại hoặc email không đúng.";
                return false;
            }

            if (taiKhoan.DaXoa)
            {
                ViewBag.ThongBao = "Tài khoản không tồn tại hoặc đã bị xóa.";
                return false;
            }

            if (!taiKhoan.TrangThai)
            {
                ViewBag.ThongBao = "Tài khoản đang bị khóa, không thể đặt lại mật khẩu.";
                return false;
            }

            return true;
        }

        private void GanTrangThaiNhapMaXacNhan()
        {
            var hetHanTicks = HttpContext.Session.GetString("DatLaiMatKhau_HetHanTicks");
            ViewBag.YeuCauMaXacNhan = long.TryParse(hetHanTicks, out var ticks)
                && new DateTime(ticks, DateTimeKind.Utc) >= DateTime.UtcNow;
        }

        private void XoaPhienDatLaiMatKhau()
        {
            HttpContext.Session.Remove("DatLaiMatKhau_MaTaiKhoan");
            HttpContext.Session.Remove("DatLaiMatKhau_MaXacNhan");
            HttpContext.Session.Remove("DatLaiMatKhau_HetHanTicks");
        }

        private static bool EmailHopLe(string email)
        {
            try
            {
                var diaChi = new MailAddress(email.Trim());
                return string.Equals(diaChi.Address, email.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static bool SoDienThoaiHopLe(string soDienThoai)
        {
            return Regex.IsMatch(soDienThoai.Trim(), @"^0\d{9,10}$");
        }

        private static bool MatKhauHopLe(string matKhau)
        {
            return matKhau.Length >= 8
                && matKhau.Any(char.IsUpper)
                && matKhau.Any(char.IsLower)
                && matKhau.Any(char.IsDigit);
        }

    }
}
