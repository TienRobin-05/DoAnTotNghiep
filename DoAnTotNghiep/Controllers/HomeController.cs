using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DoAnTotNghiep.Controllers
{
    public class HomeController : Controller
    {
        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Index()
        {
            return View();
        }

        // Mục đích: action ChucNang xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult ChucNang(string id)
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var dichDen = id switch
            {
                "quan-ly-vaccine" => ("Index", "Vaccine"),
                "quan-ly-mui-tiem" => ("Index", "MuiTiemVaccine"),
                "quan-ly-bai-viet" => ("Index", "AdminBaiViet"),
                "quan-ly-tu-van" => ("Index", "CauHoiTuVan"),
                "ho-so-suc-khoe" => ("Index", "HoSoSucKhoe"),
                "lich-tiem" => ("ChonHoSo", "LichTiem"),
                "cap-nhat-tiem" => ("ChonHoSo", "LichTiem"),
                "lich-su-tiem" => ("ChonHoSo", "LichSuTiem"),
                "thong-bao" => ("Index", "ThongBao"),
                "tra-cuu-vaccine" => ("TraCuu", "Vaccine"),
                "hoi-dap-tu-van" => ("Index", "CauHoiTuVan"),
                _ => (string.Empty, string.Empty)
            };

            if (!string.IsNullOrEmpty(dichDen.Item1))
            {
                return RedirectToAction(dichDen.Item1, dichDen.Item2);
            }

            ViewBag.MaChucNang = id;
            return View();
        }

        // Mục đích: action Privacy xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // Mục đích: action Error xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
