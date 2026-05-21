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
        public IActionResult DangNhap(string soDienThoai, string matKhau)
        {
            // Đăng nhập chỉ dùng số điện thoại và mật khẩu, không dùng email.
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

            if (!taiKhoan.TrangThai)
            {
                ViewBag.ThongBao = "Tài khoản đã bị khóa";
                return View();
            }

            HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", taiKhoan.HoTen ?? "");
            HttpContext.Session.SetString("VaiTro", taiKhoan.VaiTro ?? "");
            HttpContext.Session.SetString("SoDienThoai", taiKhoan.SoDienThoai ?? "");
            HttpContext.Session.SetString("Email", taiKhoan.Email ?? "");

            // Chỉ chấp nhận đúng hai vai trò trong database: Admin và User.
            if (taiKhoan.VaiTro == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            if (taiKhoan.VaiTro == "User")
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

        // Mục đích: action DangXuat xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
