using DoAnTotNghiep.DAL;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    /// <summary>
    /// Lớp AdminController là controller tiếp nhận request từ trình duyệt, gọi các lớp DAL hoặc service cần thiết và trả về View/Redirect phù hợp cho người dùng.
    /// </summary>
    public class AdminController : Controller
    {
        private readonly taiKhoanDAL taiKhoanDAL;
        private readonly vaccineDAL vaccineDAL;
        private readonly lichTiemDAL lichTiemDAL;
        private readonly cauHoiTuVanDAL cauHoiTuVanDAL;

        public AdminController(taiKhoanDAL taiKhoanDAL, vaccineDAL vaccineDAL, lichTiemDAL lichTiemDAL, cauHoiTuVanDAL cauHoiTuVanDAL)
        {
            this.taiKhoanDAL = taiKhoanDAL;
            this.vaccineDAL = vaccineDAL;
            this.lichTiemDAL = lichTiemDAL;
            this.cauHoiTuVanDAL = cauHoiTuVanDAL;
        }

        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Index()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            ViewBag.HoTen = HttpContext.Session.GetString("HoTen");
            ViewBag.SoTaiKhoan = taiKhoanDAL.layTatCa().Count;
            ViewBag.SoVaccine = vaccineDAL.layTatCa().Count;
            ViewBag.SoLichTiem = lichTiemDAL.layTatCa().Count;
            ViewBag.SoCauHoiChoTraLoi = cauHoiTuVanDAL.layTatCa().Count(x => x.trangThai != "Đã trả lời");
            return View();
        }

        // Mục đích: action taiKhoan xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult taiKhoan()
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            return View(taiKhoanDAL.layTatCa());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action doiTrangThaiTaiKhoan xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult doiTrangThaiTaiKhoan(int maTaiKhoan, bool trangThai)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            taiKhoanDAL.doiTrangThai(maTaiKhoan, trangThai);
            return RedirectToAction(nameof(taiKhoan));
        }

        // Mục đích: action LaAdmin xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        private bool LaAdmin()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            return maTaiKhoan != null && vaiTro == "Admin";
        }
    }
}
