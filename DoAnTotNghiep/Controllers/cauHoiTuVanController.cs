using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    /// <summary>
    /// Lớp CauHoiTuVanController là controller tiếp nhận request từ trình duyệt, gọi các lớp DAL hoặc service cần thiết và trả về View/Redirect phù hợp cho người dùng.
    /// </summary>
    public class CauHoiTuVanController : Controller
    {
        private readonly CauHoiTuVan_DAL cauHoiTuVanDAL;

        public CauHoiTuVanController(CauHoiTuVan_DAL cauHoiTuVanDAL)
        {
            this.cauHoiTuVanDAL = cauHoiTuVanDAL;
        }

        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Index()
        {
            if (LaAdmin())
            {
                ViewBag.LaAdmin = true;
                return View(cauHoiTuVanDAL.LayTatCa());
            }

            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            ViewBag.LaAdmin = false;
            return View(cauHoiTuVanDAL.LayDanhSachTheoNguoiGui(maTaiKhoan.Value));
        }

        [HttpGet]
        // Mục đích: action Create xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Create()
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            return View(new CauHoiTuVan());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action Create xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Create(CauHoiTuVan model)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            if (string.IsNullOrWhiteSpace(model.CauHoi))
            {
                ViewBag.ThongBao = "Vui lòng nhập nội dung câu hỏi.";
                return View(model);
            }

            var cauHoi = new CauHoiTuVan
            {
                MaNguoiGui = maTaiKhoan.Value,
                MaNguoiTraLoi = null,
                CauHoi = model.CauHoi.Trim(),
                CauTraLoi = string.Empty,
                NgayGui = DateTime.Now,
                NgayTraLoi = null,
                TrangThai = "Chưa trả lời"
            };

            if (!cauHoiTuVanDAL.GuiCauHoi(cauHoi))
            {
                ViewBag.ThongBao = "Gửi câu hỏi thất bại, vui lòng thử lại.";
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // Mục đích: action Details xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Details(int maCauHoi)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var cauHoi = cauHoiTuVanDAL.LayTheoIdCuaNguoiGui(maCauHoi, maTaiKhoan.Value);
            return cauHoi == null ? NotFound() : View(cauHoi);
        }

        [HttpGet]
        public IActionResult traLoi(int id, int maCauHoi = 0)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var maCauHoiCanTraLoi = maCauHoi > 0 ? maCauHoi : id;
            var cauHoi = cauHoiTuVanDAL.LayTheoId(maCauHoiCanTraLoi);
            return cauHoi == null ? NotFound() : View(cauHoi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult traLoi(int maCauHoi, string cauTraLoi)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!LaAdmin() || maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            if (string.IsNullOrWhiteSpace(cauTraLoi))
            {
                var cauHoi = cauHoiTuVanDAL.LayTheoId(maCauHoi);
                ViewBag.ThongBao = "Vui lòng nhập câu trả lời.";
                return cauHoi == null ? NotFound() : View(cauHoi);
            }

            cauHoiTuVanDAL.TraLoi(maCauHoi, maTaiKhoan.Value, cauTraLoi.Trim());
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        // Mục đích: action guiCauHoi xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult guiCauHoi()
        {
            return RedirectToAction(nameof(Create));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action guiCauHoi xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult guiCauHoi(CauHoiTuVan model)
        {
            return Create(model);
        }

        // Mục đích: action LayMaTaiKhoanUser xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        private int? LayMaTaiKhoanUser()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return null;

            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "User") return null;

            return maTaiKhoan.Value;
        }

        private bool LaAdmin()
        {
            return HttpContext.Session.GetInt32("MaTaiKhoan") != null
                && HttpContext.Session.GetString("VaiTro") == "Admin";
        }
    }
}
