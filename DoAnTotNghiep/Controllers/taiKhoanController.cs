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
                ViewBag.ThongBao = "Há» tÃªn khÃ´ng Ä‘Æ°á»£c bá» trá»‘ng";
                return View();
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.ThongBao = "Email khÃ´ng Ä‘Æ°á»£c bá» trá»‘ng";
                return View();
            }
            if (string.IsNullOrWhiteSpace(soDienThoai))
            {
                ViewBag.ThongBao = "Sá»‘ Ä‘iá»‡n thoáº¡i khÃ´ng Ä‘Æ°á»£c bá» trá»‘ng";
                return View();
            }
            if (string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.ThongBao = "Máº­t kháº©u khÃ´ng Ä‘Æ°á»£c bá» trá»‘ng";
                return View();
            }
            if (string.IsNullOrWhiteSpace(nhapLaiMatKhau))
            {
                ViewBag.ThongBao = "Nháº­p láº¡i máº­t kháº©u khÃ´ng Ä‘Æ°á»£c bá» trá»‘ng";
                return View();
            }
            if (matKhau != nhapLaiMatKhau)
            {
                ViewBag.ThongBao = "Nháº­p láº¡i máº­t kháº©u khÃ´ng trÃ¹ng khá»›p";
                return View();
            }
            if (taiKhoanDAL.KiemTraSoDienThoaiTonTai(soDienThoai))
            {
                ViewBag.ThongBao = "Sá»‘ Ä‘iá»‡n thoáº¡i Ä‘Ã£ Ä‘Æ°á»£c sá»­ dá»¥ng";
                return View();
            }
            if (taiKhoanDAL.KiemTraEmailTonTai(email))
            {
                ViewBag.ThongBao = "Email Ä‘Ã£ Ä‘Æ°á»£c sá»­ dá»¥ng";
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
                TempData["ThongBao"] = "ÄÄƒng kÃ½ thÃ nh cÃ´ng. Vui lÃ²ng Ä‘Äƒng nháº­p.";
                return RedirectToAction(nameof(DangNhap));
            }

            ViewBag.ThongBao = "ÄÄƒng kÃ½ tháº¥t báº¡i, vui lÃ²ng thá»­ láº¡i";
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
            // ÄÄƒng nháº­p chá»‰ dÃ¹ng sá»‘ Ä‘iá»‡n thoáº¡i vÃ  máº­t kháº©u, khÃ´ng dÃ¹ng email.
            if (string.IsNullOrWhiteSpace(soDienThoai) || string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.ThongBao = "Sá»‘ Ä‘iá»‡n thoáº¡i hoáº·c máº­t kháº©u khÃ´ng Ä‘Æ°á»£c bá» trá»‘ng";
                return View();
            }

            var taiKhoan = taiKhoanDAL.DangNhap(soDienThoai, matKhau);
            if (taiKhoan == null)
            {
                ViewBag.ThongBao = "Sá»‘ Ä‘iá»‡n thoáº¡i hoáº·c máº­t kháº©u khÃ´ng Ä‘Ãºng";
                return View();
            }

            if (!taiKhoan.TrangThai)
            {
                ViewBag.ThongBao = "TÃ i khoáº£n Ä‘Ã£ bá»‹ khÃ³a";
                return View();
            }

            HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", taiKhoan.HoTen ?? "");
            HttpContext.Session.SetString("VaiTro", taiKhoan.VaiTro ?? "");
            HttpContext.Session.SetString("SoDienThoai", taiKhoan.SoDienThoai ?? "");
            HttpContext.Session.SetString("Email", taiKhoan.Email ?? "");

            // Chá»‰ cháº¥p nháº­n Ä‘Ãºng hai vai trÃ² trong database: Admin vÃ  User.
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

            ViewBag.ThongBao = "Vai trÃ² tÃ i khoáº£n khÃ´ng há»£p lá»‡";
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
