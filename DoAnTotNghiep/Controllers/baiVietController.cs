using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    /// <summary>
    /// Lớp baiVietController là controller tiếp nhận request từ trình duyệt, gọi các lớp DAL hoặc service cần thiết và trả về View/Redirect phù hợp cho người dùng.
    /// </summary>
    public class baiVietController : Controller
    {
        private readonly baiVietCamNangDAL baiVietCamNangDAL;

        public baiVietController(baiVietCamNangDAL baiVietCamNangDAL)
        {
            this.baiVietCamNangDAL = baiVietCamNangDAL;
        }

        // Mục đích: action index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult index()
        {
            return View(baiVietCamNangDAL.layTatCa(chiLayDangHienThi: !laAdmin()));
        }

        // Mục đích: action chiTiet xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult chiTiet(int id)
        {
            var baiViet = baiVietCamNangDAL.layTheoMa(id);
            return baiViet == null ? NotFound() : View(baiViet);
        }

        [HttpGet]
        // Mục đích: action them xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult them()
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            return View(new baiVietCamNangModels());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action them xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult them(baiVietCamNangModels model)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            if (!ModelState.IsValid) return View(model);
            model.maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
            baiVietCamNangDAL.them(model);
            TempData["ThongBao"] = "Thêm bài viết thành công";
            return RedirectToAction(nameof(index));
        }

        [HttpGet]
        // Mục đích: action sua xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult sua(int id)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            var baiViet = baiVietCamNangDAL.layTheoMa(id);
            return baiViet == null ? NotFound() : View(baiViet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action sua xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult sua(baiVietCamNangModels model)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            if (!ModelState.IsValid) return View(model);
            baiVietCamNangDAL.capNhat(model);
            TempData["ThongBao"] = "Cập nhật bài viết thành công";
            return RedirectToAction(nameof(index));
        }

        // Mục đích: action laAdmin xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        private bool laAdmin()
        {
            return string.Equals(HttpContext.Session.GetString("VaiTro"), "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
