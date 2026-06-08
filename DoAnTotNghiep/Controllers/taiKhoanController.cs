using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    /// <summary>
    /// Lớp TaiKhoanController là controller tiếp nhận request từ trình duyệt, gọi các lớp DAL hoặc service cần thiết và trả về View/Redirect phù hợp cho người dùng.
    /// </summary>
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
        // Mục đích: action DangKy xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action DangKy xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
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
                Email = email,
                MatKhau = matKhau,
                SoDienThoai = soDienThoai,
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
        // Mục đích: action DangNhap xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action DangNhap xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
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

        [HttpGet]
        public IActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult QuenMatKhau(string soDienThoai, string email, string matKhauMoi, string xacNhanMatKhau)
        {
            soDienThoai = soDienThoai?.Trim() ?? string.Empty;
            email = email?.Trim() ?? string.Empty;

            ViewBag.SoDienThoai = soDienThoai;
            ViewBag.Email = email;

            if (string.IsNullOrWhiteSpace(soDienThoai))
            {
                ViewBag.ThongBao = "Số điện thoại không được bỏ trống";
                return View();
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.ThongBao = "Email không được bỏ trống";
                return View();
            }

            if (string.IsNullOrWhiteSpace(matKhauMoi))
            {
                ViewBag.ThongBao = "Mật khẩu mới không được bỏ trống";
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

            var taiKhoan = taiKhoanDAL.LayTaiKhoanTheoSoDienThoaiVaEmail(soDienThoai, email);
            if (taiKhoan == null)
            {
                ViewBag.ThongBao = "Số điện thoại hoặc email không đúng.";
                return View();
            }

            if (taiKhoan.DaXoa)
            {
                ViewBag.ThongBao = "Tài khoản không tồn tại hoặc đã bị xóa.";
                return View();
            }

            if (!taiKhoan.TrangThai)
            {
                ViewBag.ThongBao = "Tài khoản đang bị khóa, không thể đặt lại mật khẩu.";
                return View();
            }

            if (!taiKhoanDAL.DatLaiMatKhau(taiKhoan.MaTaiKhoan, matKhauMoi))
            {
                ViewBag.ThongBao = "Đặt lại mật khẩu thất bại, vui lòng thử lại.";
                return View();
            }

            TempData["ThongBao"] = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại.";
            return RedirectToAction(nameof(DangNhap));
        }

        // Mục đích: action DangXuat xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            TempData["ThongBao"] = "Đăng xuất thành công";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
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
            if (string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase))
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

    }
}
